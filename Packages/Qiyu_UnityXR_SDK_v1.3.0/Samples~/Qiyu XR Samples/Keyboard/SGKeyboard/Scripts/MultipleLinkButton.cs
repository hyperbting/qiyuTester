﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class UnderlineProperty
{
    public Color _color;
    public Vector3 _position;
    public float _width;
    public float _height;
    public Vector2 _privot;
}

public class MultipleLinkButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Text _text;
    private int _curCharacterCount = 0;
    private List<Image> _lines = new List<Image>();
    private System.Action _clickEvent = null; //下划线点击事件
    private bool _isInitUnderline = false;
    private bool m_bNeedPaint = false;
    private string m_strLastText;

    public System.Action ClickEvent
    {
        get
        {
            return _clickEvent;
        }

        set
        {
            _clickEvent = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        _text = transform.GetComponent<Text>();
        _text.gameObject.AddComponent<Button>().onClick.AddListener(() =>
        {
            if (ClickEvent != null)
                ClickEvent();
        });
    }

    void Update()
    {
        //做初始化
        if (m_bNeedPaint && _text.cachedTextGenerator.lineCount > 0 && !_isInitUnderline)
        {
            Debug.Log("MultipleLinkButton::Update1 ");
            _isInitUnderline = true;

            _curCharacterCount = _text.cachedTextGenerator.characterCount;
            List<UnderlineProperty> list = GetUnderlinePropertys();
            CreateUnderLines(list);   
        }

        //刷新
        if (m_bNeedPaint && _isInitUnderline && _curCharacterCount != _text.cachedTextGenerator.characterCount)
        {
            Debug.Log("MultipleLinkButton::Update2 ");
            Debug.Log("MultipleLinkButton::GetUnderlinePropertys charcnt : " + _text.cachedTextGenerator.characterCount);
            _curCharacterCount = _text.cachedTextGenerator.characterCount;
            List<UnderlineProperty> list = GetUnderlinePropertys();
            CreateUnderLines(list);
        }
    }

    List<UnderlineProperty> GetUnderlinePropertys()
    {
        Debug.Log("MultipleLinkButton::GetUnderlinePropertys linecount : " + _text.cachedTextGenerator.lineCount);
        List<UnderlineProperty> list = new List<UnderlineProperty>();
        for (int i = 0; i < _text.cachedTextGenerator.lineCount; i++)
        {// !< text 行数
            var curPos = _text.cachedTextGenerator.characters[_text.cachedTextGenerator.lines[i].startCharIdx].cursorPos;
            UnderlineProperty property = new UnderlineProperty
            {
                _color = _text.color,
                _height = _text.fontSize / 10 == 0 ? 1 : _text.fontSize / 10,
                _width = GetWidth(_text.cachedTextGenerator.lines[i].startCharIdx, _text.cachedTextGenerator.characters),
                _position = new Vector3(curPos.x, curPos.y - _text.cachedTextGenerator.lines[i].height, 0),
                _privot = GetTextAnchorPivot(_text.alignment)
            };

            list.Add(property);
        }

        return list;
    }

    float GetWidth(int idx, IList<UICharInfo> info)
    {
        float width = 0;
        float start = info[idx].cursorPos.x;
        for (int i = idx; i < info.Count - 1; i++)
        {
            if (info[i].cursorPos.x > info[i + 1].cursorPos.x)
            {
                width = info[i].cursorPos.x - start;
                break;
            }

            if (info.Count - 1 == i + 1)
                width = info[i + 1].cursorPos.x - start;
        }
        return width;
    }

    Vector2 GetTextAnchorPivot(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.LowerLeft: return new Vector2(0, 0);
            case TextAnchor.LowerCenter: return new Vector2(0.5f, 0);
            case TextAnchor.LowerRight: return new Vector2(1, 0);
            case TextAnchor.MiddleLeft: return new Vector2(0, 0.5f);
            case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
            case TextAnchor.MiddleRight: return new Vector2(1, 0.5f);
            case TextAnchor.UpperLeft: return new Vector2(0, 1);
            case TextAnchor.UpperCenter: return new Vector2(0.5f, 1);
            case TextAnchor.UpperRight: return new Vector2(1, 1);
            default: return Vector2.zero;
        }
    }

    void CreateUnderLines(List<UnderlineProperty> list)
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        _lines.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            //初始化
            GameObject obj = new GameObject();
            obj.transform.SetParent(transform, false);
            obj.name = "underline" + i;
            _lines.Add(obj.AddComponent<Image>());
            _lines[i].rectTransform.pivot = list[i]._privot;

            //颜色和大小
            var tex = new Texture2D((int)list[i]._width, (int)list[i]._height, TextureFormat.ARGB32, false);
            Color[] colors = tex.GetPixels();
            for (int j = 0; j < colors.Length; j++)
                colors[j] = list[i]._color;
            tex.SetPixels(colors);
            tex.Apply();
            _lines[i].sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            _lines[i].SetNativeSize();
            _lines[i].rectTransform.sizeDelta = new Vector2(_lines[i].rectTransform.sizeDelta.x, _lines[i].rectTransform.sizeDelta.y);

            //座标
            float x = list[i]._position.x;
            if (_text.alignment == TextAnchor.MiddleCenter || _text.alignment == TextAnchor.UpperCenter || _text.alignment == TextAnchor.LowerCenter)
                x = 0;
            if (_text.alignment == TextAnchor.MiddleRight || _text.alignment == TextAnchor.UpperRight || _text.alignment == TextAnchor.LowerRight)
                x += list[i]._width;
                x += m_strLastText.Length;
            _lines[i].rectTransform.anchoredPosition = new Vector2(x, list[i]._position.y);
        }
    }
    public void CleanUnderLines()
    {
        List<UnderlineProperty> list = GetUnderlinePropertys();

        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        _lines.Clear();

    }

    /*实现下划线同步点击效果*/
    public void OnPointerDown(PointerEventData eventData)
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            Color[] colors = _lines[i].sprite.texture.GetPixels();
            for (int j = 0; j < colors.Length; j++)
                colors[j] = new Color(colors[j].r, colors[j].g, colors[j].b, colors[j].a * 0.70f);
            _lines[i].sprite.texture.SetPixels(colors);
            _lines[i].sprite.texture.Apply();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            Color[] colors = _lines[i].sprite.texture.GetPixels();
            for (int j = 0; j < colors.Length; j++)
                colors[j] = new Color(colors[j].r, colors[j].g, colors[j].b, colors[j].a / 0.70f);
            _lines[i].sprite.texture.SetPixels(colors);
            _lines[i].sprite.texture.Apply();
        }
    }

    public void SetPaintUnderLine(bool p_bNeedPaint)
    {
        Debug.Log("MultipleLinkButton::SetPaintUnderLine : " + p_bNeedPaint);
        m_bNeedPaint = p_bNeedPaint;
    }
    public void SetUnderLineText(string p_strPaintText, string p_strLastText)
    {
        //!< 已上屏 string + focus string 
        Debug.Log("MultipleLinkButton::SetUnderLineText : " + p_strPaintText);
        m_strLastText = p_strLastText;
        _text.text = "";
        _text.text += p_strLastText;
        _text.text += p_strPaintText;// !< text obj save text； text已被更新未focus text
        Debug.Log("MultipleLinkButton::SetUnderLineText text : " + _text.text);
    }
}
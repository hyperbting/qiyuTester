using System;
using System.Collections.Generic;
using System.Text;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.UI;

public class DebugShow : MonoBehaviour
{
    public static DebugShow Instance;
    Text _showText;
    List<string> _showList = new List<string>();

    private void Awake()
    {
        _showText = transform.GetComponent<Text>();
        Instance = this;
    }

    void Start()
    {
    }

    private void OnDestroy()
    {
    }

    private void Update()
    {
       
    }
    void Add(string info)
    {
        //Debug.Log("xing_debug " + info);
        if (_showList.Count > 10)
        {
            _showList.RemoveAt(0);
        }
        _showList.Add(info);

        StringBuilder sb = new StringBuilder(5);
        for (int i = 0; i < _showList.Count; i++)
        {
            sb.Append(_showList[i]);
            sb.Append("\n");
        }
        _showText.text = sb.ToString();
    } 
}


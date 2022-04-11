using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SGIme : ImeBase
{
#if UNITY_EDITOR
    private bool mUseAndroid = false;
#else
    private bool mUseAndroid = true;
#endif
    private static AndroidJavaObject javaIme = null;
    private ImeDelegateBase mDelegate;
    private Vector2 mTextureSize;
    private string mStrCommit;
    private bool mShow = false;

    public bool Create(ImeDelegateBase pDelegate)
    {
        Debug.Log("sogou SGIme ime create");
        mDelegate = pDelegate;
        JavaInit();
        Hide();
        return true;
    }
    public void GetSize(ref Vector2 size)
    {
        size = mTextureSize;
        Debug.Log("sogou SGIme ime getsize func : " + size[0] + "," + size[1]);
    }
    public void Draw(Texture2D tex)
    {
        if (tex == null) return;

        if (IsInited() && IsNeedUpdate())
        {
            //Debug.Log("sogou SGIme ime draw");
            byte[] data = javaIme.Call<byte[]>("getTextureData");
            tex.LoadRawTextureData(data);
            tex.Apply();
        }
    }
    public void Show(SGImeInputType typeInput, SGImeTextType typeText)
    {
        if (IsInited())
        {
            Debug.Log("sogou SGIme ime show:" + typeInput + "," + typeText);
            javaIme.Call<bool>("show", (int)typeInput, (int)typeText);
            //mShow = true;
        }
    }
    public void Hide()
    {
        if (IsInited())
        {
            Debug.Log("sogou SGIme ime hide");
            javaIme.Call<bool>("hide");
            //mShow = false;
        }
    }
    public void OnTouch(float x, float y, SGImeMotionEventType type)
    {
        if (IsInited())
        {
            //Debug.Log("sogou SGIme ime ontouch:(" + x + "," + y + "),type=" + type);
            bool bHandle = javaIme.Call<bool>("onTouch", x, y, (int)type);
        }
    }
    public void UpdateData()
    {
        if (!IsInited())
        {
            return;
        }

        //update commit
        SGImeKey nCommit = (SGImeKey)GetCommitCode();

        if (nCommit == SGImeKey.KEYCODE_COMMIT)
        {
            mStrCommit = GetCommitString();
            Debug.Log("sogou SGIme ime updatedata commit string:" + mStrCommit);
            mDelegate.OnIMECommit(mStrCommit);
        }
        else if (nCommit != SGImeKey.KEYCODE_UNKNOWN)
        {
            Debug.Log("sogou SGIme ime updatedata commit key:" + nCommit);
            mDelegate.OnIMEKey(nCommit);
        }
        else if (Input.GetKeyUp(KeyCode.Backspace))
        {
            Debug.Log("sogou SGIme ime get key up, commit key:" + SGImeKey.KEYCODE_DEL);
            mDelegate.OnIMEKey(SGImeKey.KEYCODE_DEL);
        }
        //update hide
        bool bShow = IsShow();    
        if (!bShow && mShow)
        {
            Debug.Log("sogou SGIme Hide");
            mShow = bShow;
            mDelegate.OnIMEHide();
        }
        else if (bShow && !mShow)
        {
            Debug.Log("sogou SGIme Show");
            mShow = bShow;
            mDelegate.OnIMEShow(mTextureSize);
        }
    }
    private void JavaInit()
    {
        Debug.Log("sogou SGIme ime start JavaInit");
        if (mUseAndroid && javaIme == null)
        {
            Debug.Log("sogou SGIme JavaInit");
            javaIme = new AndroidJavaObject("com.sohu.inputmethod.sogou.sgrenderproxy.RenderProxyUnity");
        }
        if (mUseAndroid)
        {
            int[] s = javaIme.Call<int[]>("getSize");
            mTextureSize[0] = s[0];
            mTextureSize[1] = s[1];
            Debug.Log("sogou ime start JavaInit, getKeyBoardSize size = " + mTextureSize[0] + ":" + mTextureSize[1]);
        }
    }

    private bool IsInited()
    {
        return mUseAndroid && null != javaIme;
    }

    private bool IsNeedUpdate()
    {
        bool bNeedUpdate = javaIme.Call<bool>("isNeedRefresh");
        return bNeedUpdate;
    }

    private int GetCommitCode()
    {
        return javaIme.Call<int>("getCommitCode");
    }
    private string GetCommitString()
    {
        string strCommit = javaIme.Call<string>("getCommitString");
        return strCommit;
    }
    public bool IsShow()
    {
        return javaIme.Call<bool>("isShow");
    }
}

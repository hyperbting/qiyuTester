using Qiyi.UI.InputField;
using System;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SGViewGather
{
    private GameObject[] mViews;

    public SGViewGather(GameObject[] param)
    {
        mViews = param;
    }
    public void SetActive(bool bActive)
    {
        foreach (GameObject view in mViews)
        {
            if(view != null)
             view.SetActive(bActive);
        }
    }
    public bool FindName(string name)
    {
        foreach (GameObject view in mViews)
        {
            if (view.name == name)
            {
                return true;
            }
        }
        return false;
    }
    public void SetTexture(Texture2D tex)
    {
        foreach (GameObject view in mViews)
        {
            Renderer rend = view.GetComponent<Renderer>();
            rend.material.mainTexture = tex;
        }
    }
}

public class SGMouseTracker
{
    private bool mDownOld = false;
    private Vector2 mPtOld = new Vector2();
    private SGImeMotionEventType mEvent;
    private const float mTrackRadius = 10.0f;
    private long mTimeDown;
    private bool mLongPressed = false;
    private long mIntervelLongPress = 100;

    public bool Track(Vector2 pt, bool bDown)
    {
        bool bRes = false;
        if (bDown)
        {
            if (mDownOld)
            {
                mEvent = SGImeMotionEventType.ACTION_MOVE;
                if ( !mLongPressed )
                {
                    long timeDiff = DateTime.Now.Ticks - mTimeDown;
                    if (timeDiff > mIntervelLongPress)
                    {
                        mLongPressed = true;
                        mEvent = SGImeMotionEventType.ACTION_LONGPRESS;
                        bRes = true; //force sendmessage
                    }
                }
            }
            else
            {
                mEvent = SGImeMotionEventType.ACTION_DOWN;
                mTimeDown = DateTime.Now.Ticks;
                mLongPressed = false;
            }
        }
        else
        {
            if (mDownOld)
            {
                mEvent = SGImeMotionEventType.ACTION_UP;
            }
            else
            {
                //mEvent = SGImeMotionEventType.ACTION_HOVER_MOVE;
                mEvent = SGImeMotionEventType.ACTION_MOVE; //c++代码只识别move事件
            }
        }
        if (mDownOld != bDown)
        {
            bRes = true;
        }
        else if ( PointDist(mPtOld, pt) > mTrackRadius )
        {
            bRes = true;
        }
        mDownOld = bDown;

        if (bRes)
        {
            mPtOld = pt;
        }
        return bRes;
    }

    public bool TrackOuter()
    {
        bool bRes = false;
        if (mEvent != SGImeMotionEventType.ACTION_OUTSIDE)
        {
            SGImeMotionEventType eventOld = mEvent;
            mEvent = SGImeMotionEventType.ACTION_OUTSIDE;
        }
        return bRes;
    }

    public Vector2 GetPoint()
    {
        return mPtOld;
    }
    public SGImeMotionEventType GetEvent()
    {
        return mEvent;
    }

    private float PointDist(Vector2 ptNew, Vector2 ptOld)
    {
        return Math.Abs(ptNew[0] - ptOld[0]) + Math.Abs(ptNew[1] - ptOld[1]);
    }
}

public class ImeDelegateImpl_kbd : ImeDelegateBase
{
    public static IVrInputField _inputField;
    public GameObject[] mKbdViews;
    public SGViewGather mKbdView;
    public ImeManager mManager;
    private Texture2D mTexture;
    private Vector2 mTextureSize = new Vector2(780,390);
    private Vector2 mPtKbd = new Vector2();
    private SGMouseTracker mTracker = new SGMouseTracker();
    //ImeDelegateBase

    private float fixDistance = 1.5f;
    private Vector3 posOffSet = new Vector3(0, 0, 0);
    private Transform tr;
    private System.Action<Transform> setPoseAction = null;

    public XRRayInteractor leftXRRayInteractor;
    public XRRayInteractor rightXRRayInteractor;

    public void Awake()
    {
        tr = transform.Find("TR");
    }
    public void SetPos()
    {
        if (setPoseAction != null)
        {
            setPoseAction(tr);
        }
        else {
            tr.transform.localPosition = new Vector3(0, 0, fixDistance);
            tr.transform.localEulerAngles = new Vector3(-20, 180, 0);
            tr.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            transform.localPosition = new Vector3(0, -0.4f, 0);
            transform.localEulerAngles = new Vector3(0, QiyuManager.Instance.Head.localEulerAngles.y, 0);
        }
    }
    public override void SetPose(System.Action<Transform> fun)
    {
        setPoseAction = fun;
    }

    public override Transform GetTransform()
    {
        return tr.transform;
    }

    public override void OnIMEShow(Vector2 vSize)
    {
        Debug.Log("sogou DelegateImpl_kbd OnIMEShow");
        mTextureSize = vSize;
        Debug.Log("sogou DelegateImpl_kbd OnIMEShow, size = " + mTextureSize[0] + ":" + mTextureSize[1]);
        CreateTexture(vSize);
        mManager.Draw(mTexture);
        mKbdView.SetActive(true);
        SetPos();
    }
    public override void OnIMEHide()
    {
        Debug.Log("sogou DelegateImpl_kbd OnIMEHide");
        if(mKbdView!=null)
            mKbdView.SetActive(false);
        FinishInput();
    }
    public override void OnIMECommit(string strCommit)
    {
        if (currentXRRayInteractor != null)
        {
            if (currentXRRayInteractor == leftXRRayInteractor)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(1, 1, 0.1f);
            }
            else
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(1, 1, 0.1f);
            }
        }
        foreach (var c in strCommit)
        {
            KeyPressEvent(KeyCode.None, c, EventModifiers.None);
        }
    }
    public override void OnIMEKey(SGImeKey key)
    {
        switch (key)
        {
            case SGImeKey.KEYCODE_DEL:
                KeyPressEvent(KeyCode.Backspace,'\0', EventModifiers.None);
                break;
            case SGImeKey.KEYCODE_ENTER:
                FinishInput();
                break;
        }
    }
    public void FinishInput()
    {
        if (_inputField != null)
        {
            _inputField.FinishInput();
            _inputField.DeactivateInputField();
            _inputField = null;
        }
    }

    public override void OnIMEError(SGImeError nType, string strErr)
    {
    }

    public  void KeyPressEvent(KeyCode code, char c, EventModifiers modifiers)
    {
        if (_inputField == null)
        {
            return;
        }
        UnityEngine.Event evt = new UnityEngine.Event();
        evt.type = EventType.KeyDown;
        evt.keyCode = code;
        evt.modifiers = modifiers;
        evt.character = c;
        _inputField.ProcessEvent(evt);
    }

    //MonoBehaviour
    void Start()
    {
        mKbdView = new SGViewGather(mKbdViews);
        //CreateTexture(mTextureSize);
#if UNITY_EDITOR
#else
        mKbdView.SetActive(false);
#endif
    }
    private void OnDestroy()
    {

    }
    
    void Update()
    { 
        RaycastHitCheck(leftXRRayInteractor,ImeManager.Instance.LeftHand);
        RaycastHitCheck(rightXRRayInteractor, ImeManager.Instance.RightHand);
    }

    private void RaycastHitCheck(XRRayInteractor xRRayInteractor, InputDevice inputDevice)
    {
        if (xRRayInteractor == null || inputDevice == null)
        {
            return;
        }            
        RaycastHit hitInfo;
        bool isValid = xRRayInteractor.TryGetCurrent3DRaycastHit(out hitInfo);
        if (hitInfo.collider == null)
        {
            if (currentXRRayInteractor == xRRayInteractor)
            {
                currentXRRayInteractor = null;
            }
            return;
        }          
        XRController xRController = xRRayInteractor.gameObject.GetComponent<XRController>();
        if (xRController != null)
        {
            InputHelpers.IsPressed(xRController.inputDevice, xRController.uiPressUsage, out var isuiPressUsage, xRController.axisToPressThreshold);
            CheckMouseEvent(hitInfo, isuiPressUsage, isValid,xRRayInteractor);
        }
        else
        {
            ActionBasedController actionBasedController = xRRayInteractor.gameObject.GetComponent<ActionBasedController>();
            CheckMouseEvent(hitInfo, actionBasedController.uiPressAction.action.triggered, isValid,xRRayInteractor);
        }
        mManager.Draw(mTexture);
    }

    //other
    private void CreateTexture(Vector2 vSize)
    {
        if (mTexture)
        {
            return;
        }
#if UNITY_EDITOR
#else
        // Create a texture
        mTexture = new Texture2D((int)vSize.x, (int)vSize.y, TextureFormat.RGBA32, false);
        // Set point filtering just so we can see the pixels clearly
        mTexture.filterMode = FilterMode.Trilinear;
        // Call Apply() so it's actually uploaded to the GPU
        mTexture.Apply();

        Debug.Log(" sogou DelegateImpl_kbd CreateTexture: texture created");

        // Set texture onto cube
        //GameObject cube = GameObject.Find("Cube");
        //cube.GetComponent<Renderer>().material.mainTexture = mTexture;

        // Set texture onto kbdview
        mKbdView.SetTexture(mTexture);
#endif
    }

    private void DispatchMessageToAndroid(SGImeMotionEventType type, Vector2 pt)
    {
        if (null != mManager)
        {
            mManager.OnTouch(pt.x, pt.y, type);
        }
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }

    private XRRayInteractor currentXRRayInteractor=null;
    private void CheckMouseEvent(RaycastHit hit, bool isDown, bool isValid,XRRayInteractor xRRayInteractor)
    {
        bool isPoint2UV = Point2UV(hit, ref mPtKbd);
        if (isPoint2UV)
        {
            if (currentXRRayInteractor == null)
                currentXRRayInteractor = xRRayInteractor;
        }
        else
        {
            if(currentXRRayInteractor == xRRayInteractor)
                currentXRRayInteractor = null;
        }
        if (!isValid)
        {
            if (currentXRRayInteractor == xRRayInteractor)
            {
                currentXRRayInteractor = null;
            }
        }
        if (currentXRRayInteractor != xRRayInteractor)
            return;
        if (isValid && isPoint2UV)
        {
            if (mTracker.Track(mPtKbd, isDown))
            {
                DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
            }
        }
        else if (mTracker.TrackOuter())
        {
            DispatchMessageToAndroid(mTracker.GetEvent(), mTracker.GetPoint());
        }
    }

    private bool Point2UV(RaycastHit hitInfo, ref Vector2 ptUV)
    {      
        bool bRes = false;
        string name = hitInfo.collider.gameObject.name;
        if (mKbdView.FindName(name))
        {
            GameObject kbd = hitInfo.collider.gameObject;
            Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
            Vector2 pixelUV = hitInfo.textureCoord;
            Renderer rend = hitInfo.transform.GetComponent<Renderer>();
            ptUV.x = pixelUV.x * mTextureSize.x;
            ptUV.y = (1 - pixelUV.y) * mTextureSize.y;
            //Debug.Log("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")" + " w=" + texSize.x + ",h=" + texSize.y);
            bRes = true;
        }
        return bRes;
    }

}

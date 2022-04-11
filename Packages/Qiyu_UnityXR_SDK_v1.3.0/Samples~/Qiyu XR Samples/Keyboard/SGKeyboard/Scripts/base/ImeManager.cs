using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ImeManager : MonoBehaviour
{
    public static ImeManager Instance;
    public ImeDelegateBase mDelegate;
    private ImeBase mIme;
    private Vector2 mSize;
    private bool mIsPaused = false;
    private bool mIsFocus = false;

    public InputDevice RightHand;
    public InputDevice Head;
    public InputDevice LeftHand;

    private void Awake()
    {
        Instance = this;
        InitInputDevices();
    }

    private void InitInputDevices()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);
        var usage = new List<InputFeatureUsage>();
        foreach (var device in inputDevices)
        {
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));

            if (device.role == InputDeviceRole.RightHanded)
            {
                RightHand = device;
            }
            else if (device.role == InputDeviceRole.Generic)
            {
                Head = device;
            }
            if (device.role == InputDeviceRole.LeftHanded)
            {
                LeftHand = device;
            }
        }
    }

    //MonoBehaviour
    void Start()
    {
        if (null != mDelegate)
        {
#if UNITY_EDITOR
            mIme = new DummyIme();
#else
            mIme = new SGIme();
#endif
            mIme.Create(mDelegate);
        }
    }

    private void OnDestroy()
    {
        Hide();
    }

    void Update()
    {
        if (null == mIme)
        {
            return;
        }
        mIme.UpdateData();
        //if (mIsFocus && !mIsPaused)
        //{
        //    mIme.UpdateData();
        //}
    }

    //export
    public void Show(SGImeInputType typeInput, SGImeTextType typeText)
    {
        mIme.Show(typeInput, typeText);
        mIme.GetSize(ref mSize);
    }

    public void Hide()
    {
        mIme.Hide();
        mDelegate.OnIMEHide();
    }

    public bool IsShow()
    {
      return  mIme.IsShow();
    }

    public void SetPose(System.Action<Transform> fun)
    {
        mDelegate.SetPose(fun);
    }
    public Transform GetTransform()
    {
       return mDelegate.GetTransform();
    }
    public void Draw(Texture2D tex)
    {
        mIme.Draw(tex);
    }

    public void OnTouch(float x, float y, SGImeMotionEventType type)
    {
        mIme.OnTouch(x, y, type);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        //if (!hasFocus)
        //{
        //    Hide();
        //}
        mIsFocus = hasFocus;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Hide();
        }
        mIsPaused = pauseStatus;
    }
}

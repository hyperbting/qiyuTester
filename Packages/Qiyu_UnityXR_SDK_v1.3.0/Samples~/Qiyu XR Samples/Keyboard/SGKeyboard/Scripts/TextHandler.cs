using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextHandler : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public ImeManager mManager;
    public SGImeInputType mInputType;
    public SGImeTextType mTextType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    //OnPointerDown is also required to receive OnPointerUp callbacks
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("TextHandler OnPointerUp");
        LogEvent("click text", eventData);
        mManager.Show(mInputType, mTextType);
    }

    private void LogEvent(string prefix, PointerEventData eventData)
    {
        Debug.Log(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x + ",y=" + eventData.position.y);
    }

}

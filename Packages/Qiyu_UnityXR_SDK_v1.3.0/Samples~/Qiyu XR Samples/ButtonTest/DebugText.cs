using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    Text text;
    private void Awake()
    {
        text = transform.Find("Text").GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
      
    }

    public void SetText(string info)
    {
        text.text = name+" : "+info;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

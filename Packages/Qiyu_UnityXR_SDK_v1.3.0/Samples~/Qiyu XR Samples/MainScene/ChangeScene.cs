using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void GoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Goto " + sceneName);
    }
}



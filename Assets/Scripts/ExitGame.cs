using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    public void doExitGame()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
        //Debug.Log("do Exit");
    }

}

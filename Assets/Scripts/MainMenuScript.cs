using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuScript : MonoBehaviour
{
    public void Continue()
    {
        CrossSceneInformation.CrossSceneInfo = "lightbeam";
        SceneManager.LoadScene(PlayerPrefs.GetInt("SavedScene"));
    }

    public void StartGame()
    {
        CrossSceneInformation.CrossSceneInfo = "arrow";
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}

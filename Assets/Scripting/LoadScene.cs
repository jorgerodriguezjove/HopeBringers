using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
  

    public void SceneToLoad(string sceneLoaded) {
       
        SceneManager.LoadScene(sceneLoaded, LoadSceneMode.Single);
    }

    public void ExitGame()
    {

        Application.Quit();
    }

}

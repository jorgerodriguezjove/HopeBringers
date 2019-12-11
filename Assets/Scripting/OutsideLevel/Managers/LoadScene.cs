using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public void OnClickButtonSceneLoad(string sceneNameToLoad)
    {
        SceneManager.LoadScene(sceneNameToLoad, LoadSceneMode.Single);
    }
}

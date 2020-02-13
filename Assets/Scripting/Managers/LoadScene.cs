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

    //Sirve para cargar el nivel de level selection pero que no desbloquee personaje si en ese nivel se desbloquea
    public void SurrenderAndGetBackToLevelSelection(string sceneNameToLoad)
    {
        GameManager.Instance.newCharacterToUnlock = null;
        SceneManager.LoadScene(sceneNameToLoad, LoadSceneMode.Single);
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}

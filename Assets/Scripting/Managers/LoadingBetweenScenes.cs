using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBetweenScenes : MonoBehaviour
{
    [HideInInspector]
    public string levelName;

    [SerializeField]
    public Image progressBar;


    // Start is called before the first frame update
    void Start()
    {
        levelName = GameManager.Instance.currentLevelToLoad;

        StartCoroutine(LoadAsyncOperation());
    }

    public IEnumerator LoadAsyncOperation()
    {
        yield return new WaitForSeconds(1f);

        AsyncOperation levelProgress = SceneManager.LoadSceneAsync(levelName);

        while (levelProgress.progress < 1)
        {
            progressBar.fillAmount = levelProgress.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}

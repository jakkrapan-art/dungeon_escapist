using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    public static LoadingSceneController instance;
    public GameObject loadingSceneObj;
    public Slider slider;
    AsyncOperation async;

    private IEnumerator coroutine;

    private void Awake()
    {
        instance = this;
    }
    public void loadingSceneTo(string nameScene)
    {
        StartCoroutine(LoadingScene(nameScene));
    }

    IEnumerator LoadingScene(string nameScene)
    {
        loadingSceneObj.SetActive(true);
        async = SceneManager.LoadSceneAsync(nameScene);
        async.allowSceneActivation = false;
        while (async.isDone == false)
        {
            slider.value = async.progress;
            if (async.progress == 0.9f)
            {
                slider.value = 1f;
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}

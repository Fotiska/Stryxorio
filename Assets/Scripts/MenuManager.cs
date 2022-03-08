using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    
    public void openURL(String URL)
    {
        Application.OpenURL(URL);
    }

    public void LoadScene(int scene)
    {
        StartCoroutine(LoadAsynchronously(scene));
    }

    public void Exit()
    {
        Application.Quit();
    }
    
    IEnumerator LoadAsynchronously(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {    
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log("Progress:" + progress);
            slider.value = progress;
            
            yield return null;
        }
    }
}

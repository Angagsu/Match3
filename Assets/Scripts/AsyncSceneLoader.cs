using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(9)]
public class AsyncSceneLoader : MonoBehaviour
{
    public static AsyncSceneLoader Instance { get; private set; }

    public  event Action OnLoadingIsStarted;
    public  event Action OnLoadingIsDone;

    private static readonly string levels = "Levels";
    private readonly string asyncLoader = "AsyncLoader";

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(asyncLoader);
        
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void Restart()
    {
        OnLoadingIsStarted?.Invoke();

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel(int sceneIndex)
    {
        OnLoadingIsStarted?.Invoke();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);

        if (loadOperation.isDone)
        {
            OnLoadingIsDone?.Invoke();
        }
    }
 
    public  void MainMenu()
    {
        OnLoadingIsStarted?.Invoke();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levels);
        if (loadOperation.isDone)
        {
            OnLoadingIsDone?.Invoke();
        }
    }
    
    public void Play()
    {
        SceneManager.LoadScene(levels);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        OnLoadingIsStarted?.Invoke();

        StartCoroutine(LoadLevelByIndex(sceneIndex));   
    }

    public IEnumerator LoadLevelByIndex(int sceneIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!loadOperation.isDone)
        {
            yield return null;
        }
        OnLoadingIsDone?.Invoke();
    }

    public int GetCurrentSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

}

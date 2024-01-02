using UnityEngine;

public class AsyncLoadSetup : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject loadingCanvas;

    private AsyncSceneLoader asyncSceneLoader;

    private void Awake()
    {
        asyncSceneLoader = AsyncSceneLoader.Instance;
    }

    private void OnEnable()
    {
        asyncSceneLoader.OnLoadingIsStarted += SceneLoader_OnLoadScene;
    }

    private void SceneLoader_OnLoadScene()
    {
        mainCanvas.SetActive(false);
        loadingCanvas.SetActive(true);
    }

    private void OnDisable()
    {
        asyncSceneLoader.OnLoadingIsStarted -= SceneLoader_OnLoadScene;
    }
}

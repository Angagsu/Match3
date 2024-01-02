using System;
using UnityEngine;


[DefaultExecutionOrder(12)]
public class SettingsUI : MonoBehaviour
{
    public event Action OnPause;
    public event Action OnUnpause;

    [SerializeField] private GameStateService gameStateService;

    [SerializeField] private RectTransform sliderTransform;
    [SerializeField] private RectTransform musicOnOffButtonTransform;

    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private GameObject musicOnOffButtonPrefab;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausedSettingsPanel;
    [SerializeField] private GameObject exitPanel;


    private bool toggle = false;
    private bool pausedSettingsToggle = false;
    private bool exitPanelToogle = false;

    private GameObject slider = null;
    private GameObject onOffButton = null;
    private AsyncSceneLoader asyncSceneLoader;

    private void Awake()
    {
        asyncSceneLoader = AsyncSceneLoader.Instance;
        InstantiateButtons();
    }
    private void Start()
    {
        settingsPanel.gameObject.SetActive(toggle);
    }
    
    private void InstantiateButtons()
    {
        slider = Instantiate(sliderPrefab, sliderTransform, false);
        onOffButton = Instantiate(musicOnOffButtonPrefab, musicOnOffButtonTransform, false);
    }

    public void HideOrShowSettingsPanel()
    {
        toggle = !toggle;
        settingsPanel.gameObject.SetActive(toggle);
        if (toggle)
        {
            OnPause?.Invoke();
        }
        else
        {
            OnUnpause?.Invoke();
        }
    }

    public void HideOrShowPausedSettingsPanel()
    {
        pausedSettingsToggle = !pausedSettingsToggle;
        pausedSettingsPanel.SetActive(pausedSettingsToggle);
    }

    public void HideOrShowExitPanel()
    {
        exitPanelToogle = !exitPanelToogle;
        exitPanel.SetActive(exitPanelToogle);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void PlayButton()
    {
        asyncSceneLoader.Play();
    }
}

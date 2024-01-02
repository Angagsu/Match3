using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateService : MonoBehaviour
{
    public event Action OnGameOver;
    public event Action OnLevelComplete;
    public event Action OnStarsCountChanged;

    public GameStates GameState { get; private set; }
    public int StarsCount { get; private set; }

    private readonly string Level_Reached_Key = "LevelReachedKey";

    [SerializeField] private ScoreTextUI scoreTextUI;
    [SerializeField] private BoardService boardService;
    [SerializeField] private SettingsUI settingsUI;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private float maxTime;

    [field: Header("Level Settings")]
    [field: Space(20)]

    [field: SerializeField] public int HighestScore { get; private set; }
    [field: SerializeField] public int MediumScore { get; private set; }
    [field: SerializeField] public int LowestScore { get; private set; }

    [field: SerializeField] public int LevelToUnlock { get; private set; }
    [SerializeField] private int currentLevel;   

    private List<int> reachedStarsCount;
    private List<string> reachedStarsCountKeys;

    private float currentTime;
    private int currentScore;


    private void Awake()
    {
        GameState = GameStates.Unpause;
        currentTime = maxTime;
        timerText.text = currentTime.ToString("0");
    }
    private void Start()
    {
        reachedStarsCount = LevelMap.Instance.GetReachedStarsCount();
        reachedStarsCountKeys = LevelMap.Instance.GetReachedStarsCountKey();
        goalText.text = HighestScore.ToString();
    }

    private void Update()
    {
        if (GameState == GameStates.GameOver || GameState == GameStates.LevelComplete)
        {
            return;
        }

        if (GameState == GameStates.Unpause)
        {
            currentTime -= Time.deltaTime;

            DisplayTime(currentTime);

            if (currentScore >= LowestScore)
            {
                StarsCount = 1;
                OnStarsCountChanged?.Invoke();

                if (currentScore >= MediumScore)
                {
                    StarsCount = 2;
                    OnStarsCountChanged?.Invoke();
                    if (currentScore >= HighestScore)
                    {
                        StarsCount = 3;
                        OnStarsCountChanged?.Invoke();
                        GameState = GameStates.LevelComplete;
                        
                        LevelComplete();
                    }
                }
            }
        }
        

        if (currentTime <= 0 && currentScore < LowestScore)
        {
            GameOver();
            GameState = GameStates.GameOver;
        }
        else if (currentTime <= 0 && currentScore > LowestScore)
        {
            Debug.Log("Level Coplete !!!");
            LevelComplete();
            GameState = GameStates.LevelComplete;
        }
    }

    private void OnEnable()
    {
        scoreTextUI.OnAddScore += ScoreTextUI_OnAddScore;
        settingsUI.OnPause += SettingsUI_OnPause;
        settingsUI.OnUnpause += SettingsUI_OnUnpause;
    }

    private void OnDisable()
    {
        scoreTextUI.OnAddScore -= ScoreTextUI_OnAddScore;
        settingsUI.OnPause -= SettingsUI_OnPause;
        settingsUI.OnUnpause -= SettingsUI_OnUnpause;
    }

    private void SettingsUI_OnUnpause()
    {
        GameState = GameStates.Unpause;
        boardService.EnableBoardService();
    }

    private void SettingsUI_OnPause()
    {
        GameState = GameStates.Pause; 
        boardService.DisableBoardService();
    }
 
    private void ScoreTextUI_OnAddScore()
    {
        currentScore = scoreTextUI.GetScore();
    }

    public void GameOver()
    {
        GameState = GameStates.GameOver;
        AdsService.Instance.interstitialAds.ShowAd();
        OnGameOver?.Invoke();
        boardService.DisableBoardService();
    }

    public void LevelComplete()
    {
        GameState = GameStates.LevelComplete;
        AdsService.Instance.interstitialAds.ShowAd();
        OnLevelComplete?.Invoke();
        boardService.DisableBoardService();
        Save();
    }

    private void Save()
    {
        if (PlayerPrefs.GetInt(Level_Reached_Key) < LevelToUnlock)
        {
            PlayerPrefs.SetInt(Level_Reached_Key, LevelToUnlock);
        }

        if (reachedStarsCount[(currentLevel - 1)] < StarsCount)
        {
            reachedStarsCount[(currentLevel - 1)] = StarsCount;
            PlayerPrefs.SetInt(reachedStarsCountKeys[(currentLevel - 1)], StarsCount);
        }
        //PlayerPrefs.Save();
    }

    public int GetTimeLeft()
    {
        return (int)currentTime;
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}


public enum GameStates
{
    LevelComplete,
    GameOver,
    Pause,
    Unpause
}

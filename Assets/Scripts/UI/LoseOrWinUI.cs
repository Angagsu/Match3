using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoseOrWinUI : MonoBehaviour
{
    [SerializeField] private GameStateService gameStateService;
    [SerializeField] private ScoreTextUI scoreTextUI;

    [SerializeField] private GameObject loseOrWinPanel;
    [SerializeField] private Button nextLevelButton;
    
    [SerializeField] private Button settingsButton;

    [Space(30f)]
    [SerializeField] private Image loseOrWinImage;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject[] stars;

    [SerializeField] private Sprite loseImage;
    [SerializeField] private Sprite winImage;

    private AsyncSceneLoader asyncSceneLoader;

    private void Start()
    {
        asyncSceneLoader = AsyncSceneLoader.Instance;
        Hide();    
    }

    private void OnEnable()
    {
        gameStateService.OnGameOver += GameStateService_OnGameOver;
        gameStateService.OnLevelComplete += GameStateService_OnLevelComplete;
    }

    private void OnDisable()
    {
        gameStateService.OnGameOver -= GameStateService_OnGameOver;
        gameStateService.OnLevelComplete -= GameStateService_OnLevelComplete;
    }
    

    private void GameStateService_OnLevelComplete()
    {
        Show();
        CalculateStarsCount();
        loseOrWinImage.sprite = winImage;
        scoreText.text = "Score " + scoreTextUI.GetScore();
        timeText.text = "Time " + gameStateService.GetTimeLeft().ToString();
    }

    private void GameStateService_OnGameOver()
    {
        Show();
        CalculateStarsCount();
        loseOrWinImage.sprite = loseImage;
        loseOrWinImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 255f);
        loseOrWinImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80f);
        scoreText.text = "Score " + scoreTextUI.GetScore();
        timeText.text = "Time " + gameStateService.GetTimeLeft().ToString();
    }

    private void CalculateStarsCount()
    {
        if (gameStateService.StarsCount == 2)
        {
            stars[2].gameObject.SetActive(false);
        }
        if (gameStateService.StarsCount == 1)
        {
            stars[1].gameObject.SetActive(false);
            stars[2].gameObject.SetActive(false);
        }
        if (gameStateService.StarsCount == 0)
        {
            stars[0].gameObject.SetActive(false);
            stars[1].gameObject.SetActive(false);
            stars[2].gameObject.SetActive(false);
        }
        
    }

    public void RestartButton()
    {
        asyncSceneLoader.Restart();
    }

    public void MainMenuButton()
    {
        asyncSceneLoader.MainMenu();
    }

    public void LoadNextLevel()
    {
        asyncSceneLoader.NextLevel(gameStateService.LevelToUnlock + 1);
    }
    private void Hide()
    {
        loseOrWinPanel.gameObject.SetActive(false);
    }
    private void Show()
    {
        settingsButton.interactable = false;

        loseOrWinPanel.gameObject.SetActive(true);

        if (gameStateService.GameState != GameStates.LevelComplete)
        {
            nextLevelButton.interactable = false;
        }
    }
}

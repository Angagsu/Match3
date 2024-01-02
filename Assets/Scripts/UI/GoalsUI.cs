using UnityEngine;
using UnityEngine.UI;

public class GoalsUI : MonoBehaviour
{
    [SerializeField] private ScoreTextUI scoreTextUI;
    [SerializeField] private GameStateService gameStateService;

    [SerializeField] private Image progressBarOneStar;
    [SerializeField] private Image progressBarTwoStar;
    [SerializeField] private Image progressBarThreeStar;

    [Space(20)]
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

    private float lowestScore;
    private float mediumScore;
    private float highestScore;
    private float score;
    private void Start()
    {
        score = scoreTextUI.GetScore();
        lowestScore = gameStateService.LowestScore;
        mediumScore = gameStateService.MediumScore;
        highestScore = gameStateService.HighestScore;

        progressBarOneStar.fillAmount = score / lowestScore;
        progressBarTwoStar.fillAmount = score / mediumScore;
        progressBarThreeStar.fillAmount = score / highestScore;
    }

    private void OnEnable()
    {
        scoreTextUI.OnAddScore += ScoreTextUI_OnAddScore;
        gameStateService.OnStarsCountChanged += GameStateService_OnStarsCountChanged;
    }

    private void GameStateService_OnStarsCountChanged()
    {
        
        if (gameStateService.StarsCount == 1)
        {
            star1.SetActive(true);
        }
        if (gameStateService.StarsCount == 2)
        {
            star2.SetActive(true);
        }
        if (gameStateService.StarsCount == 3)
        {
            star3.SetActive(true);
        }
    }

    private void ScoreTextUI_OnAddScore()
    {
        score = scoreTextUI.GetScore();
        if (gameStateService.StarsCount == 0)
        {
            progressBarOneStar.fillAmount = score / lowestScore;
        }
        if (gameStateService.StarsCount == 1)
        {
            star1.SetActive(true);
            progressBarTwoStar.fillAmount = (score - lowestScore) / (mediumScore - lowestScore);
        }
        if (gameStateService.StarsCount == 2)
        {
            star2.SetActive(true);
            progressBarThreeStar.fillAmount = (score - mediumScore) / (highestScore - mediumScore);
        }
        if (gameStateService.StarsCount == 3)
        {
            star3.SetActive(true);
        }
    }

    private void OnDisable()
    {
        scoreTextUI.OnAddScore -= ScoreTextUI_OnAddScore;
        gameStateService.OnStarsCountChanged -= GameStateService_OnStarsCountChanged;
    }
}

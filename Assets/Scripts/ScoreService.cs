using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreService : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private int score;


    private void Start()
    {
        score = 0;
        scoreText.text = "0";
    }

    public void AddScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();

        if (DOTween.IsTweening(scoreText.transform))
        {
            DOTween.Kill(scoreText.transform);
        }

        DOTween.Sequence()
            .Append(scoreText.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 03f))
            .Append(scoreText.transform.DOScale(Vector3.one, 0.3f));
    }
}

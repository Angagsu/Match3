using TMPro;
using UnityEngine;
using System;

public class ScoreTextUI : MonoBehaviour
{
    public event Action OnAddScore;

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

        OnAddScore?.Invoke();
    }

    public int GetScore()
    {
        return score;
    }
}

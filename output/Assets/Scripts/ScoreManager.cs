using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton
    private int score = 0; // Score
    public Text scoreText; // Reference to the text object

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText(); 
    }

    /// <summary>
    /// This function adds points to the score variable.
    /// Then it updates the score text.
    /// </summary>
    /// <param name="points">An integer that will be added to the score.</param>
    public void AddPoints(int points)
    {
        score += points; 
        UpdateScoreText(); 
    }

    /// <summary>
    /// This function updates the text object based on the score variable.
    /// </summary>
    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score; 
    }
}


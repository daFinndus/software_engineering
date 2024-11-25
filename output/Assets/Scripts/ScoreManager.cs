using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton-Instanz
    private int score = 0; // Punktestand
    public Text scoreText; // Referenz auf das UI-Text-Element

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Zerstört doppelte Instanzen
        }
    }

    void Start()
    {
        UpdateScoreText(); // Initialisiere den Text
    }

    public void AddPoints(int points)
    {
        score += points; // Addiere Punkte
        UpdateScoreText(); // Aktualisiere die Anzeige
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score; // Aktualisiere den UI-Text
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Lives")]
    public Image lifeIndicatorPrefab;
    public Transform livesContainer;
    public Button exitButton;

    [Header("Score")]
    public TextMeshProUGUI scoreText;

    [Header("Timers")]
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI ghostScaredTimerText;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverText;

    private int lives = 3;
    

    void Start()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitToStart);
        
        InitializeLives();
        ghostScaredTimerText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);

    }

    void ExitToStart()
    {
        SceneManager.LoadScene("StartScene");
    }

    void InitializeLives()
    {
        for (int i = 0; i < lives; i++)
        {
            Instantiate(lifeIndicatorPrefab, livesContainer);
        }
    }

    public void UpdateLives(int currentLives)
    {
        lives = currentLives;
        // Clear existing indicators
        foreach (Transform child in livesContainer)
        {
            Destroy(child.gameObject);
        }

        // Re-instantiate life indicators
        for (int i = 0; i < lives; i++)
        {
            Instantiate(lifeIndicatorPrefab, livesContainer);
        }
    }

    public void UpdateScore(int currentScore)
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }

    public void StartGhostScaredTimer(float duration)
    {
        ghostScaredTimerText.gameObject.SetActive(true);
        StartCoroutine(GhostScaredCountdown(duration));
    }

    IEnumerator GhostScaredCountdown(float duration)
    {
        float timer = duration;
        while (timer > 0)
        {
            ghostScaredTimerText.text = Mathf.CeilToInt(timer).ToString();
            yield return new WaitForSeconds(1f);
            timer--;
        }
        ghostScaredTimerText.gameObject.SetActive(false);
    }

    public void HideGhostScaredTimer()
    {
        ghostScaredTimerText.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverText.gameObject.SetActive(true);
        // Optionally, add animations or effects
    }

    public void UpdateHighScore(int highScore, string bestTime)
    {
        // Update high score and time in StartScene
        // Implement as needed
    }
}
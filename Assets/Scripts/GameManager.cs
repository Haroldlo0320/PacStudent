using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD Elements")]
    public HUDManager hudManager;
    public GameTimer gameTimer;

    [Header("Audio")]
    public AudioSource walkingMusic;
    public AudioSource scaredMusic;
    public AudioSource deadMusic;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverText;

    private int score = 0;
    private int lives = 3;
    private bool isGameOver = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResetGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ResetGame()
    {
        score = 0;
        lives = 3;
        isGameOver = false;
        hudManager.UpdateLives(lives);
        hudManager.UpdateScore(score);
        hudManager.HideGhostScaredTimer();
        gameTimer.ResetTimer();
    }

    public void AddScore(int value)
    {
        score += value;
        hudManager.UpdateScore(score);
    }

    public void PacStudentDies()
    {
        lives--;
        hudManager.UpdateLives(lives);

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn PacStudent handled in PacStudentCollision script
        }
    }

    public void EatPowerPill()
    {
        // Change all ghosts to Scared state
        foreach (GhostController ghost in FindObjectsOfType<GhostController>())
        {
            ghost.EnterScaredState();
        }

        // Change background music
        ChangeMusicToScared();

        // Start Ghost Scared Timer UI
        hudManager.StartGhostScaredTimer(10f);
    }

    public void ChangeMusicToScared()
    {
        walkingMusic.Stop();
        deadMusic.Stop();
        scaredMusic.Play();
    }

    public void ChangeMusicToWalking()
    {
        scaredMusic.Stop();
        deadMusic.Stop();
        walkingMusic.Play();
    }

    public void ChangeMusicToDead()
    {
        walkingMusic.Stop();
        scaredMusic.Stop();
        deadMusic.Play();
    }

    void GameOver()
    {
        isGameOver = true;
        hudManager.ShowGameOver();

        // Stop Timer
        gameTimer.StopTimer();

        // Play Game Over effects if any

        // Save High Score
        SaveHighScore();

        // Invoke return to StartScene after 3 seconds
        Invoke("ReturnToStartScene", 3f);
    }

    void ReturnToStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    void SaveHighScore()
    {
        int previousHighScore = PlayerPrefs.GetInt("HighScore", 0);
        int previousBestTime = PlayerPrefs.GetInt("BestTime", 0);
        int currentTime = gameTimer.GetElapsedTimeInCentiseconds();

        if (score > previousHighScore || (score == previousHighScore && currentTime < previousBestTime))
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.SetInt("BestTime", currentTime);
            PlayerPrefs.Save();
        }
    }

    public void LoadHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        int bestTime = PlayerPrefs.GetInt("BestTime", 0);
        hudManager.UpdateHighScore(highScore, FormatTime(bestTime));
    }

    string FormatTime(int centiseconds)
    {
        int minutes = centiseconds / 6000;
        int seconds = (centiseconds % 6000) / 100;
        int millis = centiseconds % 100;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, millis);
    }
}
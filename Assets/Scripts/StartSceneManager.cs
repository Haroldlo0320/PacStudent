using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public Button levelOneButton;
    public Button levelTwoButton;
    public Text highScoreText;
    public Text timeText;
    public AudioSource backgroundMusic;

    void Start()
    {
        // Assign button listeners
        if (levelOneButton != null)
            levelOneButton.onClick.AddListener(LoadLevel1);

        if (levelTwoButton != null)
            levelTwoButton.onClick.AddListener(LoadLevel2);

        // Initialize high score and time
        highScoreText.text = "High Score: 0";
        timeText.text = "Time: 00:00:00";

        // Play background music
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    void LoadLevel1()
    {
        SceneManager.LoadScene("LevelOne");
    }

    void LoadLevel2()
    {
        SceneManager.LoadScene("LevelTwo");
    }
}
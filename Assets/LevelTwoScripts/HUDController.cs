using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Button exitButton;

    void Start()
    {
        // Assign button listeners
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitToStart);

    }

    void ExitToStart()
    {
        SceneManager.LoadScene("StartScene");
    }
}
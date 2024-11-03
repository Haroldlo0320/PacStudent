using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public GameObject player;
    public GhostController[] ghosts;
    public GameManager gameManager;

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        string[] countdown = { "3", "2", "1", "GO!" };

        // Disable player and ghosts movement
        player.GetComponent<PacStudentController>().enabled = false;
        foreach (GhostController ghost in ghosts)
        {
            ghost.enabled = false;
        }

        foreach (string count in countdown)
        {
            countdownText.text = count;
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);

        // Enable player and ghosts movement
        player.GetComponent<PacStudentController>().enabled = true;
        foreach (GhostController ghost in ghosts)
        {
            ghost.enabled = true;
        }

        // Start game timer
        gameManager.gameTimer.StartTimer();

        // Start walking background music
        gameManager.ChangeMusicToWalking();
    }
}
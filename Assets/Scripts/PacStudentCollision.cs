using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PacStudentCollision : MonoBehaviour
{
    // References (assign via Inspector)
    [Header("Collision Effects")]
    public GameObject wallCollisionParticle; // Particle effect prefab
    public AudioClip wallCollisionSound;      // Wall collision sound
    public GameObject deathParticle;          // PacStudent death particle effect
    public AudioClip deathSound;              // Death sound effect

    [Header("Teleporters")]
    public Transform teleporterLeftExit;       // Assign TeleporterRight's position
    public Transform teleporterRightExit;      // Assign TeleporterLeft's position

    // Movement Tracking
    private Vector3 previousPosition;

    // Components
    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        previousPosition = transform.position;
    }

    void Update()
    {
        // Store previous position before movement
        previousPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Reset position to prevent passing through
            transform.position = previousPosition;

            // Instantiate wall collision particle effect at collision point
            ContactPoint2D contact = collision.contacts[0];
            Instantiate(wallCollisionParticle, contact.point, Quaternion.identity);

            // Play wall collision sound
            audioSource.PlayOneShot(wallCollisionSound);
        }

        if (collision.gameObject.CompareTag("Ghost"))
        {
            GhostController ghost = collision.gameObject.GetComponent<GhostController>();
            if (ghost != null && ghost.CurrentState == GhostController.GhostState.Walking)
            {
                // Handle PacStudent death
                GameManager.Instance.PacStudentDies();

                // Instantiate death particle effect
                Instantiate(deathParticle, transform.position, Quaternion.identity);

                // Play death sound
                audioSource.PlayOneShot(deathSound);

                // Respawn PacStudent
                StartCoroutine(Respawn());
            }
            else if (ghost != null && (ghost.CurrentState == GhostController.GhostState.Scared || ghost.CurrentState == GhostController.GhostState.Recovering))
            {
                // Ghost enters Dead state
                ghost.Die();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.AddScore(10);
        }
        else if (other.CompareTag("PowerPill"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.EatPowerPill();
        }
        else if (other.CompareTag("Cherry"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.AddScore(100);
        }
        else if (other.CompareTag("Teleporter"))
        {
            Teleport(other.gameObject);
        }
    }

    private void Teleport(GameObject teleporter)
    {
        if (teleporter.name.Contains("Left"))
        {
            transform.position = teleporterLeftExit.position;
        }
        else if (teleporter.name.Contains("Right"))
        {
            transform.position = teleporterRightExit.position;
        }

        // Optionally, adjust movement direction here
    }

    private IEnumerator Respawn()
    {
        // Disable PacStudent controls
        GetComponent<PlayerController>().enabled = false;

        // Wait for player input (e.g., press a key to continue)
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        // Reset position to start
        transform.position = new Vector3(-9f, 0f, 0f); // Adjust based on your start position

        // Enable PacStudent controls
        GetComponent<PlayerController>().enabled = true;
    }
}
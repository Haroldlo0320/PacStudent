using System.Collections; // Necessary for IEnumerator
using UnityEngine;
using UnityEngine.Tilemaps;
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

    [Header("Teleport Cooldown")]
    public float teleportCooldown = 0.5f;     // Cooldown duration in seconds

    // Cooldown Flag
    private bool canTeleport = true;

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
        // Get the Tilemap component
        Tilemap wallTilemap = GameObject.Find("Wall").GetComponent<Tilemap>();

        // Get the tile at the collision point
        Vector3Int tilePosition = wallTilemap.WorldToCell(collision.contacts[0].point);
        TileBase tile = wallTilemap.GetTile(tilePosition);

        // Check if the tile is a wall tile
        if (tile != null)
        {
            UnityEngine.Debug.Log("Collided with Wall");
            // Reset position to prevent passing through
            transform.position = previousPosition;

            // Instantiate wall collision particle effect at collision point
            if (wallCollisionParticle != null)
            {
                Instantiate(wallCollisionParticle, collision.contacts[0].point, Quaternion.identity);
            }

            // Play wall collision sound
            if (wallCollisionSound != null)
            {
                audioSource.PlayOneShot(wallCollisionSound);
            }
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            UnityEngine.Debug.Log("Collided with Wall");
            // Reset position to prevent passing through
            transform.position = previousPosition;

            // Instantiate wall collision particle effect at collision point
            ContactPoint2D contact = collision.contacts[0];
            if (wallCollisionParticle != null)
            {
                Instantiate(wallCollisionParticle, contact.point, Quaternion.identity);
            }

            // Play wall collision sound
            if (wallCollisionSound != null)
            {
                audioSource.PlayOneShot(wallCollisionSound);
            }
        }

        if (collision.gameObject.CompareTag("Ghost"))
        {
            UnityEngine.Debug.Log("Collided with Ghost");
            GhostController ghost = collision.gameObject.GetComponent<GhostController>();
            if (ghost != null && ghost.CurrentState == GhostController.GhostState.Walking)
            {
                // Handle PacStudent death
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PacStudentDies();
                }

                // Instantiate death particle effect
                if (deathParticle != null)
                {
                    Instantiate(deathParticle, transform.position, Quaternion.identity);
                }

                // Play death sound
                if (deathSound != null)
                {
                    audioSource.PlayOneShot(deathSound);
                }

                // Respawn PacStudent
                StartCoroutine(Respawn());
            }
            else if (ghost != null && (ghost.CurrentState == GhostController.GhostState.Scared || ghost.CurrentState == GhostController.GhostState.Recovering))
            {
                UnityEngine.Debug.Log("Ghost is scared or recovering, ghost will die.");
                // Ghost enters Dead state
                ghost.Die();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UnityEngine.Debug.Log("Triggered with " + other.gameObject.tag);

        if (other.CompareTag("Pellet"))
        {
            UnityEngine.Debug.Log("Collected Pellet");
            Destroy(other.gameObject);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10);
            }
        }
        else if (other.CompareTag("PowerPill"))
        {
            UnityEngine.Debug.Log("Collected Power Pill");
            Destroy(other.gameObject);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EatPowerPill();
            }
        }
        else if (other.CompareTag("Cherry"))
        {
            UnityEngine.Debug.Log("Collected Cherry");
            Destroy(other.gameObject);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(100);
            }
        }
        
        else if (other.CompareTag("Teleporter"))
        {
            UnityEngine.Debug.Log("Entered Teleporter");
            Teleport(other.gameObject);
        }
    }

    private void Teleport(GameObject teleporter)
    {
        if (!canTeleport)
        {
            UnityEngine.Debug.Log("Teleport on cooldown. Teleportation skipped.");
            return;
        }

        // Begin Teleportation
        canTeleport = false;

        if (teleporter.name.Contains("Left"))
        {
            UnityEngine.Debug.Log("Teleporting to TeleporterRightExit");
            transform.position = teleporterRightExit.position;
        }
        else if (teleporter.name.Contains("Right"))
        {
            UnityEngine.Debug.Log("Teleporting to TeleporterLeftExit");
            transform.position = teleporterLeftExit.position;
        }

        // Optionally, adjust movement direction here

        // Start Cooldown
        StartCoroutine(TeleportCooldown());
    }

    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }

    private IEnumerator Respawn()
    {
        // Disable PacStudent controls
        PacStudentController PacStudentController = GetComponent<PacStudentController>();
        if (PacStudentController != null)
        {
            PacStudentController.enabled = false;
        }

        // Wait for player input (e.g., press any key to continue)
        UnityEngine.Debug.Log("Press any key to respawn...");
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        // Reset position to start (top-left corner)
        transform.position = new Vector3(-9f, 0f, 0f); // Adjust based on your start position

        // Enable PacStudent controls
        if (PacStudentController != null)
        {
            PacStudentController.enabled = true;
        }
    }
}
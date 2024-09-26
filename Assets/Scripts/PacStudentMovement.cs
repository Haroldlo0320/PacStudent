using UnityEngine;
using System.Collections;

public class PacStudentMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioClip movingAudio;

    private Vector3[] waypoints;
    private int currentWaypointIndex = 0;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        // Define the waypoints for the clockwise movement around the top-left inner block
        waypoints = new Vector3[]
        {
            new Vector3(-2, 2, 0),  // Top-left corner
            new Vector3(2, 2, 0),   // Top-right corner
            new Vector3(2, -2, 0),  // Bottom-right corner
            new Vector3(-2, -2, 0)  // Bottom-left corner
        };

        // Set the initial position to the first waypoint
        transform.position = waypoints[0];

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = movingAudio;
        audioSource.loop = true;

        // Get the Animator component
        animator = GetComponent<Animator>();

        // Start the movement
        StartCoroutine(MoveToNextWaypoint());
    }

    IEnumerator MoveToNextWaypoint()
    {
        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = waypoints[currentWaypointIndex];

            // Play the movement animation
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }

            // Play the moving audio
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            float journeyLength = Vector3.Distance(startPosition, endPosition);
            float startTime = Time.time;

            while (transform.position != endPosition)
            {
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;

                transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

                // Rotate PacStudent to face the movement direction
                Vector3 direction = (endPosition - startPosition).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjust the angle based on your sprite's default orientation

                yield return null;
            }

            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void OnDisable()
    {
        // Stop the audio and animation when the script is disabled
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
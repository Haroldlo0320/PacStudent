using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    // Reference to the Dust Particle System
    [Header("Dust Particle Settings")]
    public ParticleSystem dustParticles;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set the collision detection mode to Continuous
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Set the layer mask to the correct layer for the walls
        int wallLayer = LayerMask.NameToLayer("Wall");
        Debug.Log($"Wall Layer: {wallLayer}");

        if (dustParticles != null)
        {
            dustParticles.Stop();
        }
        else
        {
            Debug.LogWarning("Dust Particle System is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Calculate desired position
        Vector2 desiredPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        // Check for collisions before moving
        if (!WillCollide(desiredPosition))
        {
            rb.MovePosition(desiredPosition);
            HandleDustParticles(true); // Player is moving
        }
        else
        {
            HandleDustParticles(false); // Movement blocked by collision
        }
    }

    private bool WillCollide(Vector2 desiredPosition)
    {
        // Cast a box to check if we would collide with anything
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            transform.position,
            boxCollider.size,
            0f,
            (desiredPosition - rb.position).normalized,
            movement.magnitude * moveSpeed * Time.fixedDeltaTime,
            LayerMask.GetMask("Wall")
        );

        foreach (RaycastHit2D hit in hits)
        {
            // Ignore self-collision
            if (hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }
    private void HandleDustParticles(bool isMoving)
    {
        if (dustParticles == null)
        {
            return;
        }

        if (isMoving)
        {
            if (!dustParticles.isPlaying)
            {
                dustParticles.Play();
            }
        }
        else
        {
            if (dustParticles.isPlaying)
            {
                dustParticles.Stop();
            }
        }
    }
}
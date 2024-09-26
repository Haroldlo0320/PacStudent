using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private bool isDead = false;
    private Vector3 movement;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", moveHorizontal);
        animator.SetFloat("Vertical", moveVertical);

        movement = new Vector3(moveHorizontal, moveVertical, 0f).normalized;

        // Move the character
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Set facing direction based on movement
        if (movement != Vector3.zero)
        {
            if (moveHorizontal != 0)
            {
                // Horizontal movement
                spriteRenderer.flipX = moveHorizontal < 0;
            }
            else if (moveVertical != 0)
            {
                // Vertical movement
                transform.rotation = Quaternion.Euler(0, 0, moveVertical > 0 ? 90 : -90);
            }
        }
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        // Add any other death logic here
    }
}
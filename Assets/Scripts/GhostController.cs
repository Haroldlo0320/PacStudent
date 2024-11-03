using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum GhostState { Walking, Scared, Recovering, Dead }

    [Header("State Durations")]
    public float scaredDuration = 10f;
    public float recoveringDuration = 3f;
    public float deadDuration = 5f;

    // Components
    private Animator animator;
    private AudioSource audioSource;

    // State Management
    private float timer;
    public GhostState CurrentState { get; private set; } = GhostState.Walking;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (CurrentState)
        {
            case GhostState.Scared:
                if (timer >= scaredDuration - recoveringDuration)
                {
                    TransitionToRecovering();
                }
                if (timer >= scaredDuration)
                {
                    TransitionToWalking();
                }
                break;

            case GhostState.Recovering:
                if (timer >= recoveringDuration)
                {
                    TransitionToWalking();
                }
                break;

            case GhostState.Dead:
                if (timer >= deadDuration)
                {
                    TransitionToWalking();
                }
                break;
        }
    }

    public void EnterScaredState()
    {
        animator.SetBool("Scared", true);
        animator.SetBool("Recovering", false);
        animator.SetBool("IsDead", false);
        CurrentState = GhostState.Scared;
        timer = 0f;

        // Change background music if needed
        GameManager.Instance.ChangeMusicToScared();
    }

    private void TransitionToRecovering()
    {
        animator.SetBool("Scared", false);
        animator.SetBool("Recovering", true);
        CurrentState = GhostState.Recovering;
        timer = 0f;
    }

    private void TransitionToWalking()
    {
        animator.SetBool("Scared", false);
        animator.SetBool("Recovering", false);
        animator.SetBool("IsDead", false);
        CurrentState = GhostState.Walking;
        timer = 0f;

        // Revert background music
        GameManager.Instance.ChangeMusicToWalking();
    }

    public void Die()
    {
        animator.SetBool("IsDead", true);
        CurrentState = GhostState.Dead;
        timer = 0f;

        // Add score
        GameManager.Instance.AddScore(300);

        // Change music if needed
        GameManager.Instance.ChangeMusicToDead();
    }
}
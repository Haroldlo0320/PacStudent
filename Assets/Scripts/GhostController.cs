using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float scaredDuration = 3f;
    public float recoveringDuration = 2f;
    public float deadDuration = 2f;  // Add this to control how long the dead state lasts

    private Animator animator;
    private float timer;
    private enum GhostState { Scared, Recovering, Dead }
    private GhostState currentState = GhostState.Scared;

    void Start()
    {
        animator = GetComponent<Animator>();
        ResetState();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case GhostState.Scared:
                if (timer >= scaredDuration)
                {
                    TransitionToRecovering();
                }
                break;

            case GhostState.Recovering:
                if (timer >= recoveringDuration)
                {
                    TransitionToDead();
                }
                break;

            case GhostState.Dead:
                if (timer >= deadDuration)
                {
                    ResetState();
                }
                break;
        }
    }

    void TransitionToRecovering()
    {
        animator.SetBool("Scared", false);
        animator.SetBool("Recovering", true);
        currentState = GhostState.Recovering;
        timer = 0f;
    }

    void TransitionToDead()
    {
        animator.SetBool("Recovering", false);
        animator.SetBool("IsDead", true);
        currentState = GhostState.Dead;
        timer = 0f;
    }

    void ResetState()
    {
        animator.SetBool("Scared", true);
        animator.SetBool("Recovering", false);
        animator.SetBool("IsDead", false);
        currentState = GhostState.Scared;
        timer = 0f;
    }
}
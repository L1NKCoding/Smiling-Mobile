using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    void Awake()
    {
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Start in animated mode
        SetRagdollState(false);
    }

    public void SetRagdollState(bool enableRagdoll)
    {
        animator.enabled = !enableRagdoll;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !enableRagdoll;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = enableRagdoll;
        }
    }

    // Call this on death, knockback, etc.
    public void Die()
    {
        SetRagdollState(true);
    }
}
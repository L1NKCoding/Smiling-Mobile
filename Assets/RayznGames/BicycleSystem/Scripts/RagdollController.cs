using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Tooltip("Assign the hips/pelvis bone here so the camera tracks the ragdoll body")]
    [SerializeField] Transform hipBone;

    // Returns the best transform to follow during ragdoll
    public Transform CameraTarget => hipBone != null ? hipBone : transform;

    [Header("Ragdoll Control")]
    [SerializeField] float hipTorque = 200f;
    [SerializeField] float thrustForce = 30f;
    [SerializeField] float spaceBoostMultiplier = 1f;

    private Rigidbody hipRigidbody;
    private bool isRagdoll = false;

    private Animator animator;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private Transform[] ragdollTransforms;
    private Vector3[] boneLocalPositions;
    private Quaternion[] boneLocalRotations;

    void Awake()
    {
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Store initial local pose of every physics bone for respawn
        ragdollTransforms = new Transform[ragdollBodies.Length];
        boneLocalPositions = new Vector3[ragdollBodies.Length];
        boneLocalRotations = new Quaternion[ragdollBodies.Length];
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            ragdollTransforms[i] = ragdollBodies[i].transform;
            boneLocalPositions[i] = ragdollBodies[i].transform.localPosition;
            boneLocalRotations[i] = ragdollBodies[i].transform.localRotation;
        }

        // Start in animated mode
        SetRagdollState(false);

        if (hipBone != null)
            hipRigidbody = hipBone.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isRagdoll || hipRigidbody == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Lean forward/back and twist side to side
        hipRigidbody.AddTorque(transform.right  *  v * hipTorque);
        hipRigidbody.AddTorque(transform.forward * -h * hipTorque);

        // Brief jump impulse (fires once per key press)
        if (Input.GetKeyDown(KeyCode.Space))
            hipRigidbody.AddForce(Vector3.up * thrustForce * spaceBoostMultiplier, ForceMode.Impulse);
    }

    public void SetRagdollState(bool enableRagdoll)
    {
        isRagdoll = enableRagdoll;
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

    public void Revive()
    {
        // Snap every bone back to its original local pose before re-enabling the animator
        for (int i = 0; i < ragdollTransforms.Length; i++)
        {
            ragdollTransforms[i].localPosition = boneLocalPositions[i];
            ragdollTransforms[i].localRotation = boneLocalRotations[i];
        }

        SetRagdollState(false);
    }
}
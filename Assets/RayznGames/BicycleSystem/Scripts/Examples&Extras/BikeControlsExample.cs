using rayzngames;
using UnityEngine;

namespace rayzngames
{
    public class BikeControlsExample : MonoBehaviour
    {
        BicycleVehicle bicycle;
        public bool controllingBike;

        [Header("Crash / Ragdoll")]
        [SerializeField] RagdollController ragdollController;
        [SerializeField] BikeIKTargets bikeIKTargets;
        [SerializeField] CameraController cameraController;
        [Tooltip("Side tilt (Z) or forward tilt (X) in degrees that triggers a crash")]
        [SerializeField] float crashAngleThreshold = 55f;
        [Tooltip("Seconds the angle must stay over threshold before crashing (prevents ramp false positives)")]
        [SerializeField] float crashAngleDuration = 0.35f;
        [Tooltip("How hard a landing must be (m/s downward) to trigger a crash")]
        [SerializeField] float crashImpactSpeed = 8f;
        [Tooltip("Press to manually enter ragdoll")]
        [SerializeField] KeyCode ragdollKey = KeyCode.G;
        [Tooltip("Press to respawn after crashing")]
        [SerializeField] KeyCode respawnKey = KeyCode.R;

        private bool hasCrashed = false;
        private float airborneTimer = 0f;
        private float crashAngleTimer = 0f;

        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Vector3 ragdollLocalPosition;
        private Quaternion ragdollLocalRotation;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            bicycle = GetComponent<BicycleVehicle>();

            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            if (ragdollController != null)
            {
                ragdollLocalPosition = ragdollController.transform.localPosition;
                ragdollLocalRotation = ragdollController.transform.localRotation;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(respawnKey) && hasCrashed)
            {
                Respawn();
                return;
            }

            if (hasCrashed) return;

            if (Input.GetKeyDown(ragdollKey))
            {
                TriggerCrash();
                return;
            }

            bicycle.verticalInput = Input.GetAxis("Vertical");
            bicycle.horizontalInput = Input.GetAxis("Horizontal");
            BrakingInput();

            bicycle.jumpInput = Input.GetKeyDown(KeyCode.Space);

            //Extending functionality
            bicycle.InControl(controllingBike);

            if (controllingBike)
            {
                //Constrains the Z rotation of the bike, when onground, and releases it when airborne.
                bicycle.ConstrainRotation(bicycle.OnGround());

                // Track airborne time
                if (!bicycle.OnGround())
                    airborneTimer += Time.deltaTime;
                else
                    airborneTimer = 0f;

                // Crash: bike physically tipped too far on Z (side) or X (forward/back)
                float zAngle = Mathf.Abs(Mathf.DeltaAngle(0f, transform.eulerAngles.z));
                float xAngle = Mathf.Abs(Mathf.DeltaAngle(0f, transform.eulerAngles.x));
                if (zAngle > crashAngleThreshold || xAngle > crashAngleThreshold)
                {
                    crashAngleTimer += Time.deltaTime;
                    if (crashAngleTimer >= crashAngleDuration)
                        TriggerCrash();
                }
                else
                {
                    crashAngleTimer = 0f;
                }
            }
            else
            {
                bicycle.ConstrainRotation(false);
            }
        }

        void TriggerCrash()
        {
            hasCrashed = true;
            controllingBike = false;
            bicycle.InControl(false);
            bicycle.ConstrainRotation(false);

            if (bikeIKTargets != null)
                bikeIKTargets.enabled = false;

            if (ragdollController != null)
            {
                ragdollController.transform.SetParent(null);
                ragdollController.Die();

                if (cameraController != null)
                    cameraController.SetTarget(ragdollController.CameraTarget);
            }
        }

        void Respawn()
        {
            // Reset bike transform and velocity
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            Rigidbody bikeRb = GetComponent<Rigidbody>();
            if (bikeRb != null)
            {
                bikeRb.linearVelocity = Vector3.zero;
                bikeRb.angularVelocity = Vector3.zero;
            }

            // Restore ragdoll to animated state and re-attach to bike
            if (ragdollController != null)
            {
                ragdollController.Revive();
                ragdollController.transform.SetParent(transform);
                ragdollController.transform.localPosition = ragdollLocalPosition;
                ragdollController.transform.localRotation = ragdollLocalRotation;
            }

            if (bikeIKTargets != null)
                bikeIKTargets.enabled = true;

            if (cameraController != null)
                cameraController.SetTarget(transform);

            hasCrashed = false;
            controllingBike = true;
            airborneTimer = 0f;
            crashAngleTimer = 0f;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (hasCrashed || !controllingBike) return;

            // Trigger crash on hard landing or high-speed impact
            float impactSpeed = Mathf.Abs(collision.relativeVelocity.y);
            if (impactSpeed > crashImpactSpeed && airborneTimer > 0.3f)
            {
                TriggerCrash();
            }
        }

        void BrakingInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                bicycle.braking = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                bicycle.braking = false;
            }
        }
    }
}

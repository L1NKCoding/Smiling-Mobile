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
        [Tooltip("How hard a landing must be (m/s downward) to trigger a crash")]
        [SerializeField] float crashImpactSpeed = 8f;

        private bool hasCrashed = false;
        private float airborneTimer = 0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            bicycle = GetComponent<BicycleVehicle>();
        }
        // Update is called once per frame
        void Update()
        {
            if (hasCrashed) return;

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
                    TriggerCrash();
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
                    cameraController.SetTarget(ragdollController.transform);
            }
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

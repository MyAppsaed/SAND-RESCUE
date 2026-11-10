using UnityEngine;

namespace SandRescue
{
    [RequireComponent(typeof(Rigidbody))]
    public class SandRescueVehicleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody body;
        [SerializeField] private VehicleInputManager input;
        [SerializeField] private SurfaceDetector surfaceDetector;
        [SerializeField] private VehicleRecovery recovery;
        [SerializeField] private VehicleWheel[] wheels;
        [SerializeField] private Transform centerOfMass;

        [Header("Performance")]
        [SerializeField] private float maximumForwardSpeedKph = 80f;
        [SerializeField] private float maximumReverseSpeedKph = 25f;
        [SerializeField] private float motorTorque = 1900f;
        [SerializeField] private float brakeTorque = 3200f;
        [SerializeField] private float handbrakeTorque = 5200f;
        [SerializeField] private float downforceAtTopSpeed = 90f;

        [Header("Steering")]
        [SerializeField] private float lowSpeedSteeringAngle = 32f;
        [SerializeField] private float highSpeedSteeringAngle = 12f;

        public float SpeedKph { get; private set; }
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            if (body == null) body = GetComponent<Rigidbody>();
            if (surfaceDetector == null) surfaceDetector = GetComponent<SurfaceDetector>();
            if (recovery == null) recovery = GetComponent<VehicleRecovery>();
            if (centerOfMass != null) body.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
            body.mass = 1800f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        private void Update()
        {
            SpeedKph = body.linearVelocity.magnitude * 3.6f;
            IsGrounded = false;
            for (int i = 0; i < wheels.Length; i++) if (wheels[i] != null && wheels[i].Grounded) { IsGrounded = true; break; }
            if (input != null && input.Current.RecoverPressed && recovery != null) recovery.Recover();
        }

        private void FixedUpdate()
        {
            if (body == null || input == null || wheels == null || wheels.Length == 0) return;
            VehicleInputState state = input.Current;
            float localForwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward) * 3.6f;
            float speed01 = Mathf.InverseLerp(0f, maximumForwardSpeedKph, Mathf.Abs(localForwardSpeed));
            float steering = Mathf.Lerp(lowSpeedSteeringAngle, highSpeedSteeringAngle, speed01) * state.Steering;
            var profile = surfaceDetector != null && surfaceDetector.Current != null ? surfaceDetector.Current : null;
            float gripForward = profile != null ? profile.forwardGrip : 1f;
            float gripSideways = profile != null ? profile.sidewaysGrip : 1f;
            float surfaceSpeed = profile != null ? profile.speedMultiplier : 1f;
            float motor = 0f;
            if (state.Throttle > 0f && localForwardSpeed < maximumForwardSpeedKph * surfaceSpeed) motor = state.Throttle * motorTorque;
            if (state.Throttle < 0f && localForwardSpeed > -maximumReverseSpeedKph) motor = state.Throttle * motorTorque * 0.65f;
            float brake = state.Handbrake ? handbrakeTorque : 0f;
            if (Mathf.Abs(state.Throttle) < 0.05f && Mathf.Abs(localForwardSpeed) > 2f) brake = brakeTorque * 0.12f;
            for (int i = 0; i < wheels.Length; i++) if (wheels[i] != null) wheels[i].Apply(steering, motor, brake, gripSideways, gripForward);
            float topSpeed01 = Mathf.Clamp01(SpeedKph / Mathf.Max(1f, maximumForwardSpeedKph));
            body.AddForce(-transform.up * (downforceAtTopSpeed * topSpeed01), ForceMode.Force);
        }
    }
}

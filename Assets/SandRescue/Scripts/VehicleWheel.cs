using UnityEngine;

namespace SandRescue
{
    [RequireComponent(typeof(WheelCollider))]
    public class VehicleWheel : MonoBehaviour
    {
        [SerializeField] private Transform visualWheel;
        [SerializeField] private bool steeringWheel;
        [SerializeField] private bool drivenWheel = true;
        [SerializeField] private bool brakeWheel = true;
        [SerializeField] private float suspensionDistance = 0.25f;
        [SerializeField] private float spring = 35000f;
        [SerializeField] private float damper = 4500f;

        public WheelCollider Collider { get; private set; }
        public bool IsSteering => steeringWheel;
        public bool IsDriven => drivenWheel;
        public bool IsBrakeWheel => brakeWheel;
        public bool Grounded => Collider != null && Collider.isGrounded;
        public float Slip { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<WheelCollider>();
            Collider.suspensionDistance = suspensionDistance;
            var s = Collider.suspensionSpring; s.spring = spring; s.damper = damper; Collider.suspensionSpring = s;
        }

        public void Apply(float steer, float motorTorque, float brakeTorque, float sidewaysGrip, float forwardGrip)
        {
            Collider.steerAngle = steeringWheel ? steer : 0f;
            Collider.motorTorque = drivenWheel ? motorTorque : 0f;
            Collider.brakeTorque = brakeWheel ? brakeTorque : 0f;
            var forward = Collider.forwardFriction; forward.stiffness = forwardGrip; Collider.forwardFriction = forward;
            var sideways = Collider.sidewaysFriction; sideways.stiffness = sidewaysGrip; Collider.sidewaysFriction = sideways;
            if (Collider.GetGroundHit(out WheelHit hit)) Slip = Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip); else Slip = 0f;
        }

        private void LateUpdate()
        {
            if (visualWheel == null || Collider == null) return;
            Collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            visualWheel.SetPositionAndRotation(position, rotation);
        }
    }
}

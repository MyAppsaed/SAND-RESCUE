using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SandRescue
{
    public class VehicleCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SandRescueVehicleController vehicle;
        [SerializeField] private Camera cameraComponent;
        [SerializeField] private CameraCollisionHandler collisionHandler;
        [SerializeField] private float distance = 7f;
        [SerializeField] private float height = 3.2f;
        [SerializeField] private float followSmooth = 8f;
        [SerializeField] private float rotationSmooth = 7f;
        [SerializeField] private float normalFov = 60f;
        [SerializeField] private float maximumFov = 72f;
        [SerializeField] private float touchRotationSpeed = 0.15f;
        [SerializeField, Range(0f, 120f)] private float maximumTouchYaw = 60f;
        [SerializeField] private float lookBackDuration = 1.4f;
        private float yawOffset;
        private float lookBackTimer;
        private VehicleInputManager input;

        private void Awake()
        {
            if (vehicle != null) input = vehicle.GetComponent<VehicleInputManager>();
            if (cameraComponent == null) cameraComponent = GetComponent<Camera>();
            if (collisionHandler == null) collisionHandler = GetComponent<CameraCollisionHandler>();
        }

        private void Update()
        {
            if (target == null) return;
            bool dragging = false;
#if ENABLE_INPUT_SYSTEM
            Touchscreen screen = Touchscreen.current;
            if (screen != null && screen.primaryTouch.press.isPressed)
            {
                Vector2 delta = screen.primaryTouch.delta.ReadValue();
                dragging = delta.sqrMagnitude > 0.01f;
                if (dragging)
                    yawOffset = Mathf.Clamp(yawOffset + delta.x * touchRotationSpeed,
                        -maximumTouchYaw, maximumTouchYaw);
            }
#endif
            if (input != null && input.Current.LookBack) lookBackTimer = lookBackDuration;
            lookBackTimer = Mathf.Max(0f, lookBackTimer - Time.deltaTime);
            if (!dragging && lookBackTimer <= 0f)
                yawOffset = Mathf.Lerp(yawOffset, 0f, Time.deltaTime * 2f);
        }

        private void LateUpdate()
        {
            if (target == null) return;
            float yaw = lookBackTimer > 0f ? 180f : yawOffset;
            Quaternion orbit = Quaternion.Euler(0f, target.eulerAngles.y + yaw, 0f);
            Vector3 desired = target.position + orbit * new Vector3(0f, height, -distance);
            if (collisionHandler != null)
                desired = collisionHandler.Resolve(desired, target.position + Vector3.up * 1.2f);
            transform.position = Vector3.Lerp(transform.position, desired,
                1f - Mathf.Exp(-followSmooth * Time.deltaTime));
            Quaternion look = Quaternion.LookRotation(target.position + Vector3.up * 1.1f - transform.position,
                Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look,
                1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
            if (cameraComponent != null && vehicle != null)
                cameraComponent.fieldOfView = Mathf.Lerp(normalFov, maximumFov,
                    Mathf.Clamp01(vehicle.SpeedKph / 80f));
        }
    }
}

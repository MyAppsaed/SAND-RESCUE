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
        [SerializeField] private float freeLookSensitivity = 0.16f;
        [SerializeField] private float freeLookDistance = 8f;
        [SerializeField] private float minimumFreeLookDistance = 3f;
        [SerializeField] private float maximumFreeLookDistance = 18f;
        private float yawOffset;
        private float lookBackTimer;
        private float freeYaw;
        private float freePitch = 18f;
        private bool freeLook;
        private VehicleInputManager input;

        private void Awake()
        {
            if (vehicle != null) input = vehicle.GetComponent<VehicleInputManager>();
            if (cameraComponent == null) cameraComponent = GetComponent<Camera>();
            if (collisionHandler == null) collisionHandler = GetComponent<CameraCollisionHandler>();
            if (target != null) freeYaw = target.eulerAngles.y;
        }

        private void Update()
        {
            if (target == null) return;
            bool dragging = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                freeLook = !freeLook;
                if (freeLook)
                {
                    freeYaw = target.eulerAngles.y + yawOffset;
                    freePitch = 18f;
                }
            }
            Mouse mouse = Mouse.current;
            if (freeLook && mouse != null && mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                freeYaw += delta.x * freeLookSensitivity;
                freePitch = Mathf.Clamp(freePitch - delta.y * freeLookSensitivity, -10f, 70f);
                freeLookDistance = Mathf.Clamp(freeLookDistance - mouse.scroll.ReadValue().y * 0.01f,
                    minimumFreeLookDistance, maximumFreeLookDistance);
                dragging = delta.sqrMagnitude > 0.01f;
            }
            Touchscreen screen = Touchscreen.current;
            if (!freeLook && screen != null && screen.primaryTouch.press.isPressed)
            {
                Vector2 delta = screen.primaryTouch.delta.ReadValue();
                dragging = delta.sqrMagnitude > 0.01f;
                if (dragging)
                    yawOffset = Mathf.Clamp(yawOffset + delta.x * touchRotationSpeed,
                        -maximumTouchYaw, maximumTouchYaw);
            }
#endif
            if (!freeLook && input != null && input.Current.LookBack) lookBackTimer = lookBackDuration;
            lookBackTimer = Mathf.Max(0f, lookBackTimer - Time.deltaTime);
            if (!freeLook && !dragging && lookBackTimer <= 0f)
                yawOffset = Mathf.Lerp(yawOffset, 0f, Time.deltaTime * 2f);
        }

        private void LateUpdate()
        {
            if (target == null) return;
            if (freeLook)
            {
                Quaternion freeOrbit = Quaternion.Euler(freePitch, freeYaw, 0f);
                Vector3 freeDesired = target.position + freeOrbit * new Vector3(0f, 0f, -freeLookDistance);
                if (collisionHandler != null)
                    freeDesired = collisionHandler.Resolve(freeDesired, target.position + Vector3.up * 1.2f);
                transform.position = Vector3.Lerp(transform.position, freeDesired,
                    1f - Mathf.Exp(-followSmooth * Time.deltaTime));
                Quaternion freeLookRotation = Quaternion.LookRotation(target.position + Vector3.up * 1.1f - transform.position,
                    Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, freeLookRotation,
                    1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
                if (cameraComponent != null) cameraComponent.fieldOfView = normalFov;
                return;
            }
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

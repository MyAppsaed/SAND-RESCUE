using UnityEngine;

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
        [SerializeField] private float lookBackDuration = 1.4f;
        private float yawOffset;
        private float lookBackTimer;
        private Vector2 lastTouch;

        private void Update()
        {
            if (target == null) return;
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
                yawOffset += Input.touches[0].deltaPosition.x * touchRotationSpeed;
            if (vehicle != null && vehicle.GetComponent<VehicleInputManager>()?.Current.LookBack == true) lookBackTimer = lookBackDuration;
            lookBackTimer -= Time.deltaTime;
            if (lookBackTimer <= 0f) yawOffset = Mathf.Lerp(yawOffset, 0f, Time.deltaTime * 2f);
        }

        private void LateUpdate()
        {
            if (target == null) return;
            float yaw = lookBackTimer > 0f ? 180f : yawOffset;
            Quaternion orbit = Quaternion.Euler(0f, target.eulerAngles.y + yaw, 0f);
            Vector3 desired = target.position + orbit * new Vector3(0f, height, -distance);
            if (collisionHandler != null) desired = collisionHandler.Resolve(desired, target.position + Vector3.up * 1.2f);
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSmooth * Time.deltaTime));
            Quaternion look = Quaternion.LookRotation(target.position + Vector3.up * 1.1f - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime));
            if (cameraComponent != null && vehicle != null) cameraComponent.fieldOfView = Mathf.Lerp(normalFov, maximumFov, Mathf.Clamp01(vehicle.SpeedKph / 80f));
        }
    }
}

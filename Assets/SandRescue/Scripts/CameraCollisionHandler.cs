using UnityEngine;

namespace SandRescue
{
    public class CameraCollisionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionMask = ~0;
        [SerializeField] private float radius = 0.25f;
        public Vector3 Resolve(Vector3 desired, Vector3 target)
        {
            Vector3 direction = desired - target; float distance = direction.magnitude;
            if (distance <= 0.01f) return desired;
            direction /= distance;
            if (Physics.SphereCast(target, radius, direction, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
                return target + direction * Mathf.Max(0.3f, hit.distance - 0.05f);
            return desired;
        }
    }
}

using UnityEngine;

namespace SandRescue
{
    public enum SandRescueSurface { Sand, HardSand, Rock, Asphalt, Unknown }

    public class SurfaceDetector : MonoBehaviour
    {
        [System.Serializable]
        public class SurfaceProfile
        {
            public SandRescueSurface surface = SandRescueSurface.Sand;
            [Range(0.1f, 2f)] public float forwardGrip = 0.75f;
            [Range(0.1f, 2f)] public float sidewaysGrip = 0.65f;
            [Range(0.1f, 2f)] public float speedMultiplier = 0.8f;
            [Range(0f, 1f)] public float dustAmount = 0.8f;
        }

        [SerializeField] private LayerMask surfaceMask = ~0;
        [SerializeField] private SurfaceProfile sand = new SurfaceProfile();
        [SerializeField] private SurfaceProfile hardSand = new SurfaceProfile { surface = SandRescueSurface.HardSand, forwardGrip = 0.95f, sidewaysGrip = 0.9f, speedMultiplier = 0.95f, dustAmount = 0.35f };
        [SerializeField] private SurfaceProfile rock = new SurfaceProfile { surface = SandRescueSurface.Rock, forwardGrip = 1.05f, sidewaysGrip = 1f, speedMultiplier = 0.9f, dustAmount = 0.15f };
        [SerializeField] private SurfaceProfile asphalt = new SurfaceProfile { surface = SandRescueSurface.Asphalt, forwardGrip = 1.2f, sidewaysGrip = 1.15f, speedMultiplier = 1f, dustAmount = 0f };

        public SurfaceProfile Current { get; private set; }
        public bool IsGrounded { get; private set; }

        private void FixedUpdate()
        {
            IsGrounded = Physics.Raycast(transform.position + Vector3.up * 0.15f, Vector3.down, out RaycastHit hit, 1.2f, surfaceMask, QueryTriggerInteraction.Ignore);
            Current = IsGrounded ? ProfileFromCollider(hit.collider) : sand;
        }

        private SurfaceProfile ProfileFromCollider(Collider collider)
        {
            string n = collider.gameObject.name.ToLowerInvariant();
            int layer = collider.gameObject.layer;
            if (n.Contains("asphalt") || n.Contains("road") || layer == LayerMask.NameToLayer("Asphalt")) return asphalt;
            if (n.Contains("rock") || layer == LayerMask.NameToLayer("Rock")) return rock;
            if (n.Contains("hard sand") || n.Contains("hardsand") || layer == LayerMask.NameToLayer("HardSand")) return hardSand;
            return sand;
        }
    }
}

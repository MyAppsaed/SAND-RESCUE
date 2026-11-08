using UnityEngine;

namespace SandRescue
{
    public class RescueTargetDetector : MonoBehaviour
    {
        [SerializeField] private Transform origin;
        [SerializeField] private float radius = 6f;
        [SerializeField] private LayerMask targetMask = ~0;
        public RescueConnectionPoint NearestConnection { get; private set; }
        private readonly Collider[] results = new Collider[32];
        private void Update()
        {
            NearestConnection = null; if (origin == null) return;
            int count = Physics.OverlapSphereNonAlloc(origin.position, radius, results, targetMask, QueryTriggerInteraction.Collide);
            float best = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                var point = results[i] != null ? results[i].GetComponentInParent<RescueConnectionPoint>() : null;
                if (point == null || !point.AcceptsWinch) continue;
                float d = (point.transform.position - origin.position).sqrMagnitude;
                if (d < best) { best = d; NearestConnection = point; }
            }
        }
    }
}

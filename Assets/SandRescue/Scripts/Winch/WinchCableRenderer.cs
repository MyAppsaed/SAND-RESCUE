using UnityEngine;

namespace SandRescue
{
    [RequireComponent(typeof(LineRenderer))]
    public class WinchCableRenderer : MonoBehaviour
    {
        [SerializeField] private Transform source;
        [SerializeField] private Transform target;
        private LineRenderer line;
        private void Awake() { line = GetComponent<LineRenderer>(); line.positionCount = 2; }
        public void SetEndpoints(Transform from, Transform to) { source = from; target = to; enabled = true; }
        public void Clear() { source = null; target = null; line.enabled = false; }
        private void LateUpdate()
        {
            if (source == null || target == null) { line.enabled = false; return; }
            line.enabled = true; line.SetPosition(0, source.position); line.SetPosition(1, target.position);
        }
    }
}

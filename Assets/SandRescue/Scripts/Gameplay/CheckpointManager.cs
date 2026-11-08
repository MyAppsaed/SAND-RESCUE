using UnityEngine;

namespace SandRescue
{
    public class CheckpointManager : MonoBehaviour
    {
        [SerializeField] private Transform initialCheckpoint;
        private Transform latestCheckpoint;
        public Transform LatestCheckpoint => latestCheckpoint != null ? latestCheckpoint : initialCheckpoint;
        public void Register(Transform checkpoint) { if (checkpoint != null) latestCheckpoint = checkpoint; }
        public bool TryGetRespawn(out Vector3 position, out Quaternion rotation)
        {
            Transform point = LatestCheckpoint;
            position = point != null ? point.position : Vector3.zero;
            rotation = point != null ? point.rotation : Quaternion.identity;
            return point != null;
        }
        public void ResetCheckpoint() => latestCheckpoint = null;
    }
}

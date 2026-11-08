using UnityEngine;

namespace SandRescue
{
    public class MissionMarker : MonoBehaviour
    {
        [SerializeField] private string markerId;
        [SerializeField] private Transform target;
        public string MarkerId => markerId;
        public Transform Target => target != null ? target : transform;
        public void SetTarget(Transform value) => target = value;
    }
}

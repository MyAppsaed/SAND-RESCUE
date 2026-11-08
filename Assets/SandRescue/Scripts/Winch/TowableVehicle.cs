using UnityEngine;

namespace SandRescue
{
    [RequireComponent(typeof(Rigidbody))]
    public class TowableVehicle : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private float resistance = 80f;
        [SerializeField] private bool requiresHeavyWinch;
        public Rigidbody Body => body;
        public bool RequiresHeavyWinch => requiresHeavyWinch;
        public bool IsBeingTowed { get; private set; }
        private void Awake() { if (body == null) body = GetComponent<Rigidbody>(); }
        public void SetTowed(bool value) { IsBeingTowed = value; }
        public void ApplyTowForce(Vector3 direction, float force)
        {
            if (body == null || !IsBeingTowed) return;
            body.AddForce(direction.normalized * Mathf.Max(0f, force) - body.velocity * resistance, ForceMode.Force);
        }
    }
}

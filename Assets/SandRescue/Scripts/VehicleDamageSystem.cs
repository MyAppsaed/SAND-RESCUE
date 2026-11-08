using UnityEngine;

namespace SandRescue
{
    public class VehicleDamageSystem : MonoBehaviour
    {
        [SerializeField] private VehicleStatus status;
        [SerializeField] private float minimumImpactSpeedKph = 12f;
        [SerializeField] private float damagePerKphOverThreshold = 0.8f;
        private void Awake() { if (status == null) status = GetComponent<VehicleStatus>(); }
        private void OnCollisionEnter(Collision collision)
        {
            float impactKph = collision.relativeVelocity.magnitude * 3.6f;
            if (impactKph <= minimumImpactSpeedKph || status == null) return;
            status.ApplyDamage((impactKph - minimumImpactSpeedKph) * damagePerKphOverThreshold);
        }
    }
}

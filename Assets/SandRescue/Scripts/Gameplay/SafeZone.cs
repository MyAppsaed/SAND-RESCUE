using UnityEngine;
using UnityEngine.Events;

namespace SandRescue
{
    public class SafeZone : MonoBehaviour
    {
        [SerializeField] private string zoneId = "safe_zone_01";
        public UnityEvent<string, GameObject> OnVehicleEntered;
        private void OnTriggerEnter(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            if (body == null) return;
            OnVehicleEntered?.Invoke(zoneId, body.gameObject);
            if (body.GetComponent<SandRescueVehicleController>() != null)
                MissionManager.Active?.ReportObjective(zoneId, MissionObjectiveType.ReachSafeZone, body.gameObject);
        }
    }
}

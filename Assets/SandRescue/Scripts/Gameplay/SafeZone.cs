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
            if (other.attachedRigidbody != null) OnVehicleEntered?.Invoke(zoneId, other.attachedRigidbody.gameObject);
        }
    }
}

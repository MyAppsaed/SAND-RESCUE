using UnityEngine;

namespace SandRescue
{
    public class RescueCharacter : RescueTarget
    {
        [SerializeField] private Transform carryPoint;
        public bool IsBeingTransported { get; private set; }
        public void LoadIntoVehicle(Transform vehicle)
        {
            if (vehicle == null) return; BeginRescue(); IsBeingTransported = true; transform.SetParent(vehicle); transform.position = carryPoint != null ? carryPoint.position : vehicle.position;
        }
        public void DeliverToSafety() { if (IsBeingTransported) { IsBeingTransported = false; CompleteRescue(); } }
    }
}

using UnityEngine;

namespace SandRescue
{
    public class StrandedVehicle : RescueTarget
    {
        [SerializeField] private TowableVehicle towable;
        [SerializeField] private SafeZone destination;
        private void Awake() { if (towable == null) towable = GetComponent<TowableVehicle>(); }
        private void OnTriggerEnter(Collider other)
        {
            if (State != RescueTargetState.RescueInProgress || destination == null) return;
            // The entering collider here belongs to the zone, not our own Rigidbody.
            SafeZone enteredZone = other.GetComponentInParent<SafeZone>();
            if (enteredZone == destination) CompleteRescue();
        }
        public override void BeginRescue() { base.BeginRescue(); if (towable != null) towable.SetTowed(true); }
    }
}

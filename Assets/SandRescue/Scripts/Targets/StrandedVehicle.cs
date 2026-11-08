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
            if (other.attachedRigidbody == towable?.Body) CompleteRescue();
        }
        public override void BeginRescue() { base.BeginRescue(); if (towable != null) towable.SetTowed(true); }
    }
}

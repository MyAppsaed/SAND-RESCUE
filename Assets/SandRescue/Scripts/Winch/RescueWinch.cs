using UnityEngine;

namespace SandRescue
{
    public class RescueWinch : MonoBehaviour
    {
        [SerializeField] private Rigidbody vehicleBody;
        [SerializeField] private Transform attachmentPoint;
        [SerializeField] private WinchCableRenderer cable;
        [SerializeField] private float maximumCableLength = 25f;
        [SerializeField] private float minimumCableLength = 4f;
        [SerializeField] private float winchForce = 4500f;
        [SerializeField] private float reelSpeed = 8f;
        [SerializeField] private float breakForce = 9000f;
        [SerializeField] private float attachDistance = 6f;
        [SerializeField] private bool canUseHeavyWinch;
        public TowableVehicle ConnectedTarget { get; private set; }
        public float CableLength { get; private set; }
        public bool IsAttached => ConnectedTarget != null;
        private void Awake() { if (vehicleBody == null) vehicleBody = GetComponentInParent<Rigidbody>(); CableLength = maximumCableLength; }
        public bool TryAttach(RescueConnectionPoint point)
        {
            if (point == null || !point.AcceptsWinch || point.Towable == null || attachmentPoint == null) return false;
            float distance = Vector3.Distance(attachmentPoint.position, point.transform.position);
            if (distance > attachDistance || (point.Towable.RequiresHeavyWinch && !canUseHeavyWinch)) return false;
            ConnectedTarget = point.Towable; ConnectedTarget.SetTowed(true); CableLength = distance;
            if (cable != null) cable.SetEndpoints(attachmentPoint, point.transform);
            MissionManager.Active?.ReportObjective(point.PointId, MissionObjectiveType.AttachWinch, gameObject);
            return true;
        }
        public void Detach()
        {
            if (ConnectedTarget != null) ConnectedTarget.SetTowed(false);
            ConnectedTarget = null; if (cable != null) cable.Clear();
        }
        public void ReelIn() => CableLength = Mathf.Max(minimumCableLength, CableLength - reelSpeed * Time.deltaTime);
        public void ReelOut() => CableLength = Mathf.Min(maximumCableLength, CableLength + reelSpeed * Time.deltaTime);
        private void FixedUpdate()
        {
            if (ConnectedTarget == null || vehicleBody == null || attachmentPoint == null) return;
            if (ConnectedTarget.Body == null || !ConnectedTarget.isActiveAndEnabled) { Detach(); return; }
            Vector3 delta = attachmentPoint.position - ConnectedTarget.Body.worldCenterOfMass;
            float distance = delta.magnitude;
            if (distance > CableLength + 0.5f) ConnectedTarget.ApplyTowForce(delta, winchForce);
            if (distance > maximumCableLength + 1f || ConnectedTarget.Body.velocity.magnitude * ConnectedTarget.Body.mass > breakForce) Detach();
        }
    }
}

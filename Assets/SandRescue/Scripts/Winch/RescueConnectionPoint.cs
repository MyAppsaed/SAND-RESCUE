using UnityEngine;

namespace SandRescue
{
    public class RescueConnectionPoint : MonoBehaviour
    {
        [SerializeField] private string pointId = "front";
        [SerializeField] private bool acceptsWinch = true;
        public string PointId => pointId;
        public bool AcceptsWinch => acceptsWinch;
        public TowableVehicle Towable { get; private set; }
        private void Awake() { Towable = GetComponentInParent<TowableVehicle>(); }
    }
}

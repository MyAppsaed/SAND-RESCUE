using UnityEngine;

namespace SandRescue
{
    public class MissionTrigger : MonoBehaviour
    {
        [SerializeField] private string triggerId;
        [SerializeField] private MissionObjectiveType triggerType = MissionObjectiveType.ReachTrigger;
        public string TriggerId => triggerId;
        public MissionObjectiveType TriggerType => triggerType;
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody == null) return;
            MissionManager.Active?.ReportObjective(triggerId, triggerType, other.attachedRigidbody.gameObject);
        }
    }
}

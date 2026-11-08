using UnityEngine;
using UnityEngine.Events;

namespace SandRescue
{
    public enum RescueTargetState { Idle, WaitingForRescue, RescueInProgress, Rescued, Failed }

    public class RescueTarget : MonoBehaviour
    {
        [SerializeField] protected string targetId;
        [SerializeField] protected MissionObjectiveType objectiveType;
        public string TargetId => targetId;
        public RescueTargetState State { get; private set; } = RescueTargetState.WaitingForRescue;
        public UnityEvent OnRescued;
        public UnityEvent OnFailed;
        public virtual void BeginRescue() { if (State == RescueTargetState.WaitingForRescue) State = RescueTargetState.RescueInProgress; }
        public virtual void CompleteRescue()
        {
            if (State == RescueTargetState.Rescued) return;
            State = RescueTargetState.Rescued; OnRescued?.Invoke(); MissionManager.Active?.ReportObjective(targetId, objectiveType, gameObject);
        }
        public virtual void FailRescue() { State = RescueTargetState.Failed; OnFailed?.Invoke(); }
    }
}

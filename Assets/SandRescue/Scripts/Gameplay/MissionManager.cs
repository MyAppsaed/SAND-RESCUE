using System;
using System.Collections.Generic;
using UnityEngine;

namespace SandRescue
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Active { get; private set; }
        [SerializeField] private RescueMissionData currentMission;
        [SerializeField] private VehicleStatus playerStatus;
        [SerializeField] private CheckpointManager checkpointManager;
        public RescueMissionData CurrentMission => currentMission;
        public IReadOnlyList<MissionObjectiveProgress> Progress => progress;
        public float ElapsedSeconds { get; private set; }
        public bool IsRunning { get; private set; }
        public bool RewardsIssued { get; private set; }
        public event Action<MissionObjectiveProgress> ObjectiveChanged;
        public event Action<MissionResults> MissionCompleted;
        public event Action<string> MissionFailed;
        private readonly List<MissionObjectiveProgress> progress = new List<MissionObjectiveProgress>();

        private void Awake() { Active = this; }
        private void OnDestroy() { if (Active == this) Active = null; }
        private void Update()
        {
            if (!IsRunning || currentMission == null) return;
            ElapsedSeconds += Time.deltaTime;
            if (currentMission.timeLimitSeconds > 0f && ElapsedSeconds >= currentMission.timeLimitSeconds) Fail("mission_time_expired");
        }
        public bool StartMission(RescueMissionData mission)
        {
            if (mission == null || (playerStatus != null && playerStatus.Fuel < mission.minimumFuel)) return false;
            currentMission = mission; ElapsedSeconds = 0f; RewardsIssued = false; IsRunning = true; progress.Clear();
            for (int i = 0; i < mission.objectives.Count; i++) progress.Add(new MissionObjectiveProgress { objectiveId = mission.objectives[i].objectiveId });
            return true;
        }
        public void ReportObjective(string targetId, MissionObjectiveType type, GameObject actor)
        {
            if (!IsRunning || currentMission == null) return;
            for (int i = 0; i < currentMission.objectives.Count; i++)
            {
                var definition = currentMission.objectives[i];
                if (definition.type != type || definition.targetId != targetId) continue;
                var item = progress[i];
                item.currentAmount = Mathf.Min(definition.requiredAmount, item.currentAmount + 1);
                item.completed = item.currentAmount >= definition.requiredAmount;
                ObjectiveChanged?.Invoke(item);
                if (AllRequiredObjectivesComplete()) Complete();
                return;
            }
        }
        public void ReportObjective(string targetId, MissionObjectiveType type) => ReportObjective(targetId, type, null);
        public bool AllRequiredObjectivesComplete()
        {
            for (int i = 0; i < currentMission.objectives.Count; i++) if (!currentMission.objectives[i].optional && !progress[i].completed) return false;
            return true;
        }
        public void Complete()
        {
            if (!IsRunning || RewardsIssued) return;
            IsRunning = false; RewardsIssued = true;
            float health = playerStatus != null ? playerStatus.Health01 : 1f;
            MissionCompleted?.Invoke(MissionRewardCalculator.Calculate(currentMission, ElapsedSeconds, health));
        }
        public void Fail(string reasonKey)
        {
            if (!IsRunning) return;
            IsRunning = false; MissionFailed?.Invoke(string.IsNullOrWhiteSpace(reasonKey) ? "mission_failed" : reasonKey);
        }
        public bool RestartFromCheckpoint(GameObject player)
        {
            if (player == null || checkpointManager == null || !checkpointManager.TryGetRespawn(out Vector3 p, out Quaternion r)) return false;
            var body = player.GetComponent<Rigidbody>(); if (body == null) return false;
            body.position = p; body.rotation = r; body.velocity = Vector3.zero; body.angularVelocity = Vector3.zero; return true;
        }
    }
}

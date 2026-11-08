using System.Collections.Generic;
using UnityEngine;

namespace SandRescue
{
    [CreateAssetMenu(menuName = "SAND RESCUE/Mission Data", fileName = "Mission_")]
    public class RescueMissionData : ScriptableObject
    {
        public string missionId;
        public BilingualText title;
        public BilingualText description;
        public Sprite image;
        [Range(1, 5)] public int difficulty = 1;
        [Min(0f)] public float timeLimitSeconds;
        [Min(0f)] public float estimatedDistanceMeters;
        [Min(0f)] public float minimumFuel = 10f;
        public string requiredVehicleClass;
        public int unlockAfterMissionIndex = -1;
        public int coinsReward = 100;
        public int experienceReward = 50;
        public List<MissionObjectiveDefinition> objectives = new List<MissionObjectiveDefinition>();
    }
}

using System;
using UnityEngine;

namespace SandRescue
{
    public enum MissionObjectiveType { ReachTrigger, AttachWinch, TowTarget, DeliverFuel, RescueCharacter, RecoverSupply, ReachSafeZone, SearchArea }

    [Serializable]
    public class MissionObjectiveDefinition
    {
        public string objectiveId;
        public MissionObjectiveType type;
        public BilingualText title;
        [Min(1)] public int requiredAmount = 1;
        public string targetId;
        public bool optional;
    }

    [Serializable]
    public class MissionObjectiveProgress
    {
        public string objectiveId;
        public int currentAmount;
        public bool completed;
        public float Normalized => Mathf.Clamp01((float)currentAmount / Mathf.Max(1, 1));
    }
}

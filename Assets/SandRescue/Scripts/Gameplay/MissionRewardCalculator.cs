using UnityEngine;

namespace SandRescue
{
    public static class MissionRewardCalculator
    {
        public static MissionResults Calculate(RescueMissionData mission, float completionTime, float health01)
        {
            if (mission == null) return new MissionResults();
            int stars = 1;
            if (health01 >= 0.5f) stars++;
            if (mission.timeLimitSeconds <= 0f || completionTime <= mission.timeLimitSeconds * 0.75f) stars++;
            return new MissionResults
            {
                success = true,
                stars = Mathf.Clamp(stars, 1, 3),
                coins = Mathf.Max(0, mission.coinsReward) * stars / 3,
                experience = Mathf.Max(0, mission.experienceReward),
                completionTimeSeconds = Mathf.Max(0f, completionTime)
            };
        }
    }
}

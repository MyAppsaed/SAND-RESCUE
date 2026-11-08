using System;

namespace SandRescue
{
    [Serializable]
    public class MissionResults
    {
        public bool success;
        public int stars;
        public int coins;
        public int experience;
        public float completionTimeSeconds;
        public bool isNewBestTime;
    }
}

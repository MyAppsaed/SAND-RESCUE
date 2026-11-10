using UnityEngine;

namespace SandRescue
{
    /// <summary>Starter-scene helper. Production mission selection will replace this component.</summary>
    public class MissionAutoStarter : MonoBehaviour
    {
        [SerializeField] private MissionManager missionManager;

        private void Start()
        {
            if (missionManager == null) missionManager = MissionManager.Active;
            if (missionManager != null) missionManager.StartAssignedMission();
        }
    }
}

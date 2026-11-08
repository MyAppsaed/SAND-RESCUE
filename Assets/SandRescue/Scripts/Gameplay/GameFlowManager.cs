using UnityEngine;
using UnityEngine.SceneManagement;

namespace SandRescue
{
    public class GameFlowManager : MonoBehaviour
    {
        [SerializeField] private MissionManager missionManager;
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string missionScene = "DesertMission";
        public void LoadMainMenu() { if (!string.IsNullOrWhiteSpace(mainMenuScene)) SceneManager.LoadScene(mainMenuScene); }
        public void LoadMissionScene() { if (!string.IsNullOrWhiteSpace(missionScene)) SceneManager.LoadScene(missionScene); }
        public void RestartMission() { if (missionManager != null && missionManager.CurrentMission != null) missionManager.StartMission(missionManager.CurrentMission); }
        public void AbandonMission() { if (missionManager != null) missionManager.Fail("mission_abandoned"); LoadMainMenu(); }
    }
}

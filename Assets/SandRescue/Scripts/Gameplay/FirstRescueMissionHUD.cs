using UnityEngine;

namespace SandRescue
{
    /// <summary>Desktop first-rescue smoke-test HUD; not the final localized production UI.</summary>
    public class FirstRescueMissionHUD : MonoBehaviour
    {
        [SerializeField] private MissionManager missionManager;
        [SerializeField] private RescueWinch winch;
        [SerializeField] private StrandedVehicle target;
        private GUIStyle heading;
        private GUIStyle body;

        private void Awake()
        {
            if (missionManager == null) missionManager = MissionManager.Active;
            if (winch == null) winch = FindFirstObjectByType<RescueWinch>();
            if (target == null) target = FindFirstObjectByType<StrandedVehicle>();
        }

        private void OnGUI()
        {
            if (heading == null)
            {
                heading = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
                body = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            }

            GUILayout.BeginArea(new Rect(12f, 230f, Mathf.Min(470f, Screen.width - 24f), 190f), GUI.skin.box);
            GUILayout.Label("FIRST RESCUE — TOW TEST", heading);
            string state = target == null ? "Target: missing" : "Target: " + target.State;
            string cable = winch != null && winch.IsAttached ? "Winch: ATTACHED" : "Winch: ready — press E near target";
            GUILayout.Label(state + "\n" + cable + "\nE attach   Q detach   T reel in   Y reel out\nF free-look   Hold right mouse and drag; wheel zooms", body);
            if (missionManager != null)
            {
                string missionState = missionManager.IsRunning ? "Mission active" : (missionManager.RewardsIssued ? "MISSION COMPLETE" : "Mission inactive");
                GUILayout.Label(missionState, body);
                if (missionManager.Progress != null)
                    foreach (MissionObjectiveProgress item in missionManager.Progress)
                        GUILayout.Label((item.completed ? "✓ " : "○ ") + item.objectiveId, body);
            }
            GUILayout.Label("Tow the stranded vehicle into the marked safe zone.", body);
            GUILayout.EndArea();
        }
    }
}

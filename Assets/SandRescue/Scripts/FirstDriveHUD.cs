using UnityEngine;

namespace SandRescue
{
    /// <summary>Desktop smoke-test overlay only; not the production/localized game UI.</summary>
    public class FirstDriveHUD : MonoBehaviour
    {
        [SerializeField] private SandRescueVehicleController vehicle;
        private GUIStyle heading;
        private GUIStyle bodyStyle;

        private void OnGUI()
        {
            if (heading == null)
            {
                heading = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true
                };
                bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            }
            float width = Mathf.Min(420f, Screen.width - 24f);
            if (width < 160f) return;
            GUILayout.BeginArea(new Rect(12f, 12f, width, 210f), GUI.skin.box);
            GUILayout.Label("SANID — FIRST DRIVE TEST", heading);
            if (vehicle != null)
                GUILayout.Label(string.Format("Speed: {0:0} km/h    Grounded: {1}",
                    vehicle.SpeedKph, vehicle.IsGrounded ? "Yes" : "No"), bodyStyle);
            GUILayout.Label("Click the Game view to focus keyboard input.\nW/S or arrows: forward/reverse\nA/D or arrows: steer   Space: handbrake\nR: recover   C: brief rear view", bodyStyle);
            GUILayout.Label("Driving setup only — no rescue mission, mobile controls or finished branding yet.", bodyStyle);
            GUILayout.EndArea();
        }
    }
}

using UnityEngine;

namespace SandRescue
{
    public class VehicleAudioController : MonoBehaviour
    {
        [SerializeField] private SandRescueVehicleController controller;
        [SerializeField] private AudioSource engineSource;
        [SerializeField] private float idlePitch = 0.85f;
        [SerializeField] private float maxPitch = 1.65f;
        private void Update()
        {
            if (controller == null || engineSource == null) return;
            float rpm01 = Mathf.Clamp01(controller.SpeedKph / 80f);
            engineSource.pitch = Mathf.Lerp(idlePitch, maxPitch, rpm01);
            if (!engineSource.isPlaying) engineSource.Play();
        }
    }
}

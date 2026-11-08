using UnityEngine;

namespace SandRescue
{
    public class VehicleEffectsController : MonoBehaviour
    {
        [SerializeField] private SandRescueVehicleController controller;
        [SerializeField] private ParticleSystem[] tireDust;
        [SerializeField] private Light[] brakeLights;
        [SerializeField] private Light[] reverseLights;
        private void Update()
        {
            if (controller == null) return;
            bool moving = controller.SpeedKph > 3f;
            for (int i = 0; i < tireDust.Length; i++) if (tireDust[i] != null)
            {
                var emission = tireDust[i].emission; emission.enabled = moving && controller.IsGrounded;
            }
            float forwardSpeed = Vector3.Dot(controller.transform.forward, controller.GetComponent<Rigidbody>().velocity);
            SetLights(brakeLights, !moving);
            SetLights(reverseLights, forwardSpeed < -0.5f);
        }
        private static void SetLights(Light[] lights, bool on) { for (int i = 0; i < lights.Length; i++) if (lights[i] != null) lights[i].enabled = on; }
    }
}

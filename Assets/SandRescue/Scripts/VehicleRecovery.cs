using UnityEngine;

namespace SandRescue
{
    public class VehicleRecovery : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private float upsideDownSeconds = 3f;
        [SerializeField] private float uprightDotThreshold = 0.2f;
        [SerializeField] private float liftHeight = 1.2f;
        private float timer;

        public bool IsUpsideDown => Vector3.Dot(transform.up, Vector3.up) < uprightDotThreshold;

        private void Awake() { if (body == null) body = GetComponent<Rigidbody>(); }
        private void Update()
        {
            timer = IsUpsideDown ? timer + Time.deltaTime : 0f;
            if (timer >= upsideDownSeconds) Recover();
        }
        public void Recover()
        {
            timer = 0f;
            Vector3 p = transform.position + Vector3.up * liftHeight;
            Quaternion q = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            body.position = p; body.rotation = q; body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero;
        }
    }
}

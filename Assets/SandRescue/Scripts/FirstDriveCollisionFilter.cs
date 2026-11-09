using UnityEngine;

namespace SandRescue
{
    /// <summary>Explicit self-collision filtering for the generated test rig only.</summary>
    [DisallowMultipleComponent]
    public class FirstDriveCollisionFilter : MonoBehaviour
    {
        [SerializeField] private Collider bodyHull;
        [SerializeField] private WheelCollider[] wheelColliders;

        private void OnEnable()
        {
            if (wheelColliders == null) return;
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                WheelCollider wheel = wheelColliders[i];
                if (wheel == null) continue;
                if (bodyHull != null) Physics.IgnoreCollision(bodyHull, wheel, true);
                for (int j = i + 1; j < wheelColliders.Length; j++)
                    if (wheelColliders[j] != null)
                        Physics.IgnoreCollision(wheel, wheelColliders[j], true);
            }
        }
    }
}

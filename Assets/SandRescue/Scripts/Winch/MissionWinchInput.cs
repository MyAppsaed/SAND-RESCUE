using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SandRescue
{
    /// <summary>Desktop-only mission smoke-test input; replace with the production input action later.</summary>
    public class MissionWinchInput : MonoBehaviour
    {
        [SerializeField] private RescueWinch winch;

        private void Awake()
        {
            if (winch == null) winch = GetComponent<RescueWinch>();
        }

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null || winch == null) return;
            if (Keyboard.current.eKey.wasPressedThisFrame) winch.AttachNearestTarget();
            if (Keyboard.current.qKey.wasPressedThisFrame) winch.Detach();
            if (Keyboard.current.tKey.isPressed) winch.ReelIn();
            if (Keyboard.current.yKey.isPressed) winch.ReelOut();
#endif
        }
    }
}

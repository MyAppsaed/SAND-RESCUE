using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SandRescue
{
    [DefaultExecutionOrder(-100)]
    public class VehicleInputManager : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private MobileVehicleInput mobileInput;
        [SerializeField] private bool readKeyboard = true;
        [SerializeField] private bool readGamepad = true;
        [SerializeField] private bool allowMobileInput = true;

        [Header("Tuning")]
        [SerializeField, Range(0.01f, 0.5f)] private float inputSmoothing = 0.08f;

        public VehicleInputState Current { get; private set; }
        public bool LookBackPressed => Current.LookBack;

        private float targetThrottle;
        private float targetSteering;
        private bool targetHandbrake;
        private bool recoverPressed;
        private bool lookBackPressed;

        private void Update()
        {
            ReadInput();
            // A struct property returns a copy; update a local value, then publish it.
            VehicleInputState state = Current;
            float smoothing = Mathf.Max(0.01f, inputSmoothing);
            state.Throttle = Mathf.MoveTowards(state.Throttle, targetThrottle, Time.deltaTime / smoothing);
            state.Steering = Mathf.MoveTowards(state.Steering, targetSteering, Time.deltaTime / smoothing);
            state.Handbrake = targetHandbrake;
            state.RecoverPressed = recoverPressed;
            state.LookBack = lookBackPressed;
            Current = state;
            recoverPressed = false;
            lookBackPressed = false;
            if (allowMobileInput && mobileInput != null) mobileInput.ClearMomentaryButtons();
        }

        private void ReadInput()
        {
            targetThrottle = 0f;
            targetSteering = 0f;
            targetHandbrake = false;

#if ENABLE_INPUT_SYSTEM
            if (readKeyboard && Keyboard.current != null)
            {
                float forward = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1f : 0f;
                float reverse = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? 1f : 0f;
                targetThrottle = forward - reverse;
                targetSteering = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f)
                               - (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
                targetHandbrake = Keyboard.current.spaceKey.isPressed;
                if (Keyboard.current.rKey.wasPressedThisFrame) recoverPressed = true;
                if (Keyboard.current.cKey.wasPressedThisFrame) lookBackPressed = true;
            }

            if (readGamepad && Gamepad.current != null)
            {
                var pad = Gamepad.current;
                targetThrottle = Mathf.Abs(pad.leftStick.y.ReadValue()) > 0.05f ? pad.leftStick.y.ReadValue() : targetThrottle;
                targetSteering = Mathf.Abs(pad.leftStick.x.ReadValue()) > 0.05f ? pad.leftStick.x.ReadValue() : targetSteering;
                targetHandbrake |= pad.leftShoulder.isPressed;
                if (pad.buttonEast.wasPressedThisFrame) recoverPressed = true;
                if (pad.buttonNorth.wasPressedThisFrame) lookBackPressed = true;
            }
#endif

            if (allowMobileInput && mobileInput != null)
            {
                var mobile = mobileInput.State;
                if (Mathf.Abs(mobile.Throttle) > 0.01f || Mathf.Abs(mobile.Steering) > 0.01f || mobile.Handbrake)
                {
                    targetThrottle = mobile.Throttle;
                    targetSteering = mobile.Steering;
                    targetHandbrake = mobile.Handbrake;
                }
                recoverPressed |= mobile.RecoverPressed;
                lookBackPressed |= mobile.LookBack;
            }
        }
    }
}

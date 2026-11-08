using UnityEngine;

namespace SandRescue
{
    public class MobileVehicleInput : MonoBehaviour
    {
        public VehicleInputState State { get; private set; }

        public void SetSteering(float value) => State = With(State, Mathf.Clamp(value, -1f, 1f), State.Throttle, State.Handbrake);
        public void SetThrottle(float value) => State = With(State, State.Steering, Mathf.Clamp(value, -1f, 1f), State.Handbrake);
        public void SetHandbrake(bool pressed) => State = With(State, State.Steering, State.Throttle, pressed);
        public void Recover() { State = With(State, State.Steering, State.Throttle, State.Handbrake); State = WithFlags(State, true, State.LookBack); }
        public void LookBack() { State = With(State, State.Steering, State.Throttle, State.Handbrake); State = WithFlags(State, State.RecoverPressed, true); }
        public void ClearMomentaryButtons() => State = WithFlags(State, false, false);

        private static VehicleInputState With(VehicleInputState state, float steering, float throttle, bool handbrake)
        {
            state.Steering = steering; state.Throttle = throttle; state.Handbrake = handbrake; return state;
        }
        private static VehicleInputState WithFlags(VehicleInputState state, bool recover, bool lookBack)
        {
            state.RecoverPressed = recover; state.LookBack = lookBack; return state;
        }
    }
}

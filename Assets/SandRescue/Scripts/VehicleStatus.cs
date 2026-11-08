using UnityEngine;
using UnityEngine.Events;

namespace SandRescue
{
    public class VehicleStatus : MonoBehaviour
    {
        [SerializeField] private SandRescueVehicleController controller;
        [SerializeField, Min(0f)] private float maximumHealth = 100f;
        [SerializeField, Min(0f)] private float maximumFuel = 100f;
        [SerializeField] private float fuelUsePerSecond = 0.015f;
        public UnityEvent OnFuelEmpty;
        public float Health { get; private set; }
        public float Fuel { get; private set; }
        public float Health01 => maximumHealth <= 0f ? 0f : Health / maximumHealth;
        public float Fuel01 => maximumFuel <= 0f ? 0f : Fuel / maximumFuel;
        public bool CanDrive => Fuel > 0f && Health > 0f;

        private void Awake() { Health = maximumHealth; Fuel = maximumFuel; }
        private void Update()
        {
            if (controller == null || !CanDrive) return;
            Fuel = Mathf.Max(0f, Fuel - controller.SpeedKph / 3.6f * fuelUsePerSecond * Time.deltaTime);
            if (Fuel <= 0f) OnFuelEmpty?.Invoke();
        }
        public void ConsumeFuel(float amount) => Fuel = Mathf.Clamp(Fuel - Mathf.Max(0f, amount), 0f, maximumFuel);
        public void ApplyDamage(float amount) => Health = Mathf.Clamp(Health - Mathf.Max(0f, amount), 0f, maximumHealth);
        public void Repair(float amount) => Health = Mathf.Clamp(Health + Mathf.Max(0f, amount), 0f, maximumHealth);
        public void Refuel(float amount) => Fuel = Mathf.Clamp(Fuel + Mathf.Max(0f, amount), 0f, maximumFuel);
    }
}

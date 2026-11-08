using UnityEngine;

namespace SandRescue
{
    public class RescueSupplyTarget : RescueTarget
    {
        public void Collect() { BeginRescue(); CompleteRescue(); }
    }
}

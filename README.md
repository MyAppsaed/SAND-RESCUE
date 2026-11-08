# SAND RESCUE — Unity 6 Starter Package

This package implements the core third-person off-road rescue vehicle foundation for Unity 6 with URP and the New Input System. It is intentionally asset-free: use primitive meshes, Unity particle systems, and your own licensed audio.

## 1. Create the GameObjects

Create a `RescueVehicle` root GameObject with:

- `Rigidbody` (mass 1800, interpolation Interpolate, collision detection Continuous Dynamic)
- `SandRescueVehicleController`
- `VehicleInputManager`
- `SurfaceDetector`
- `VehicleRecovery`
- `VehicleStatus`
- `VehicleDamageSystem`
- `VehicleAudioController`
- `VehicleEffectsController`
- A child `CenterOfMass` transform positioned slightly below the visual body center
- Four child wheel objects, each with a `WheelCollider` and `VehicleWheel`
- Four visual wheel meshes assigned to the corresponding `VehicleWheel.visualWheel`

Create a `Main Camera` with `VehicleCameraController` and `CameraCollisionHandler`. Assign the vehicle root as the target.

Create a `MobileInput` Canvas object with `MobileVehicleInput`. UI buttons should call `SetThrottle(1)`, `SetThrottle(-1)`, `SetThrottle(0)`, `SetSteering(-1)`, `SetSteering(1)`, `SetSteering(0)`, `SetHandbrake(true/false)`, `Recover()`, and `LookBack()` through UnityEvent bindings. The vehicle reads this interface; buttons never touch vehicle physics directly.

## 2. WheelCollider alignment

Place each WheelCollider at the center of its tire, with its local Y axis pointing upward and its local Z axis pointing forward. Set the radius to match the tire mesh. Keep the collider's suspension distance small enough that the chassis does not visibly float. Front wheels should have `steeringWheel` enabled; all four wheels should have `drivenWheel` enabled for four-wheel drive; all four should normally have `brakeWheel` enabled.

## 3. Inspector assignments

On `SandRescueVehicleController`, assign the Rigidbody, VehicleInputManager, SurfaceDetector, VehicleRecovery, CenterOfMass, and all four VehicleWheel components. Set the defaults to 80 km/h forward, 25 km/h reverse, 1900 motor torque, 3200 brake torque, 32° low-speed steering, and 12° high-speed steering.

On `VehicleInputManager`, assign the MobileVehicleInput component if mobile controls are used. On `VehicleCameraController`, assign the target, controller, Camera, and CameraCollisionHandler. On `VehicleStatus`, assign the controller. On the audio/effects components, assign the engine AudioSource, dust ParticleSystems, brake Lights, and reverse Lights.

## 4. Surface setup

Create optional `Sand`, `Rock`, and `Asphalt` layers. The detector also recognizes object names containing `sand`, `rock`, `asphalt`, or `road`. Tune the serialized profiles on `SurfaceDetector`: sand starts with reduced grip and an 0.8 speed multiplier; rock has moderate grip; asphalt has the best grip and no dust.

## 5. Keyboard test

Use **W / Up Arrow** to accelerate, **S / Down Arrow** to reverse/brake, **A / Left Arrow** and **D / Right Arrow** to steer, **Space** for the handbrake, **R** to recover an overturned vehicle, and **C** to look back. A compatible gamepad is also supported through the New Input System.

## 6. Recommended Unity setup

Install and enable the `Input System` package through Package Manager and select the new input backend when Unity prompts. Add a simple desert test scene with a large plane, raised dune meshes, a few rock colliders, and rescue markers. Use URP-compatible materials. Add a `Canvas` HUD showing `VehicleStatus.Health01`, `Fuel01`, and `SandRescueVehicleController.SpeedKph`.

## Notes

The package avoids `FindObjectOfType` in runtime loops, keeps physics work in `FixedUpdate`, caches important references, exposes tuning fields in the Inspector, and guards calculations against missing references and zero ranges. The code is a core systems starter rather than a complete shippable game: mission logic, save data, navigation, rescue interactions, menus, and production art still need to be added.

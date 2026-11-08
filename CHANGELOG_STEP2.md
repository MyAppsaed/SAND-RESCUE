# Change Log — Step 2

## Added
- Data-driven ScriptableObject mission definitions.
- Mission objective progress and completion/failure flow.
- Checkpoint registration and respawn support.
- Safe-zone triggers and mission triggers.
- Winch connection points, target detection, cable rendering, reel controls, force limits, and safe detachment.
- Towable vehicles and reusable rescue target state machine.
- Stranded vehicle, character, and supply target types.
- Bilingual mission text data with Arabic-first language preference and stable localization keys.
- Step 2 setup and validation documentation.

## Preserved
- Existing Step 1 vehicle controller, input manager, mobile input, camera, status, damage, audio, effects, wheel, recovery, and surface systems.

## Modified
- `SurfaceDetector.cs`: added `HardSand` to the surface enum for Step 2 environment authoring.

## Not yet verified in this sandbox
- Unity Editor compilation and Play Mode physics.
- Unity Localization package import and Arabic RTL shaping.
- Complete scene/prefab/ScriptableObject asset creation inside the Unity Editor.
- Android/iOS builds and physical-device testing.

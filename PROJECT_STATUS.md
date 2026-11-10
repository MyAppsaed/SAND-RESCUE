# SAND RESCUE Project Status

The repository is a Unity 6.0 project scaffold and has been published to GitHub. The user installed and opened Unity 6.0 LTS 6000.0.85f2 on Windows. ProjectVersion.txt selects that editor without an invented revision hash; Unity writes its exact revision on import. This sandbox still has no Unity Editor or access to the user's local metadata and settings.

## Product direction

The current release is offline and single-player. The user confirmed online multiplayer as the next stage on 2026-11-10. Establish clear input, simulation authority, presentation, stable-identity and save-data boundaries while completing the offline loop. No networking package or service has been added. See MULTIPLAYER_FOUNDATION.md for the recorded architecture direction and current gaps; the present prototype is not yet multiplayer-ready.

Day and Night modes were requested on 2026-11-10. The recommended first implementation is mission lighting presets with usable nighttime headlights, followed optionally by a continuous cycle. The current test scene is daytime-only; this feature is recorded, not implemented. See DAY_NIGHT_DESIGN.md. In future multiplayer, the shared lighting state belongs to the session authority, not independent client clocks.

## Current source and art

- Step 1 vehicle foundation and Step 2 gameplay scripts exist under Assets/SandRescue.
- The input-state struct assignment compile blocker was corrected at source level.
- Camera touch input now uses the New Input System rather than legacy Input.touches.
- Mission and winch actions have void-returning handlers for UnityEvents.
- A stranded vehicle now verifies the entered SafeZone is its assigned destination.
- A SANID silver expedition visual prototype has a body plus four separate wheel meshes with provisional axle pivots.
- The prototype FBX and its four material maps are under Assets/SandRescue/Art/Vehicles/Vehicle01.
- A Tools/SAND RESCUE menu command exists to create starter mission assets when run inside Unity; those ScriptableObject assets have not been created here.
- A separate Create First Drive Test Scene command generated the validated FirstDrive 4 scene, prefab, materials and flat test environment inside Unity. The successful output is tracked under Assets/SandRescue/Generated/FirstDrive 4.
- FirstDriveHUD provides a desktop-only smoke-test overlay, not the production/mobile/localized UI.
- The redundant standalone TextMeshPro package request and the handcrafted incomplete dependency lock were removed. Unity will resolve and regenerate the lock on import.
- Full FBX forward/up alignment was corrected after the user reported a sideways vehicle and Wheel_RL radius error.
- Scene-level failure recovery now keeps original scenes open until setup saves successfully; pre-save failures attempt to close only the incomplete temporary scene and restore prior pipeline assignments. It does not delete partial generated assets or restore texture importer settings.

## Actual checks performed

- Blender import, export and reimport of the separated-wheel FBX.
- Five mesh objects, expected wheel names and provisional anchors verified.
- Original 13,457-triangle total preserved after separation.
- Actual model preview inspected.
- Targeted source checks and git diff --check.
- A C# grammar parser checked 37 source files without syntax errors; this is not a Unity API/compiler check.
- The user's first setup attempt stopped with Unexpected imported wheel radius for Wheel_RL: 0.1965754. After the alignment correction and active-scene fix, the user successfully generated the FirstDrive 4 scene in Unity.
- In the user's Play Mode smoke test, the vehicle stayed upright, W drove toward the visible front, A/D steered correctly, and no red Console errors appeared. Recovery with R still needs a focused test while the vehicle is tipped or stuck.

## Not completed or tested

- Unity Play Mode vehicle driving has passed its initial forward/steering smoke test; recovery, safe-zone events and winch dynamics remain untested or incomplete.
- A screenshot-level inspection of the generated scene and exact wheel alignment remains useful before mission assembly.
- Completed scenes/prefabs, URP pipeline asset or saved Inspector assignments.
- Exact SANID decal replacement, wheel rotation appearance, final collision hulls or LODs.
- Unity Localization String Tables, full Arabic RTL layout or Arabic shaping.
- Persistent coins/XP, save system, garage or upgrades (Step 3 remains deferred).
- Android/iOS export, profiling or physical-device tests.

Previous balanced-brace and syntax checks were not Unity compilation. The next validation gate is the first rescue mission loop: create the starter mission assets, place a stranded target and safe zone, then test winch attachment and tow completion in the generated scene. Preserve all Unity-generated local changes when pulling code; do not discard them to avoid a conflict.

See FIRST_DRIVE_GUIDE.md for the first-drive command. Docs/Vehicle01/START_HERE.md and STEP2_SETUP.md contain model and mission setup notes.

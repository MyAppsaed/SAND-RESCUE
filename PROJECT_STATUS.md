# SAND RESCUE Project Status

The repository is a Unity 6.0 project scaffold and has been published to GitHub. The user installed and opened Unity 6.0 LTS 6000.0.85f2 on Windows. ProjectVersion.txt selects that editor without an invented revision hash; Unity writes its exact revision on import. This sandbox still has no Unity Editor or access to the user's local metadata and settings.

## Current source and art

- Step 1 vehicle foundation and Step 2 gameplay scripts exist under Assets/SandRescue.
- The input-state struct assignment compile blocker was corrected at source level.
- Camera touch input now uses the New Input System rather than legacy Input.touches.
- Mission and winch actions have void-returning handlers for UnityEvents.
- A stranded vehicle now verifies the entered SafeZone is its assigned destination.
- A SANID silver expedition visual prototype has a body plus four separate wheel meshes with provisional axle pivots.
- The prototype FBX and its four material maps are under Assets/SandRescue/Art/Vehicles/Vehicle01.
- A Tools/SAND RESCUE menu command exists to create starter mission assets when run inside Unity; those ScriptableObject assets have not been created here.
- A separate Create First Drive Test Scene command is prepared to generate the driving rig, camera, URP assets if needed, prefab and flat test scene inside Unity. It has not been run here.
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
- The user's screenshot showed no Console errors before running the first-drive command, and the command executed on the user's machine. Setup then stopped with Unexpected imported wheel radius for Wheel_RL: 0.1965754. This was a setup failure, not a passing drive test.
- After the alignment correction, numerical tests using actual mesh coordinates passed 15 rotation/scale cases with corrected wheel radii approximately 0.45 m. Unity verification is still pending.

## Not completed or tested

- Repeat Unity compilation/import after the latest patches; the user's local generated metadata and resolved package lock are not yet captured in this sandbox.
- The corrected first-drive generation and scene failure recovery in Unity.
- Unity Play Mode, vehicle driving, safe-zone events or winch dynamics.
- Completed scenes/prefabs, URP pipeline asset or saved Inspector assignments.
- Exact SANID decal replacement, wheel rotation appearance, final collision hulls or LODs.
- Unity Localization String Tables, full Arabic RTL layout or Arabic shaping.
- Persistent coins/XP, save system, garage or upgrades (Step 3 remains deferred).
- Android/iOS export, profiling or physical-device tests.

Previous balanced-brace and syntax checks were not Unity compilation. The next validation gate is rerunning the corrected setup in the user's actual project and inspecting the Console before driving. Preserve all Unity-generated local changes when pulling code; do not discard them to avoid a conflict.

See FIRST_DRIVE_GUIDE.md for the first-drive command. Docs/Vehicle01/START_HERE.md and STEP2_SETUP.md contain model and mission setup notes.

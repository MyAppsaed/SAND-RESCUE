# SAND RESCUE Project Status

The repository is a Unity 6.0 project scaffold and has been published to GitHub. The local user is installing Unity 6.0 LTS 6000.0.85f2. ProjectVersion.txt now selects that editor without an invented revision hash; Unity will write its own exact revision on import.

## Current source and art

- Step 1 vehicle foundation and Step 2 gameplay scripts exist under Assets/SandRescue.
- The input-state struct assignment compile blocker was corrected at source level.
- Camera touch input now uses the New Input System rather than legacy Input.touches.
- Mission and winch actions have void-returning handlers for UnityEvents.
- A stranded vehicle now verifies the entered SafeZone is its assigned destination.
- A SANID silver expedition visual prototype has a body plus four separate wheel meshes with provisional axle pivots.
- The prototype FBX and its four material maps are under Assets/SandRescue/Art/Vehicles/Vehicle01.
- A Tools/SAND RESCUE menu command exists to create starter mission assets when run inside Unity; those ScriptableObject assets have not been created here.

## Actual checks performed

- Blender import, export and reimport of the separated-wheel FBX.
- Five mesh objects, expected wheel names and provisional anchors verified.
- Original 13,457-triangle total preserved after separation.
- Actual model preview inspected.
- Targeted source checks and git diff --check.

## Not completed or tested

- Unity Package Manager resolution or Unity Editor compilation.
- Unity Play Mode, vehicle driving, safe-zone events or winch dynamics.
- Completed scenes/prefabs, URP pipeline asset or saved Inspector assignments.
- Exact SANID decal replacement, wheel rotation appearance, final collision hulls or LODs.
- Unity Localization String Tables, full Arabic RTL layout or Arabic shaping.
- Persistent coins/XP, save system, garage or upgrades (Step 3 remains deferred).
- Android/iOS export, profiling or physical-device tests.

Previous balanced-brace checks were only text sanity checks, not compilation. Opening the project in Unity is the next validation gate; stop on Console or package errors and fix them before testing the first mission.

See Docs/Vehicle01/START_HERE.md and STEP2_SETUP.md for setup notes.

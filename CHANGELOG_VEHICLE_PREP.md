# Vehicle Preparation — 2026-11-09

## Added

- Assets/SandRescue/Art/Vehicles/Vehicle01/SANID_Vehicle01_SeparateWheels.fbx.
- Four named URP-oriented material maps in that folder's Textures subdirectory.
- Docs/Vehicle01/START_HERE.md and SEPARATION_REPORT.json.

## Modified

- VehicleInputManager.cs: update a local VehicleInputState before assigning Current; prevent zero smoothing division; consume mobile one-shot flags; sample input before ordinary vehicle Update methods.
- VehicleCameraController.cs: New Input System touch API, cached input reference, bounded drag yaw and return-on-release.
- Gameplay/MissionManager.cs: void StartAssignedMission button handler plus bool TryStartAssignedMission for code callers.
- Winch/RescueWinch.cs: void Attach and AttachNearestTarget handlers; detector reference; avoid duplicate attach reporting for the same connected target.
- Targets/StrandedVehicle.cs: compare the entered SafeZone to the configured destination.
- Gameplay/SafeZone.cs: report ReachSafeZone only for the player controller.
- ProjectSettings/ProjectVersion.txt: align with user-selected 6000.0.85f2; remove unverified old revision hash.
- PROJECT_STATUS.md and STEP2_SETUP.md: correct current status and UI hookup instructions.

## Validation

The separated FBX imported, exported and reimported in Blender 4.0.2. The five mesh objects and three markers were found; total triangles remained 13,457. The four wheel origin locations were computed from the tire-region bounding boxes. Actual assembled and separated-wheel renders were inspected.

The source changes were checked for their targeted error patterns and whitespace errors. No Unity installation is available in this sandbox: no Unity compile, Editor scene test, device test or frame-rate result is claimed.

## Limits

The generated logo still needs replacing using the original artwork. The vehicle is not a completed driving prefab. Stable wheel animation needs Unity-axis verification; anchors, dimensions, mass and suspension need fitting. Step 3 and Step 4 remain deferred until Step 2's actual Unity validation succeeds.

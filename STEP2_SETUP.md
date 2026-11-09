# SAND RESCUE — Step 2 Setup and Validation

Step 2 adds source foundations for data-driven missions, rescue objectives, towing, checkpoints, safe zones, reward calculation, and gameplay flow. The vehicle controller remains in place. The subsequent vehicle-preparation pass applies targeted input/camera bug fixes and adds a separate-wheel visual prototype; see CHANGELOG_VEHICLE_PREP.md. This is not yet a Unity-validated completed gameplay loop.

## Scene hierarchy

```text
DesertMission
├── MissionSystems
│   ├── MissionManager
│   ├── GameFlowManager
│   └── CheckpointManager
├── RescueVehicle
│   ├── SandRescueVehicleController        (existing Step 1)
│   ├── RescueWinch
│   ├── RescueTargetDetector
│   └── WinchCable
├── StrandedVehicle
│   ├── TowableVehicle
│   ├── StrandedVehicle
│   └── RescueConnectionPoint
├── SafeZone
├── MissionTriggers
└── Environment
```

## Inspector assignments

On `MissionManager`, assign the player `VehicleStatus` and `CheckpointManager`. Create one `RescueMissionData` asset per mission and assign it at runtime through `StartMission`. Each objective needs a stable `objectiveId`, `MissionObjectiveType`, localized title data, `requiredAmount`, and a matching `targetId`.

On `RescueWinch`, assign the vehicle Rigidbody, front or rear connection transform, `WinchCableRenderer`, and tune maximum length, reel speed, winch force, and break force. On the cable object, add a `LineRenderer` and assign it to `WinchCableRenderer`. On each stranded vehicle, add `Rigidbody`, `TowableVehicle`, `StrandedVehicle`, and one or more `RescueConnectionPoint` components.

A `MissionTrigger` reports progress when the player Rigidbody enters it. A `SafeZone` is a trigger collider and should be used as the destination for tow missions. A `CheckpointManager` registers checkpoint transforms through `Register(Transform)` from checkpoint trigger events or a small UI/scene event binding.

For a mission-start UI button, bind the void `MissionManager.StartAssignedMission()` method. Code callers can use the bool `TryStartAssignedMission()` method instead. For attach UI, assign `RescueWinch.targetDetector` and bind the void `AttachNearestTarget()` method. A fixed-target test can bind the void `Attach(RescueConnectionPoint)` method with the target point in its argument slot. Do not bind the bool-returning `TryAttach` method to Button.onClick. The first mission's attachment point ID must be `stranded_car_front`; the rescued vehicle target ID must be `stranded_car_01`.

On `StrandedVehicle`, assign its `Destination` to the exact SafeZone component. Entering that zone while rescue is in progress reports its TowTarget objective. The SafeZone separately reports ReachSafeZone when the player-controller Rigidbody enters, not when unrelated physics props enter. Reward calculation emits an event; persistent coins/experience storage is not implemented yet.

## Five starter mission assets

Create these with the Unity Editor menu or manually:

1. `mission_first_rescue`: attach winch, tow `stranded_car_01`, reach `safe_zone_01`; no timer.
2. `mission_fuel_emergency`: reach `fuel_target_01`, then deliver fuel; add a time limit.
3. `mission_dune_recovery`: reach and tow `stranded_suv_01` across dune terrain.
4. `mission_sandstorm_search`: search `search_area_01`, rescue `traveler_01`, reach safety.
5. `mission_heavy_rescue`: use a heavy-class vehicle and tow `heavy_vehicle_01`; configure a stronger winch requirement in the vehicle data layer.

Use `BilingualText.key` values such as `mission_first_rescue`, `objective_attach_winch`, and `objective_tow_to_safety`. The fallback fields are Arabic and English data, not hardcoded C# UI text. In a project with the Unity Localization package installed, map these stable keys to Arabic and English String Tables.

## Runtime flow

Call `MissionManager.StartMission(missionAsset)` from mission selection. `MissionTrigger`, rescue targets, and safe-zone events report progress using stable IDs. When all required objectives complete, `MissionManager` calculates 1–3 stars from completion, time, and vehicle health, raises `MissionCompleted`, and guards rewards with `RewardsIssued`. Call `Fail("mission_time_expired")` for failure. `GameFlowManager` provides restart, abandon, main-menu, and mission-scene entry points.

## Validation status

The scripts were checked for balanced braces, control characters, and null-guarded references in this sandbox. Unity Editor compilation, physics behavior, Arabic shaping, localization package table wiring, and physical Android/iOS testing require a Unity project/editor and devices; they have not been claimed as completed here.

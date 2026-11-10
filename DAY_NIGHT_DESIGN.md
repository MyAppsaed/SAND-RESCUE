# SAND RESCUE — Day and Night Modes

The user requested day and night modes on 2026-11-10. The current first-drive scene has daytime-style lighting only. This document records the feature direction; night lighting and headlights have not been implemented or tested.

## Recommended first version

Offer Day and Night as a simple choice before a mission, or let a mission specify its lighting preset. Keep that preset fixed during the first implementation. A continuous sunrise/sunset cycle can be added later if desired; it is not required to deliver two playable modes.

Day should use clear desert daylight, readable terrain and a bright sky. Night should use a dark sky and restrained ambient/moon illumination, with vehicle headlights and rescue-light presentation. Night must remain playable: nearby terrain, tow connection points, cable and important mission markers need to remain visible. Avoid making the whole scene pitch black or using excessive glare.

Headlights should have a player control and a clear on/off state. Beam lights and emissive lens artwork serve different purposes: making the lens glow is not a substitute for illuminating the road. Rescue beacons are a presentation feature, not a requirement to finish the first driving test.

## Keep it modular

Store lighting presets separately from vehicle physics, input and mission-reward logic. A lighting component should apply the selected scene preset without modifying traction, torque or objective completion. Headlight control belongs to each vehicle's visual/light equipment, with a clear input request rather than a direct dependency on one UI button.

When mission selection and local saves are built, store the selected preset as a stable value so a restarted mission uses the same lighting. Both modes must work without a login, game server, network time request or online asset download.

For future multiplayer, the session authority selects the common Day/Night state, and joining players receive it. If a continuous cycle is added later, synchronize its authoritative time/phase rather than advancing unrelated clocks on each client. Shared vehicle-light state should also have defined ownership. Local brightness/accessibility settings must not change authoritative mission state.

## Build and test order

First finish the corrected daytime driving and rescue loop. Then add the night preset and working headlights to that same scene, keeping the existing offline behavior intact. Validate terrain readability, camera visibility, tow-cable visibility and mission markers in both modes. Finally profile lighting and shadows on the intended Android devices and reduce costs where necessary; no mobile performance result is claimed now.

Accounts, matchmaking, weather simulation, an automatic day/night cycle and a new networking package are outside this small first implementation. The immediate goal is two clear, playable lighting modes, not an overcomplicated environment system.

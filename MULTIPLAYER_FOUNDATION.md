# SAND RESCUE — Offline First, Multiplayer Next

## Agreed direction

On 2026-11-10, the user confirmed that the current game should remain offline and single-player, with online multiplayer planned for the next stage. Build a strong foundation now without adding a login requirement, network service dependency or hosting bill to the offline version.

This is an architecture direction, not a claim that the current prototype is already multiplayer-ready. No networking framework has been selected or installed, and no multiplayer session, synchronization or server has been implemented. The first-drive setup still needs validation in the user's actual Unity project.

## Boundaries to establish while completing the offline game

**Input is a request, not permission to change the world.** Keyboard, gamepad and mobile controls should produce a compact vehicle command. Simulation consumes that command through a narrow input-source boundary instead of querying a specific keyboard or UI button. Offline play supplies local commands; a later network adapter can supply commands from an authorized player. Throttle, steering, handbrake and recovery are gameplay actions; camera look-back is local presentation and should not be replicated just because it shares today's input struct.

**Gameplay decisions have one authority.** Offline, local simulation decides whether a winch can attach, an objective completes, damage is applied or a reward is earned. In online mode, designate a host or server authority for shared gameplay. A client may request an action; it must not directly declare that an objective passed or award itself online currency. Keep the same validation rules where practical so offline and online do not become two different games.

**Shared vehicle and towing physics need a deliberate owner.** A vehicle and its connected winch/target cannot be simulated independently by several peers and expected to agree. The online design must decide who simulates them, how ownership changes, and what happens if a driver disconnects. Prototype vehicle-plus-winch synchronization before committing to a networking framework. Do not assume FixedUpdate or WheelCollider makes separate machines' physics deterministic. Client prediction, reconciliation and remote interpolation are future work, not solved by the present input seam.

**World entities and actions need stable identities.** Keep existing missionId, objectiveId, targetId, pointId and zoneId values stable. Add separate runtime entity/player identities when dynamic spawns and sessions are introduced; definition IDs alone cannot distinguish several instances of the same vehicle. Avoid treating object names, hierarchy positions or local Unity instance IDs as persistent cross-session identities.

**Authority checks include the actor.** Validate the requesting player, target, distance, vehicle capability, mission state and ownership before accepting rescue actions. Record accepted action identifiers where retries might otherwise duplicate objective progress or rewards. UI clicks, repeated trigger entries and later network retries must not each create a separate valid rescue result by accident.

**Presentation observes state.** Cameras, wheel artwork, sound, particles, mission markers and UI should display simulation results rather than independently decide mission outcomes. Remote players' vehicles should not read the local keyboard or receive the local follow camera. Separate event delivery from state mutation so networking does not require rewriting every HUD widget.

**Saves are versioned and separated from live world objects.** When the offline save system is implemented, save stable IDs and plain progress data with a schema version and migration policy. Do not serialize live MonoBehaviour references as the progression format. Offline saves may be player-editable; they are not trusted evidence for future competitive rewards, leaderboards or an online economy. Decide explicitly whether any offline progression can transfer online, and validate that transfer rather than silently trusting it.

## What the current source already provides

VehicleInputState is a useful small input container. VehicleWheel separates visual wheel transforms from WheelCollider physics. Mission definitions already have stable string IDs, and MissionManager emits objective and result events. These are helpful starting points, not a complete authority or network abstraction.

The gaps are concrete: SandRescueVehicleController currently references the concrete VehicleInputManager; MissionManager.Active is a process-wide singleton; objective reporting accepts an actor argument but does not yet validate its ownership; winch actions directly mutate local target state. Replace these couplings in small, testable steps as the offline loop becomes reliable. Do not perform a speculative full rewrite while the first-drive scene is still being debugged.

## Implementation order

First, finish and validate offline driving, wheel alignment, recovery, winch attachment and the first rescue mission. Then introduce the input-source and action-validation seams, without changing the offline controls. Add automated tests for repeated rescue requests, reward issuance and ownership rejection as those services become available.

Next, implement versioned local saves and stable runtime identities where the offline content requires them. After that, build a small networking experiment with two vehicle instances and one towable target to test authority transfer, latency, duplicate requests and disconnections. Only then choose a supported networking framework and service model for the intended player count and platforms.

The two-vehicle experiment is a validation tool, not an agreed public player limit. Co-op versus competitive play, maximum players per session, matchmaking, accounts, voice/chat, hosting region and backend costs remain product decisions for the online stage.

## Release gates

The offline build must launch and support its core rescue loop and local save/load without a network connection. The eventual online mode must verify authorized actions, prevent duplicate reward application, preserve consistent vehicle/tow state and handle connection loss. A disconnect must not silently turn a shared online session into independent offline worlds; any fallback behavior needs an explicit design.

No server deployment, paid online service, public multiplayer release or replacement of the user's local Unity files is authorized merely by this document. Keep the current version playable offline while using these boundaries to reduce future migration work.

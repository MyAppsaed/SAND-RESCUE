# First SANID Driving Test

While Unity installs, the project now includes an Editor menu command designed to create a first-drive test scene from the existing SANID model. The command source has been prepared outside Unity. The scene and prefab will be created on your computer only when you run it inside the Editor; they have not already been built or tested here.

## When Unity is ready

1. In GitHub Desktop, select SAND-RESCUE, click Fetch origin, then Pull origin if offered. Do not discard local changes to get an update; report any conflict instead.
2. In Unity Hub, add the cloned repository folder containing Assets, Packages and ProjectSettings. Open it with Unity 6.0 LTS 6000.0.85f2. Do not create a separate new project.
3. Let the first package resolution and asset import finish. If Unity asks to enable the New Input System, allow it and restart the Editor if requested. Otherwise set Edit > Project Settings > Player > Other Settings > Active Input Handling to Input System Package (New). Clear actual package/compiler errors before continuing.
4. Select Tools > SAND RESCUE > Create First Drive Test Scene. Read its local setup dialog. Save any modified open scene when prompted, or cancel to keep working there.
5. When the new scene opens, press Play and click inside the Game view before using the keyboard.

If the menu is missing, check the Console for compiler errors first. The setup command cannot run until its source and dependencies compile in Unity.

## What the command is designed to create

It creates a separate folder under Assets/SandRescue/Generated/FirstDrive. A repeated run selects a new unique folder rather than replacing an earlier generated scene or prefab.

The scene uses the silver SANID expedition model with the existing vehicle controller, input manager, surface detector and recovery component. It aligns the vehicle's nose to the controller's forward direction using the model's front/rear markers. It adds a Rigidbody, a provisional body box collider and four WheelColliders. Tire radii use the imported vertical extent, avoiding tread-thickness assumptions. A test-only runtime filter explicitly ignores collision pairs between the hull and its wheels and between wheel pairs; the Ignore Raycast layer is used for sensor exclusion, not as a claim that physics collisions are disabled. Wheel visual wrappers preserve the model's authored orientation while the wheel physics supplies position, steering and rotation.

The environment is deliberately simple: a flat 120-metre test surface, six lane markers, a directional light and a follow camera. A desktop-only overlay shows speed in km/h and the controls. This is a driving smoke test, not a detailed desert map or a rescue mission.

If no pipeline is configured, the command creates URP pipeline/renderer assets and assigns them as the default/current-quality pipeline. It does not deliberately replace a configured non-URP pipeline; that case stops with an explanation. It also sets the four vehicle textures' import types, color-space settings and initial maximum size to 1024. It does not change the source image pixels. Generated materials are assigned to the scene copy of the model, not remapped into the original FBX.

## Controls

Use W or Up Arrow to drive forward, S or Down Arrow to reverse, and A/D or Left/Right Arrow to steer. Space applies the handbrake. R invokes vehicle recovery, and C briefly looks behind. The controller's reverse input is not a production automatic-gearbox/braking model. Stop Play Mode and press Play again to restart the smoke test cleanly.

## First checks

Check one thing at a time. First confirm the vehicle settles on its wheels without falling through the ground. Next confirm W moves it toward its visible nose, and that A/D steer the front wheels without turning the wheels sideways. Finally test a brief low-speed turn, handbrake and recovery.

Send a screenshot of the Console if any red error appears. If the model slides, tips excessively or the wheels rotate incorrectly, stop and share a screenshot of the vehicle hierarchy and wheel Inspector. Wheel radius, collider hull, mass, suspension and camera framing are initial tuning values, not verified real-vehicle specifications.

## Package cleanup for first import

The manifest no longer requests the standalone TextMeshPro 3.0.6 package because Unity UI 2.0 already includes TextMeshPro. The original handcrafted partial packages-lock.json was removed. Unity Package Manager must resolve the installed versions and write a real lockfile on your first import; commit that generated lock with your Unity metadata after successful resolution. URP 17.0.3 and Input System 1.11.2 were left pinned rather than speculatively upgraded.

## Validation and limits

A C# grammar parser found no syntax errors across the 37 project source files at the preparation check. Serialized field names were compared to their source declarations, and git diff --check was run. These checks are not a Unity compile, dependency-resolution result or physics test. No Unity Editor is installed in this sandbox.

The test does not include winch gameplay, mission UI, a garage, persistent currency, audio, mobile touch buttons, final Arabic localization or device profiling. The original SANID artwork still needs an exact-decal pass, and generated unseen geometry and wheel appearance still need cleanup. Passing this first driving test is the next validation gate, not proof that the full game is complete.

## Technical references

The wheel-collider placement uses Unity's recommended half-suspension upward offset [1]. The URP setup uses the documented UniversalRenderPipelineAsset.Create API [2]. The package decision follows the official Unity UI 2.0 changelog [3]. Unity maintains the resolved package graph in its generated lockfile [4]. The current-scene save prompt uses the documented EditorSceneManager method [5].

**References**

[1]: https://docs.unity3d.com/6000.0/Documentation/Manual/WheelColliderTutorial.html "Unity 6 WheelCollider tutorial"
[2]: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/api/UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset.html "URP 17 UniversalRenderPipelineAsset API"
[3]: https://docs.unity3d.com/Packages/com.unity.ugui@2.0/changelog/CHANGELOG.html "Unity UI 2.0 TextMeshPro merge changelog"
[4]: https://docs.unity3d.com/6000.0/Documentation/Manual/upm-conflicts-auto.html "Unity Package Manager lockfiles"
[5]: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo.html "Unity Editor save-current-scenes prompt"

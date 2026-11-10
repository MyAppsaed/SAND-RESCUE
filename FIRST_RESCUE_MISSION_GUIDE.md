# First Rescue Mission Test

This is the first playable rescue loop built on top of the validated SANID driving scene. It is a starter mission slice, not the final production UI or art.

## Generate the scene

1. Stop Play Mode and save any open scene.
2. Wait for Unity to finish compiling the new scripts.
3. Select **Tools > SAND RESCUE > Create First Rescue Mission Scene**.
4. Click **Create** in the confirmation dialog.
5. Unity opens and saves a new scene under `Assets/SandRescue/Generated/Missions/FirstRescue` or a numbered unique folder if an earlier attempt exists.

The command starts from the validated `SANID_FirstDrive` scene and does not overwrite it.

## Play the mission

1. Open the newly created `FirstRescue.unity` scene.
2. Confirm the Console has no red errors.
3. Press **Play** and click the Game view.
4. Drive toward the placeholder stranded vehicle.
5. When the HUD says the target is near, press **E** to attach the winch.
6. Drive toward the green **Safe Zone** cylinder. The cable applies tow force to the stranded vehicle.
7. Use **Q** to detach. Hold **T** to reel in and **Y** to reel out.
8. When the target enters its assigned safe zone, the tow objective should complete and the HUD should show **MISSION COMPLETE**.

## Expected objectives

- `objective_attach_winch`: press E close to the target's front connection point.
- `objective_tow_to_safety`: tow `stranded_car_01` into `safe_zone_01`.

## Limits of this test

The target uses placeholder geometry. The mission auto-starts for testing, the controls are desktop-only, and the HUD is not the final localized interface. Do not treat this as the finished mission system until winch attachment, cable behavior, safe-zone completion, reward events and restart behavior have all been tested in Unity.

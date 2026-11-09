# SANID Vehicle 1 — Separate-Wheel Prototype

This supersedes the earlier combined-mesh prototype for future driving integration. The original combined model remains unchanged. This is still a prototype visual asset, not a tested driving prefab.

## Completed

- Separated the existing mesh into Body, Wheel_FL, Wheel_FR, Wheel_RL and Wheel_RR.
- Included tire and nearby hub geometry in each wheel object; moved each wheel origin to the estimated axle centre.
- Retained source material references and UVs.
- Preserved the source triangle count: 13,457 in total across five mesh objects.
- Added FrontWinchAnchor, RearTowAnchor and CenterOfMassSuggestion as provisional empty transforms.
- Exported a new FBX and editable Blender file.
- Reimported the FBX into Blender and verified all five mesh names, the three markers and the 13,457-triangle count.
- Rendered both an assembled preview and an exploded-wheel preview of the actual geometry.

The exploded preview intentionally moves wheels away from the body to demonstrate separation. The saved FBX and Blender asset have wheels in their assembled positions, not in the exploded positions.

## Files

- SANID_Vehicle01_SeparateWheels.fbx — import this into Unity.
- SANID_Vehicle01_SeparateWheels.blend — editable source, keep outside Unity Assets unless you intend to use Blender's importer.
- Textures — four URP-oriented texture maps.
- Vehicle01_Assembled.png — actual mesh in normal position.
- Vehicle01_SeparatedWheel_Preview.png — demonstration preview, not the saved asset pose.
- SEPARATION_REPORT.json — per-mesh counts, pivot coordinates, estimated tire radii and limits.
- separate_vehicle_wheels.py — reproducible separation script; operates on the existing prototype and saves to a separate output directory.
- SANIDLOGO.pdf — supplied original logo source for a later exact-decal pass, included in the downloadable archive only.

## Unity import

If you pull the latest GitHub update, the FBX and four textures will already be under:

Assets/SandRescue/Art/Vehicles/Vehicle01

Otherwise copy only the FBX and Textures folder to that directory. Do not replace your repository with this archive. Avoid importing the .blend and FBX together because that can duplicate the vehicle.

Create a URP Lit material, assuming the project already has an active URP pipeline:

- Base Map: SANID_01_BaseColor.png; sRGB enabled.
- Normal Map: SANID_01_Normal.png; import as Normal map.
- Metallic Map: SANID_01_MetallicSmoothness.png; sRGB disabled, preserve input alpha, Smoothness Source Metallic Alpha.
- Occlusion: SANID_01_Occlusion.png; sRGB disabled.

If Unity shows pink materials, configure URP before diagnosing the model. A URP package declaration alone does not create a pipeline asset.

## Future vehicle hierarchy

Create a physics root facing local +Z with unit scale. The art is a visual child, not the physics root. Expand the FBX to see its body and four wheel objects. Wheel names are from the vehicle's perspective, not the camera's perspective.

- Keep the WheelCollider GameObjects separate from the rotating visual wheels.
- Assign each separated Wheel_FL/FR/RL/RR transform to its corresponding VehicleWheel.visualWheel field.
- The estimated tire radius is approximately 0.45 m, but measure it in the imported scene and verify suspension placement before use.
- The source mesh faces Blender +X and has +Z up. Its FBX is exported -Z forward and Y up. Check actual Unity facing and each wheel's local rotation; the front anchor identifies the nose. Align the visual child and axle axes before attaching it to a controller. Do not blindly copy Blender coordinates into Unity.
- Use the markers as initial positions for the winch, rear towing connection and centre of mass. They are provisional and need adjustment around the collision hull and suspension.
- The geometry has slight generated asymmetries. Pivot centres are based on tire bounding boxes, not measured chassis engineering.

## Remaining work

Unity compilation, import-material appearance, WheelCollider driving, winch physics and Android/iOS performance are not tested. The generated lettering remains imperfect: the supplied original SANID artwork must replace it as a proper decal. Existing manufacturer-like grille details, unseen geometry and wheel shapes still need visual cleanup. There are no colliders, Rigidbody, LODs, driving scripts, mission prefab or completed interior in this asset.

## First-mission source fixes

The accompanying GitHub update corrects the struct-property compile error in VehicleInputManager, removes legacy camera touch reads, provides void UI handlers for StartAssignedMission and winch AttachNearestTarget/Attach, and corrects destination SafeZone detection for towed targets.

For a temporary mission start button, bind MissionManager.StartAssignedMission(). For a contextual attach button, assign RescueWinch.targetDetector and bind RescueWinch.AttachNearestTarget(). For a fixed reference test, bind RescueWinch.Attach(RescueConnectionPoint) and set the desired connection point. TryAttach remains the bool-returning code API, not a Button.onClick callback.

These fixes were inspected at source level. They are not a successful Unity compile or Play Mode test. Step 3 UI/progression and Step 4 release work remain deferred.

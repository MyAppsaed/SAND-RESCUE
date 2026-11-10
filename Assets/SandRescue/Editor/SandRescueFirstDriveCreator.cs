#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace SandRescue.Editor
{
    /// <summary>Explicit, local Editor setup. No startup hook and no existing scene overwrite.</summary>
    public static class SandRescueFirstDriveCreator
    {
        private const string VehicleFolder = "Assets/SandRescue/Art/Vehicles/Vehicle01";
        private const string ModelPath = VehicleFolder + "/SANID_Vehicle01_SeparateWheels.fbx";
        private const float Suspension = 0.25f;
        private const int VehicleLayer = 2; // Existing Ignore Raycast layer, not a new project layer.

        [MenuItem("Tools/SAND RESCUE/Create First Drive Test Scene")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                return;
#if !ENABLE_INPUT_SYSTEM
            EditorUtility.DisplayDialog("SAND RESCUE — Input setup needed",
                "Enable Input System Package (New) under Edit > Project Settings > Player > Other Settings > Active Input Handling, restart the Editor if prompted, then run this command again.", "OK");
            return;
#else
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            Shader lit = Shader.Find("Universal Render Pipeline/Lit");
            if (model == null || lit == null)
            {
                EditorUtility.DisplayDialog("SAND RESCUE — Missing dependency",
                    "The SANID FBX and URP Lit shader must finish importing before creating this scene. Resolve Package Manager and Console errors first.", "OK");
                return;
            }
            string[] wheelNames = { "Wheel_FL", "Wheel_FR", "Wheel_RL", "Wheel_RR" };
            foreach (string name in wheelNames)
                if (Find(model.transform, name) == null)
                {
                    EditorUtility.DisplayDialog("SAND RESCUE", "The imported model is missing " + name + ". Pull the separated-wheel update before continuing.", "OK");
                    return;
                }
            if (Find(model.transform, "Body") == null || Find(model.transform, "FrontWinchAnchor") == null || Find(model.transform, "RearTowAnchor") == null || Find(model.transform, "CenterOfMassSuggestion") == null)
            {
                EditorUtility.DisplayDialog("SAND RESCUE", "The model is missing its body or orientation markers, including CenterOfMassSuggestion.", "OK");
                return;
            }
            if (GraphicsSettings.currentRenderPipeline != null && !(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset))
            {
                EditorUtility.DisplayDialog("SAND RESCUE", "This test uses URP. Your existing non-URP render pipeline will not be replaced. Configure a URP project before running this command.", "OK");
                return;
            }
            if (!EditorUtility.DisplayDialog("Create SANID First Drive",
                "This creates a NEW test scene, prefab and materials. Existing generated files are not overwritten. It updates the four vehicle texture import settings. If no pipeline is active, it also creates URP assets and assigns the default/current-quality pipeline. Save changes in open scenes when prompted. This is an untested setup aid, not a completed mission.", "Create", "Cancel"))
                return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            // Keep the original scenes alive until the complete test has been saved.
            Scene previousActive = SceneManager.GetActiveScene();
            Scene[] previousScenes = new Scene[SceneManager.sceneCount];
            for (int i = 0; i < previousScenes.Length; i++) previousScenes[i] = SceneManager.GetSceneAt(i);
            UnityEngine.Object previousSelection = Selection.activeObject;
            RenderPipelineAsset previousDefaultPipeline = GraphicsSettings.defaultRenderPipeline;
            RenderPipelineAsset previousQualityPipeline = QualitySettings.renderPipeline;
            Scene testScene = default(Scene);
            bool sceneSaved = false;

            try
            {
                EnsureFolder("Assets/SandRescue/Generated");
                string folder = AssetDatabase.GenerateUniqueAssetPath("Assets/SandRescue/Generated/FirstDrive");
                EnsureFolder(folder);
                EnsurePipeline(folder);
                Material paint = CreateVehicleMaterial(folder, lit);
                Material sand = CreateMaterial(folder + "/MAT_TestSand.mat", lit, new Color(0.61f, 0.48f, 0.29f));
                Material marker = CreateMaterial(folder + "/MAT_TestMarker.mat", lit, new Color(0.95f, 0.43f, 0.1f));
                testScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                if (!SceneManager.SetActiveScene(testScene))
                    throw new InvalidOperationException("Unity could not activate the temporary test scene.");
                CreateEnvironment(sand, marker);
                GameObject car = CreateVehicle(model, paint, wheelNames);
                CreateCamera(car);
                GameObject hud = new GameObject("First Drive HUD — Desktop Test Only");
                FirstDriveHUD display = hud.AddComponent<FirstDriveHUD>();
                Reference(display, "vehicle", car.GetComponent<SandRescueVehicleController>());

                // This prefab is a first-test starting point, not a production vehicle release.
                if (PrefabUtility.SaveAsPrefabAsset(car, folder + "/SANID_Vehicle01_FirstDrive.prefab") == null)
                    throw new InvalidOperationException("Unity did not save the vehicle prefab.");
                string scenePath = folder + "/SANID_FirstDrive.unity";
                if (!EditorSceneManager.SaveScene(testScene, scenePath))
                    throw new InvalidOperationException("Unity did not save the test scene.");
                sceneSaved = true;
                AssetDatabase.SaveAssets();
                // The user already answered the save prompt. Close originals only now,
                // so old physics/cameras cannot run alongside a successful driving test.
                foreach (Scene previous in previousScenes)
                    if (previous.IsValid() && previous.isLoaded && !EditorSceneManager.CloseScene(previous, true))
                        throw new InvalidOperationException("A previous scene could not be closed. Do not enter Play Mode with multiple test scenes loaded.");
                Selection.activeGameObject = car;
                if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
                EditorUtility.DisplayDialog("First Drive setup created",
                    "Saved: " + scenePath + "\n\nPress Play, click the Game view, then use W/S, A/D, Space, R and C. Verify forward travel and wheel alignment first. Stop on Console errors. This command has not been run in Unity by the builder.", "OK");
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                string recoveryNote = "A new scene was already saved. Check the Console and open-scene list before entering Play Mode.";
                if (!sceneSaved)
                {
                    try
                    {
                        GraphicsSettings.defaultRenderPipeline = previousDefaultPipeline;
                        QualitySettings.renderPipeline = previousQualityPipeline;
                        if (previousActive.IsValid() && previousActive.isLoaded)
                            SceneManager.SetActiveScene(previousActive);
                        if (testScene.IsValid() && testScene.isLoaded && !EditorSceneManager.CloseScene(testScene, true))
                            throw new InvalidOperationException("Unity could not close the incomplete temporary scene.");
                        Selection.activeObject = previousSelection;
                        recoveryNote = "The incomplete temporary scene was closed. Your original open scenes were kept, and prior pipeline assignments were restored.";
                    }
                    catch (Exception recoveryError)
                    {
                        Debug.LogException(recoveryError);
                        recoveryNote = "Automatic cleanup could not finish. Do not enter Play Mode; check the Console before closing or saving any scene.";
                    }
                }
                EditorUtility.DisplayDialog("First Drive setup stopped",
                    "Setup failed: " + error.Message + "\n\n" + recoveryNote + "\nPartial generated assets and changed texture import settings are retained; no existing asset files were deleted. This is not a passing driving test.", "OK");
            }
#endif
        }

        private static GameObject CreateVehicle(GameObject model, Material material, string[] names)
        {
            GameObject car = new GameObject("SANID Vehicle 01 — First Drive");
            car.layer = VehicleLayer;
            car.transform.position = new Vector3(0f, 0.35f, 0f);
            Rigidbody body = car.AddComponent<Rigidbody>();
            body.mass = 1800f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            VehicleInputManager input = car.AddComponent<VehicleInputManager>();
            SurfaceDetector surface = car.AddComponent<SurfaceDetector>();
            VehicleRecovery recovery = car.AddComponent<VehicleRecovery>();
            SandRescueVehicleController controller = car.AddComponent<SandRescueVehicleController>();
            Reference(recovery, "body", body);
            Integer(surface, "surfaceMask", ~(1 << VehicleLayer));

            GameObject visual = UnityEngine.Object.Instantiate(model, car.transform);
            visual.name = "SANID Visuals";
            visual.transform.localPosition = Vector3.zero;
            // Keep the FBX importer's authored root rotation and scale.
            // Then solve the complete forward/up frame, not just a world-Y yaw.
            AlignVehicleFrame(visual.transform, car.transform);
            foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                Material[] slots = renderer.sharedMaterials;
                for (int i = 0; i < slots.Length; i++) slots[i] = material;
                renderer.sharedMaterials = slots;
            }
            Bounds imported = RendererBounds(visual);
            float length = imported.size.z; // Nose is now +Z and roof is +Y.
            if (length < 0.001f) throw new InvalidOperationException("The imported vehicle has zero visible extent.");
            visual.transform.localScale *= 4.9f / length;
            Bounds bounds = RendererBounds(visual);
            visual.transform.position += new Vector3(car.transform.position.x - bounds.center.x,
                car.transform.position.y - bounds.min.y, car.transform.position.z - bounds.center.z);

            Transform massCenter = new GameObject("Center Of Mass — Provisional").transform;
            massCenter.SetParent(car.transform, false);
            massCenter.localPosition = new Vector3(0f, 0.65f, 0f);
            Transform bodyVisual = Find(visual.transform, "Body");
            Bounds bodyBounds = RendererBounds(bodyVisual.gameObject);
            BoxCollider hull = car.AddComponent<BoxCollider>();
            hull.center = car.transform.InverseTransformPoint(bodyBounds.center);
            // Conservative single body box. Tighten around chassis after inspecting wheel clearance.
            hull.size = bodyBounds.size;

            VehicleWheel[] wheels = new VehicleWheel[names.Length];
            Transform physicsParent = new GameObject("Wheel Physics").transform;
            physicsParent.SetParent(car.transform, false);
            for (int i = 0; i < names.Length; i++)
            {
                Transform wheelMesh = Find(visual.transform, names[i]);
                Bounds wheelBounds = RendererBounds(wheelMesh.gameObject);
                float radius = wheelBounds.extents.y; // Vertical diameter is independent of tire tread thickness.
                if (radius < 0.2f || radius > 0.8f)
                    throw new InvalidOperationException("Unexpected imported wheel radius for " + names[i] + ": " + radius);
                // Canonical wrapper prevents GetWorldPose from wiping the mesh's authored orientation.
                Transform wrapper = new GameObject(names[i] + "_VisualPivot").transform;
                wrapper.SetParent(car.transform, false);
                wrapper.position = wheelMesh.position;
                wrapper.rotation = car.transform.rotation;
                wheelMesh.SetParent(wrapper, true);

                GameObject physics = new GameObject(names[i] + "_Collider");
                physics.transform.SetParent(physicsParent, false);
                physics.transform.position = wrapper.position + Vector3.up * (Suspension * 0.5f);
                WheelCollider collider = physics.AddComponent<WheelCollider>();
                collider.radius = radius;
                collider.mass = 25f;
                collider.suspensionDistance = Suspension;
                JointSpring spring = collider.suspensionSpring;
                spring.spring = 35000f;
                spring.damper = 4500f;
                spring.targetPosition = 0.5f;
                collider.suspensionSpring = spring;
                VehicleWheel wheel = physics.AddComponent<VehicleWheel>();
                Reference(wheel, "visualWheel", wrapper);
                Boolean(wheel, "steeringWheel", names[i].StartsWith("Wheel_F", StringComparison.Ordinal));
                Boolean(wheel, "drivenWheel", true);
                Boolean(wheel, "brakeWheel", true);
                wheels[i] = wheel;
            }
            Reference(controller, "body", body);
            Reference(controller, "input", input);
            Reference(controller, "surfaceDetector", surface);
            Reference(controller, "recovery", recovery);
            Reference(controller, "centerOfMass", massCenter);
            SerializedObject serialized = new SerializedObject(controller);
            SerializedProperty array = Required(serialized, "wheels");
            array.arraySize = wheels.Length;
            for (int i = 0; i < wheels.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = wheels[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            FirstDriveCollisionFilter filter = car.AddComponent<FirstDriveCollisionFilter>();
            Reference(filter, "bodyHull", hull);
            SerializedObject filterData = new SerializedObject(filter);
            SerializedProperty colliders = Required(filterData, "wheelColliders");
            colliders.arraySize = wheels.Length;
            for (int i = 0; i < wheels.Length; i++)
                colliders.GetArrayElementAtIndex(i).objectReferenceValue = wheels[i].GetComponent<WheelCollider>();
            filterData.ApplyModifiedPropertiesWithoutUndo();
            SetLayerRecursively(car.transform, VehicleLayer);
            return car;
        }

        private static void AlignVehicleFrame(Transform visual, Transform physicsRoot)
        {
            Vector3 fl = Find(visual, "Wheel_FL").position;
            Vector3 fr = Find(visual, "Wheel_FR").position;
            Vector3 rl = Find(visual, "Wheel_RL").position;
            Vector3 rr = Find(visual, "Wheel_RR").position;
            Vector3 forward = (fl + fr - rl - rr) * 0.5f;
            Vector3 right = (fr + rr - fl - rl) * 0.5f;
            Vector3 up = Vector3.Cross(forward, right);
            if (forward.sqrMagnitude < 0.01f || up.sqrMagnitude < 0.01f)
                throw new InvalidOperationException("The four wheel pivots do not define a usable vehicle frame.");

            // The source's mass marker is above its wheel plane. Use it only to
            // select the roof side; its provisional physics position is not assumed.
            Vector3 wheelCenter = (fl + fr + rl + rr) * 0.25f;
            Vector3 roofSide = Find(visual, "CenterOfMassSuggestion").position - wheelCenter;
            if (Vector3.Dot(up, roofSide) < 0f) up = -up;
            if (Vector3.Dot(up.normalized, roofSide) < 0.05f)
                throw new InvalidOperationException("The roof-side marker is too close to the wheel plane.");
            Quaternion importedFrame = Quaternion.LookRotation(forward.normalized, up.normalized);
            Quaternion correction = physicsRoot.rotation * Quaternion.Inverse(importedFrame);
            visual.rotation = correction * visual.rotation;
        }

        private static void CreateCamera(GameObject car)
        {
            GameObject holder = new GameObject("Main Camera");
            holder.tag = "MainCamera";
            holder.transform.position = car.transform.position + new Vector3(0f, 3.2f, -7f);
            holder.transform.LookAt(car.transform.position + Vector3.up * 1.1f);
            Camera camera = holder.AddComponent<Camera>();
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 300f;
            camera.backgroundColor = new Color(0.42f, 0.64f, 0.77f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            holder.AddComponent<AudioListener>();
            holder.AddComponent<UniversalAdditionalCameraData>();
            CameraCollisionHandler collision = holder.AddComponent<CameraCollisionHandler>();
            Integer(collision, "collisionMask", ~(1 << VehicleLayer));
            VehicleCameraController follow = holder.AddComponent<VehicleCameraController>();
            Reference(follow, "target", car.transform);
            Reference(follow, "vehicle", car.GetComponent<SandRescueVehicleController>());
            Reference(follow, "cameraComponent", camera);
            Reference(follow, "collisionHandler", collision);
        }

        private static void CreateEnvironment(Material sand, Material marker)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "HardSand Test Ground";
            ground.transform.position = new Vector3(0f, -0.5f, 0f);
            ground.transform.localScale = new Vector3(120f, 1f, 120f);
            ground.GetComponent<Renderer>().sharedMaterial = sand;
            GameObject lightObject = new GameObject("Sun");
            Light sun = lightObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.6f;
            sun.shadows = LightShadows.Soft;
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.56f, 0.58f, 0.6f);
            for (int i = 0; i < 6; i++)
            {
                GameObject bollard = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                bollard.name = "Test Lane Marker " + (i + 1);
                bollard.transform.position = new Vector3(i % 2 == 0 ? -5f : 5f, 0.5f, 10f + (i / 2) * 10f);
                bollard.transform.localScale = new Vector3(0.6f, 0.5f, 0.6f);
                bollard.GetComponent<Renderer>().sharedMaterial = marker;
            }
        }

        private static void EnsurePipeline(string folder)
        {
            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset) return;
            UniversalRendererData renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            AssetDatabase.CreateAsset(renderer, folder + "/FirstDrive_Renderer.asset");
            UniversalRenderPipelineAsset pipeline = UniversalRenderPipelineAsset.Create(renderer);
            pipeline.msaaSampleCount = 2;
            pipeline.useSRPBatcher = true;
            AssetDatabase.CreateAsset(pipeline, folder + "/FirstDrive_URP.asset");
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
        }

        private static Material CreateVehicleMaterial(string folder, Shader shader)
        {
            Material material = CreateMaterial(folder + "/MAT_SANID_Vehicle01.mat", shader, Color.white);
            Texture2D color = Texture("SANID_01_BaseColor", true, false);
            Texture2D normal = Texture("SANID_01_Normal", false, true);
            Texture2D metallic = Texture("SANID_01_MetallicSmoothness", false, false);
            Texture2D occlusion = Texture("SANID_01_Occlusion", false, false);
            material.SetTexture("_BaseMap", color);
            material.SetTexture("_BumpMap", normal);
            material.SetTexture("_MetallicGlossMap", metallic);
            material.SetTexture("_OcclusionMap", occlusion);
            material.SetFloat("_Smoothness", 0.75f);
            material.SetFloat("_SmoothnessTextureChannel", 0f);
            material.EnableKeyword("_NORMALMAP");
            material.EnableKeyword("_METALLICSPECGLOSSMAP");
            material.EnableKeyword("_OCCLUSIONMAP");
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Texture2D Texture(string name, bool srgb, bool normal)
        {
            string path = VehicleFolder + "/Textures/" + name + ".png";
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("Missing texture: " + path);
            importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            importer.sRGBTexture = srgb;
            importer.maxTextureSize = 1024;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = false;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Material CreateMaterial(string path, Shader shader, Color color)
        {
            Material material = new Material(shader);
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Transform Find(Transform root, string name)
        {
            foreach (Transform item in root.GetComponentsInChildren<Transform>(true))
                if (item.name == name) return item;
            return null;
        }

        private static Bounds RendererBounds(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) throw new InvalidOperationException("No renderers under " + root.name);
            Bounds result = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) result.Encapsulate(renderers[i].bounds);
            return result;
        }

        private static void SetLayerRecursively(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root) SetLayerRecursively(child, layer);
        }

        private static SerializedProperty Required(SerializedObject target, string field)
        {
            SerializedProperty property = target.FindProperty(field);
            if (property == null) throw new InvalidOperationException("Missing serialized field: " + target.targetObject.GetType().Name + "." + field);
            return property;
        }

        private static void Reference(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            Required(serialized, field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Boolean(UnityEngine.Object target, string field, bool value)
        {
            SerializedObject serialized = new SerializedObject(target);
            Required(serialized, field).boolValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Integer(UnityEngine.Object target, string field, int value)
        {
            SerializedObject serialized = new SerializedObject(target);
            Required(serialized, field).intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif

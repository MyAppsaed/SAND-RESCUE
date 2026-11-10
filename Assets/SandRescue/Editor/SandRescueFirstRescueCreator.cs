#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace SandRescue.Editor
{
    /// <summary>Creates a non-destructive first rescue mission from the validated FirstDrive scene.</summary>
    public static class SandRescueFirstRescueCreator
    {
        private const string SourceSceneName = "SANID_FirstDrive";
        private const string MissionFolder = "Assets/SandRescue/Data/Missions";
        private const string MissionId = "mission_first_rescue";
        private const string TargetId = "stranded_car_01";
        private const string PointId = "stranded_car_front";
        private const string SafeZoneId = "safe_zone_01";

        [MenuItem("Tools/SAND RESCUE/Create First Rescue Mission Scene")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            string sourcePath = FindSourceScene();
            if (string.IsNullOrEmpty(sourcePath))
            {
                EditorUtility.DisplayDialog("SAND RESCUE", "Open or import the validated SANID_FirstDrive scene before creating the mission.", "OK");
                return;
            }
            if (!EditorUtility.DisplayDialog("Create First Rescue Mission",
                "This creates a new mission scene from the validated driving scene. It adds a stranded target, safe zone, winch controls and a test HUD. Existing scenes and assets are not overwritten.", "Create", "Cancel")) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            Scene scene = default(Scene);
            try
            {
                scene = EditorSceneManager.OpenScene(sourcePath, OpenSceneMode.Single);
                string rootFolder = "Assets/SandRescue/Generated/Missions";
                EnsureFolder(rootFolder);
                string folder = AssetDatabase.GenerateUniqueAssetPath(rootFolder + "/FirstRescue");
                EnsureFolder(folder);
                RescueMissionData mission = CreateMissionAsset();
                Material targetMaterial = CreateMaterial(folder + "/MAT_StrangerVehicle.mat", new Color(0.35f, 0.38f, 0.42f));
                Material zoneMaterial = CreateMaterial(folder + "/MAT_SafeZone.mat", new Color(0.15f, 0.85f, 0.35f, 0.55f));

                GameObject player = GameObject.Find("SANID Vehicle 01 — First Drive");
                if (player == null) throw new InvalidOperationException("The validated player vehicle was not found in the source scene.");
                if (PrefabUtility.IsPartOfPrefabInstance(player))
                    PrefabUtility.UnpackPrefabInstance(player, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                RescueWinch winch = ConfigurePlayerWinch(player);
                GameObject safeZoneObject = CreateSafeZone(zoneMaterial);
                SafeZone safeZone = safeZoneObject.GetComponent<SafeZone>();
                GameObject targetObject = CreateTarget(targetMaterial, safeZone);
                StrandedVehicle target = targetObject.GetComponent<StrandedVehicle>();

                GameObject missionObject = new GameObject("First Rescue Mission — Auto Start");
                MissionManager manager = missionObject.AddComponent<MissionManager>();
                Reference(manager, "currentMission", mission);
                MissionAutoStarter starter = missionObject.AddComponent<MissionAutoStarter>();
                Reference(starter, "missionManager", manager);
                FirstRescueMissionHUD hud = missionObject.AddComponent<FirstRescueMissionHUD>();
                Reference(hud, "missionManager", manager);
                Reference(hud, "winch", winch);
                Reference(hud, "target", target);

                string scenePath = folder + "/FirstRescue.unity";
                if (!EditorSceneManager.SaveScene(scene, scenePath)) throw new InvalidOperationException("Unity did not save the first rescue scene.");
                AssetDatabase.SaveAssets();
                Selection.activeGameObject = player;
                EditorUtility.DisplayDialog("First Rescue Mission Created",
                    "Saved: " + scenePath + "\n\nPlay the scene, drive near the stranded vehicle, press E to attach, then tow it into the green safe zone. Q detaches; T/Y reel the cable. This is a starter mission slice, not the final UI.", "OK");
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                EditorUtility.DisplayDialog("First Rescue Mission stopped", error.Message + "\n\nThe original source scene was not overwritten. Check the Console before retrying.", "OK");
            }
        }

        private static string FindSourceScene()
        {
            string[] guids = AssetDatabase.FindAssets(SourceSceneName + " t:Scene");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("/SANID_FirstDrive.unity", StringComparison.OrdinalIgnoreCase)) return path;
            }
            return null;
        }

        private static RescueMissionData CreateMissionAsset()
        {
            EnsureFolder("Assets/SandRescue/Data");
            EnsureFolder(MissionFolder);
            string path = MissionFolder + "/" + MissionId + ".asset";
            RescueMissionData asset = AssetDatabase.LoadAssetAtPath<RescueMissionData>(path);
            if (asset == null) { asset = ScriptableObject.CreateInstance<RescueMissionData>(); AssetDatabase.CreateAsset(asset, path); }
            asset.missionId = MissionId;
            asset.title = new BilingualText { key = MissionId, arabicFallback = "مهمة الإنقاذ الأولى", englishFallback = "First Rescue" };
            asset.description = new BilingualText { key = MissionId + "_description", arabicFallback = "اسحب السيارة العالقة إلى منطقة الأمان", englishFallback = "Tow the stranded car to the safe zone" };
            asset.difficulty = 1;
            asset.timeLimitSeconds = 0f;
            asset.minimumFuel = 1f;
            asset.coinsReward = 100;
            asset.experienceReward = 50;
            asset.objectives = new List<MissionObjectiveDefinition>
            {
                new MissionObjectiveDefinition
                {
                    objectiveId = "objective_attach_winch",
                    type = MissionObjectiveType.AttachWinch,
                    targetId = PointId,
                    title = new BilingualText { key = "objective_attach_winch", arabicFallback = "اربط حبل السحب بالمركبة", englishFallback = "Attach the winch to the vehicle" },
                    requiredAmount = 1
                },
                new MissionObjectiveDefinition
                {
                    objectiveId = "objective_tow_to_safety",
                    type = MissionObjectiveType.TowTarget,
                    targetId = TargetId,
                    title = new BilingualText { key = "objective_tow_to_safety", arabicFallback = "اسحب المركبة إلى الأمان", englishFallback = "Tow the vehicle to safety" },
                    requiredAmount = 1
                }
            };
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static RescueWinch ConfigurePlayerWinch(GameObject player)
        {
            Rigidbody body = player.GetComponent<Rigidbody>();
            Transform anchor = new GameObject("Front Winch Anchor").transform;
            anchor.SetParent(player.transform, false);
            anchor.localPosition = new Vector3(0f, 0.65f, 2.8f);
            RescueTargetDetector detector = player.AddComponent<RescueTargetDetector>();
            RescueWinch winch = player.AddComponent<RescueWinch>();
            MissionWinchInput input = player.AddComponent<MissionWinchInput>();
            LineRenderer line = player.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.startWidth = 0.06f;
            line.endWidth = 0.045f;
            line.material = new Material(Shader.Find("Sprites/Default")) { color = new Color(0.05f, 0.05f, 0.05f) };
            WinchCableRenderer cable = player.AddComponent<WinchCableRenderer>();
            Reference(detector, "origin", anchor);
            Reference(winch, "vehicleBody", body);
            Reference(winch, "attachmentPoint", anchor);
            Reference(winch, "cable", cable);
            Reference(winch, "targetDetector", detector);
            Reference(input, "winch", winch);
            return winch;
        }

        private static GameObject CreateTarget(Material material, SafeZone destination)
        {
            GameObject root = new GameObject("Stranded Vehicle — First Rescue");
            root.transform.position = new Vector3(0f, 0.85f, 26f);
            Rigidbody body = root.AddComponent<Rigidbody>();
            body.mass = 1200f;
            body.linearDamping = 1.5f;
            body.angularDamping = 3f;
            BoxCollider hull = root.AddComponent<BoxCollider>();
            hull.size = new Vector3(2.4f, 1.4f, 4.4f);
            SphereCollider trigger = root.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 2.2f;
            TowableVehicle towable = root.AddComponent<TowableVehicle>();
            StrandedVehicle stranded = root.AddComponent<StrandedVehicle>();
            Reference(stranded, "towable", towable);
            Reference(stranded, "destination", destination);
            String(stranded, "targetId", TargetId);
            Enum(stranded, "objectiveType", MissionObjectiveType.TowTarget);

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Stranded Vehicle Placeholder Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = new Vector3(2.3f, 1.3f, 4.2f);
            visual.GetComponent<Renderer>().sharedMaterial = material;
            UnityEngine.Object.DestroyImmediate(visual.GetComponent<BoxCollider>());
            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin Placeholder";
            cabin.transform.SetParent(root.transform, false);
            cabin.transform.localPosition = new Vector3(0f, 0.95f, -0.15f);
            cabin.transform.localScale = new Vector3(1.7f, 0.75f, 2f);
            cabin.GetComponent<Renderer>().sharedMaterial = material;
            UnityEngine.Object.DestroyImmediate(cabin.GetComponent<BoxCollider>());

            GameObject pointObject = new GameObject("Front Connection — " + PointId);
            pointObject.transform.SetParent(root.transform, false);
            pointObject.localPosition = new Vector3(0f, 0.35f, 2.35f);
            SphereCollider pointCollider = pointObject.AddComponent<SphereCollider>();
            pointCollider.isTrigger = true;
            pointCollider.radius = 0.55f;
            RescueConnectionPoint point = pointObject.AddComponent<RescueConnectionPoint>();
            String(point, "pointId", PointId);
            return root;
        }

        private static GameObject CreateSafeZone(Material material)
        {
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            zone.name = "Safe Zone — First Rescue";
            zone.transform.position = new Vector3(0f, 0.08f, 52f);
            zone.transform.localScale = new Vector3(5f, 0.08f, 5f);
            zone.GetComponent<Renderer>().sharedMaterial = material;
            Collider collider = zone.GetComponent<Collider>();
            collider.isTrigger = true;
            SafeZone safe = zone.AddComponent<SafeZone>();
            String(safe, "zoneId", SafeZoneId);
            return zone;
        }

        private static Material CreateMaterial(string path, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            Material material = new Material(shader);
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void Reference(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null) throw new InvalidOperationException("Missing serialized field: " + target.GetType().Name + "." + field);
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void String(UnityEngine.Object target, string field, string value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null) throw new InvalidOperationException("Missing serialized field: " + target.GetType().Name + "." + field);
            property.stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Enum(UnityEngine.Object target, string field, Enum value)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null) throw new InvalidOperationException("Missing serialized field: " + target.GetType().Name + "." + field);
            property.enumValueIndex = Convert.ToInt32(value);
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

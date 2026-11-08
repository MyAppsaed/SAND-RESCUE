#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SandRescue.Editor
{
    public static class SandRescueStep2AssetCreator
    {
        [MenuItem("Tools/SAND RESCUE/Create Step 2 Starter Missions")]
        public static void CreateStarterMissions()
        {
            const string folder = "Assets/SandRescue/Data/Missions";
            EnsureFolder("Assets/SandRescue/Data");
            EnsureFolder(folder);
            CreateMission(folder, "mission_first_rescue", "مهمة الإنقاذ الأولى", "First Rescue", "اسحب السيارة العالقة إلى منطقة الأمان", "Tow the stranded car to the safe zone", 1, 0f, 100, 50, new[]
            {
                Objective("objective_attach_winch", MissionObjectiveType.AttachWinch, "اربط حبل السحب بالمركبة", "Attach the winch to the vehicle", "stranded_car_front"),
                Objective("objective_tow_to_safety", MissionObjectiveType.TowTarget, "اسحب المركبة إلى منطقة الأمان", "Tow the vehicle to safety", "stranded_car_01")
            });
            CreateMission(folder, "mission_fuel_emergency", "طوارئ الوقود", "Fuel Emergency", "أوصل الوقود قبل انتهاء الوقت", "Deliver fuel before time expires", 2, 180f, 150, 80, new[]
            {
                Objective("objective_reach_fuel_target", MissionObjectiveType.ReachTrigger, "توجه إلى السائق العالق", "Reach the stranded driver", "fuel_target_01"),
                Objective("objective_deliver_fuel", MissionObjectiveType.DeliverFuel, "سلم الوقود", "Deliver the emergency fuel", "fuel_target_01")
            });
            CreateMission(folder, "mission_dune_recovery", "إنقاذ بين الكثبان", "Dune Recovery", "اعثر على المركبة واسحبها عبر الكثبان", "Find and tow the vehicle across the dunes", 3, 0f, 250, 120, new[]
            {
                Objective("objective_reach_dune_vehicle", MissionObjectiveType.ReachTrigger, "اعثر على مركبة الإنقاذ", "Reach the stranded SUV", "stranded_suv_01"),
                Objective("objective_tow_dune_vehicle", MissionObjectiveType.TowTarget, "اسحب المركبة إلى الأمان", "Tow the SUV to safety", "stranded_suv_01")
            });
            CreateMission(folder, "mission_sandstorm_search", "البحث وسط العاصفة", "Sandstorm Search", "ابحث عن المسافر المفقود وأنقذه", "Search for and rescue the missing traveler", 4, 240f, 300, 180, new[]
            {
                Objective("objective_search_area", MissionObjectiveType.SearchArea, "ابحث في منطقة العاصفة", "Search the storm area", "search_area_01"),
                Objective("objective_rescue_traveler", MissionObjectiveType.RescueCharacter, "أنقذ المسافر", "Rescue the traveler", "traveler_01"),
                Objective("objective_reach_safety", MissionObjectiveType.ReachSafeZone, "عد إلى منطقة الأمان", "Return to the safe zone", "safe_zone_01")
            });
            CreateMission(folder, "mission_heavy_rescue", "مهمة الإنقاذ الثقيلة", "Heavy Rescue", "استخدم شاحنة الإنقاذ لاستعادة المركبة الثقيلة", "Use the heavy truck to recover the heavy vehicle", 5, 360f, 500, 300, new[]
            {
                Objective("objective_attach_heavy_winch", MissionObjectiveType.AttachWinch, "اربط الونش القوي", "Attach the heavy winch", "heavy_vehicle_front"),
                Objective("objective_tow_heavy_vehicle", MissionObjectiveType.TowTarget, "اسحب المركبة الثقيلة", "Tow the heavy vehicle", "heavy_vehicle_01")
            });
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("SAND RESCUE", "Five Step 2 starter missions were created or updated.", "OK");
        }

        private static MissionObjectiveDefinition Objective(string key, MissionObjectiveType type, string ar, string en, string target)
        {
            return new MissionObjectiveDefinition
            {
                objectiveId = key,
                type = type,
                targetId = target,
                title = new BilingualText { key = key, arabicFallback = ar, englishFallback = en },
                requiredAmount = 1
            };
        }

        private static void CreateMission(string folder, string id, string arTitle, string enTitle, string arDesc, string enDesc, int difficulty, float time, int coins, int xp, MissionObjectiveDefinition[] objectives)
        {
            string path = $"{folder}/{id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<RescueMissionData>(path);
            if (asset == null) { asset = ScriptableObject.CreateInstance<RescueMissionData>(); AssetDatabase.CreateAsset(asset, path); }
            asset.missionId = id;
            asset.title = new BilingualText { key = id, arabicFallback = arTitle, englishFallback = enTitle };
            asset.description = new BilingualText { key = id + "_description", arabicFallback = arDesc, englishFallback = enDesc };
            asset.difficulty = difficulty;
            asset.timeLimitSeconds = time;
            asset.coinsReward = coins;
            asset.experienceReward = xp;
            asset.objectives = new List<MissionObjectiveDefinition>(objectives);
            EditorUtility.SetDirty(asset);
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

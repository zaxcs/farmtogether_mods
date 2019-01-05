using System;
using System.Reflection;
using Harmony12;
using UnityModManagerNet;

namespace InfiniteStorage
{
    internal static class InfiniteStorage
    {
        private static bool enabled;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            enabled = modEntry.Active;
            modEntry.OnToggle = (entry, state) =>
            {
                enabled = state;
                return true;
            };
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        [HarmonyPatch(typeof(FarmResourceStorage))]
        [HarmonyPatch("IsMaxedOut", MethodType.Getter)]
        public class Patch_FarmResourceStorage_IsMaxedOut
        {
            public static bool Prefix(FarmResourceStorage __instance, ref bool __result)
            {
                if (!enabled)
                    return true;
                __result = false;
                return false;
            }
        }


        [HarmonyPatch(typeof(FarmResourceStorage))]
        [HarmonyPatch("Progress", MethodType.Getter)]
        public class Patch_FarmResourceStorage_Progress
        {
            public static bool Prefix(FarmResourceStorage __instance, ref float __result)
            {
                if (!enabled)
                    return true;
                __result = Math.Max((float) __instance.Amount / __instance.MaxValue, 1f);
                return false;
            }
        }

        [HarmonyPatch(typeof(FarmResourceStorage))]
        [HarmonyPatch("IncreaseAmount")]
        public class Patch_FarmResourceStorage_IncreaseAmount
        {
            public static bool Prefix(FarmResourceStorage __instance, long amount)
            {
                if (!enabled)
                    return true;
                Traverse.Create(__instance).Field("amount").SetValue(__instance.Amount + amount);
                return false;
            }
        }
    }
}
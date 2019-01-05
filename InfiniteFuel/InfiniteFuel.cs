using System.Reflection;
using Harmony12;
using Logic.Farm;
using UnityModManagerNet;

namespace InfiniteFuel
{
    internal static class InfiniteFuel
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

        [HarmonyPatch(typeof(Effort))]
        [HarmonyPatch("Spend")]
        public class Patch_Effort_Spend
        {
            public static bool Prefix(Effort __instance, uint effortDecrease = 1)
            {
                if (!enabled)
                    return true;
                var xp = Traverse.Create(__instance).Field("experience").GetValue<Experience>();
                return GameGlobals.MaxEffort(xp.Level) / 2 < __instance.Value - effortDecrease;
            }
        }
    }
}
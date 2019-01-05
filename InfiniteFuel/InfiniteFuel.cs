using System.Reflection;
using Harmony12;
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

        [HarmonyPatch(typeof(PlayerScript))]
        [HarmonyPatch("PayFuel")]
        public class Patch_PlayerScript_PayFuel
        {
            public static bool Prefix(PlayerScript __instance, uint value = 1)
            {
                return !enabled && __instance.MaxFuel / 2 < __instance.Fuel - value;
            }
        }
    }
}
using System.Reflection;
using Harmony12;
using UnityModManagerNet;

namespace FasterVehicles
{
    internal static class FasterVehicles
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

        [HarmonyPatch(typeof(PlayerScript), "updateCharacterControllerParameters")]
        public class Patch_PlayerScript_updateCharacterControllerParameters
        {
            public static void Postfix(PlayerScript __instance, MilkThirdPersonController ___controller)
            {
                if (!enabled || !__instance.UsingVehicle)
                    return;

                ___controller.BaseSpeed += 2f;
                ___controller.SprintSpeed += 4f;
            }
        }
    }
}
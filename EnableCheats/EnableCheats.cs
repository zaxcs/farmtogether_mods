using System.Reflection;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;

namespace EnableCheats
{
    internal static class EnableCheats
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

        [HarmonyPatch(typeof(CheatsScript), "Awake")]
        public class Patch_CheatScript_Awake
        {
            public static bool Prefix()
            {
                return false;
            }
        }


        [HarmonyPatch(typeof(CheatsScript))]
        [HarmonyPatch("Update")]
        public class Patch_CheatScript_Update
        {
            public static void Postfix(CheatsScript __instance)
            {
                if (Input.inputString.Length <= 0 || !enabled)
                    return;

                var input = Traverse.Create(__instance).Field("lastInput").GetValue<string>();
                if (input.EndsWith("goday"))
                    StageScript.Instance.FarmData.Tick(3600 * 24);
                else if (input.EndsWith("goseason"))
                    StageScript.Instance.FarmData.Tick(15 * 60);
            }
        }
    }
}
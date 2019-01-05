using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony12;
using Milkstone.Utils;
using UnityEngine;
using UnityModManagerNet;

namespace VehicleRange5x5
{
    internal static class VehicleRange5x5
    {
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        [HarmonyPatch(typeof(LocalPlayerScript), "UpdateFarmTiles")]
        public static class Patch_LocalPlayerScript_UpdateFarmTiles
        {
            public static Int2 VehicleToolSize = new Int2(5, 5);

            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                var modifierField = AccessTools.Field(typeof(Patch_LocalPlayerScript_UpdateFarmTiles),
                    nameof(VehicleToolSize));
                var newInstruction = new CodeInstruction(OpCodes.Ldsfld, modifierField);
                var indexes = new List<int>();
                for (var i = 0; i < codes.Count(); i++)
                {
                    if (codes[i].opcode != OpCodes.Ldsfld)
                        continue;

                    if (!codes[i].operand.ToString().Equals("Milkstone.Utils.Int2 VehicleToolSize"))
                        continue;
                    indexes.Add(i);
                }

                foreach (var index in indexes)
                    codes[index] = newInstruction;

                return codes;
            }
        }

        [HarmonyPatch]
        public class Patch_SelectedTiles_Awake
        {
            private static MethodBase TargetMethod(HarmonyInstance instance)
            {
                return AccessTools.Method("SelectedTiles:Awake");
            }

            public static void Postfix(object __instance)
            {
                Traverse.Create(__instance).Field("previews").SetValue(
                    new GameObject[Patch_LocalPlayerScript_UpdateFarmTiles.VehicleToolSize.x *
                                   Patch_LocalPlayerScript_UpdateFarmTiles.VehicleToolSize.y]);
            }
        }
    }
}
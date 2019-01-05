using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony12;
using UnityModManagerNet;

namespace AlwaysWatered
{
    internal static class AlwaysWatered
    {
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        public class FarmGenericCropTickPatch
        {
            protected static IEnumerable<CodeInstruction> patchIrrigation(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                var index = -1;
                for (var i = 0; i < codes.Count(); i++)
                {
                    if (codes[i].opcode != OpCodes.Callvirt)
                        continue;

                    if (!codes[i].operand.ToString().Equals("Boolean get_Irrigated()"))
                        continue;
                    index = i;
                    break;
                }

                if (index <= 2) return codes;
                {
                    var nop = new CodeInstruction(OpCodes.Nop);
                    for (var i = index - 2; i < index; i++) codes[i] = nop;

                    codes[index] = new CodeInstruction(OpCodes.Ldc_I4_1);
                }

                return codes;
            }
        }

        [HarmonyPatch]
        public class FarmCropTickPatch : FarmGenericCropTickPatch
        {
            private static MethodBase TargetMethod(HarmonyInstance instance)
            {
                return AccessTools.Method("Logic.Farm.Contents.FarmCrop:Tick", new[] {typeof(uint)});
            }

            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return patchIrrigation(instructions);
            }
        }

        [HarmonyPatch]
        public class FarmFlowerTickPatch : FarmGenericCropTickPatch
        {
            private static MethodBase TargetMethod(HarmonyInstance instance)
            {
                return AccessTools.Method("Logic.Farm.Contents.FarmFlower:Tick", new[] {typeof(uint)});
            }

            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return patchIrrigation(instructions);
            }
        }
    }
}
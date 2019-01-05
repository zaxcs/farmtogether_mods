using System;
using System.Reflection;
using Harmony12;
using Milkstone.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace ShowUnlock
{
    internal static class ShowUnlock
    {
        private static UnityModManager.ModEntry.ModLogger logger;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        [HarmonyPatch(typeof(BaseShopHud), "SetupShopButton")]
        public class Patch_BaseShopHud_SetupShopButton
        {
            public static void Postfix(GameObject buttonInstance,
                BaseShopHud.NotificationIcon notification = BaseShopHud.NotificationIcon.None)
            {
                try
                {
                    if ((int) notification != 3) return;

                    var btn = buttonInstance.GetComponentInChildWithSuffix<Image>("IconNew", false);
                    btn.sprite = SpriteLibrary.GetSprite("Star");
                }
                catch (Exception e)
                {
                    logger.Error("setup failed " + e);
                }
            }
        }

        [HarmonyPatch(typeof(FarmShopHud), "GetButtonData")]
        public class Patch_FarmShopHud_GetButtonData
        {
            public static void Postfix(ShopItemDefinition item, ref BaseShopHud.NotificationIcon notification)
            {
                try
                {
                    if (notification != BaseShopHud.NotificationIcon.None || !(item is HarvestItemDefinition) ||
                        !needed((HarvestItemDefinition) item))
                        return;

                    notification = (BaseShopHud.NotificationIcon) 3;
                }
                catch (Exception e)
                {
                    logger.Error("get failed " + e);
                }
            }

            private static bool needed(HarvestItemDefinition item)
            {
                var itemLevel = StageScript.Instance.FarmData.GetItemLevel(item);

                for (var i = 0; i < (int) ShopItemType.Count; i++)
                {
                    var list = ShopManager.GetFarmItemList((ShopItemType) i);
                    if (list == null)
                        continue;

                    foreach (var def in list)
                    {
                        if (def.ItemLevelRequirements == null || def.ItemLevelRequirements.Count == 0)
                            continue;

                        foreach (var req in def.ItemLevelRequirements)
                        {
                            if (!req.Item.FullId.Equals(item.FullId))
                                continue;
                            if (req.Level > itemLevel)
                                return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
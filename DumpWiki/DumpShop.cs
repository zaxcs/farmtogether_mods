using System;
using System.Collections.Generic;
using System.IO;

namespace DumpWiki
{
    internal static class DumpShop
    {
        private static string[] ConvertSeasons(Season s)
        {
            return new[]
            {
                (s & Season.Spring) > Season.None ? "Y" : "N",
                (s & Season.Summer) > Season.None ? "Y" : "N",
                (s & Season.Fall) > Season.None ? "Y" : "N",
                (s & Season.Winter) > Season.None ? "Y" : "N"
            };
        }

        private static string[] ConvertMoney(FarmMoney m)
        {
            return new[]
            {
                m.Coins.ToString(),
                m.Bills.ToString(),
                m.Medals.ToString(),
                m.Tickets.ToString()
            };
        }

        private static string[] DumpHeaders()
        {
            return new[]
            {
                "Item",
                "ItemEN",
                "Type",
                "MinLevel",
                "Requirements",
                "DLC",
                "EventItem",
                "EventReward",
                "SeasonalEvent",
                "VariablePrice",
                "VariablePriceFactor",
                "BuyXPGain",
                "BuyCoins",
                "BuyDiamonds",
                "BuyMedals",
                "BuyTickets",
                "ResourceType",
                "ResourceAmount",
                "GrowTime",
                "Spring",
                "Summer",
                "Autumn",
                "Winter",
                "HarvestXPGain",
                "HarvestCoins",
                "HarvestDiamonds",
                "HarvestMedals",
                "HarvestTickets",
                "HarvestChance",
                "FailedCoins",
                "FailedDiamonds",
                "FailedMedals",
                "FailedTickets"
            };
        }

        internal static void DumpStats()
        {
            using (var writer = new StreamWriter($@"dump-{DateTime.Now.Ticks}.txt"))
            {
                try
                {
                    writer.Write("{0}\r\n", string.Join("\t", DumpHeaders()));
                    foreach (var obj in Enum.GetValues(typeof(ShopItemType)))
                    {
                        var farmItemList = ShopManager.GetFarmItemList((ShopItemType) obj);
                        if (farmItemList != null)
                            foreach (var def in farmItemList)
                            {
                                var list = new List<string>();
                                list.AddRange(DumpGenericStats(def));
                                if (def is CropDefinition)
                                    list.AddRange(DumpCropStats((CropDefinition) def));
                                else if (def is TreeDefinition)
                                    list.AddRange(DumpTreeStats((TreeDefinition) def));
                                else if (def is AnimalDefinition)
                                    list.AddRange(DumpAnimalStats((AnimalDefinition) def));
                                else if (def is PondDefinition)
                                    list.AddRange(DumpPondStats((PondDefinition) def));
                                else if (def is FlowerDefinition)
                                    list.AddRange(DumpFlowerStats((FlowerDefinition) def));

                                writer.Write("{0}\r\n", string.Join("\t", list.ToArray()));
                            }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine(ex.ToString());
                }
            }
        }

        private static string[] DumpTreeStats(TreeDefinition def)
        {
            var list = new List<string>();
            list.Add("");
            list.AddRange(ConvertSeasons(def.HarvestSeasons));
            list.Add(def.HarvestXp.ToString());
            list.AddRange(ConvertMoney(def.HarvestMoney));
            return list.ToArray();
        }

        private static string[] DumpAnimalStats(AnimalDefinition def)
        {
            var list = new List<string>();
            list.Add(def.HarvestInterval.ToString());
            list.AddRange(new string[4]);
            list.Add(def.HarvestXp.ToString());
            list.AddRange(ConvertMoney(def.HarvestMoney));
            return list.ToArray();
        }

        private static string[] DumpPondStats(PondDefinition def)
        {
            var list = new List<string>();
            list.Add(def.FishInterval.ToString());
            list.AddRange(new string[4]);
            list.Add(def.HarvestXp.ToString());
            list.AddRange(ConvertMoney(def.HarvestMoney));
            list.Add(def.SuccessChance.ToString());
            list.AddRange(ConvertMoney(def.FailMoney));
            return list.ToArray();
        }

        private static List<string> DumpGenericStats(FarmItemDefinition def)
        {
            var list = new List<string>();
            list.AddRange(new[]
            {
                def.name,
                Localization.Get(def.FullId, true),
                def.Category.ToString(),
                def.MinLevel.ToString(),
                GetRequirements(def),
                def.RequiredDLC.ToString(),
                def.IsEventItem ? "Y" : "N",
                def.IsEventReward ? "Y" : "N",
                def.SeasonalEvent.ToString(),
                def.VariablePrice ? "Y" : "N",
                def.VariablePriceItemCountFactor.ToString(),
                def.BuyXp.ToString()
            });
            list.AddRange(ConvertMoney(def.Price));
            if (def is HarvestItemDefinition)
            {
                var harvestItemDefinition = (HarvestItemDefinition) def;
                list.Add(harvestItemDefinition.Resource.Type.ToString());
                list.Add(harvestItemDefinition.Resource.Amount.ToString());
            }
            else
            {
                list.AddRange(new string[2]);
            }

            return list;
        }

        private static List<string> DumpCropStats(CropDefinition def)
        {
            var list = new List<string>();
            list.Add(def.GrowTime.ToString());
            list.AddRange(ConvertSeasons(def.PlantSeasons));
            list.Add(def.HarvestXp.ToString());
            list.AddRange(ConvertMoney(def.HarvestMoney));
            return list;
        }

        private static string GetRequirements(FarmItemDefinition f)
        {
            var list = new List<string>();
            foreach (var itemLevelRequirement in f.ItemLevelRequirements)
                list.Add(itemLevelRequirement.Item.name + "@" + itemLevelRequirement.Level);

            return string.Join(",", list.ToArray());
        }

        private static string[] DumpFlowerStats(FlowerDefinition def)
        {
            var list = new List<string>();
            list.Add(def.MaxGrowth.ToString());
            list.AddRange(new string[4]);
            list.Add(def.HarvestXp.ToString());
            list.AddRange(ConvertMoney(def.HarvestMoney));
            return list.ToArray();
        }
    }
}
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TOR_Core.Models;
using TOR_Core.Extensions;

namespace TORBalance.GoldCost
{
    [HarmonyPatch]
    internal class WagePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TORPartyWageModel),"GetCharacterWage")]  // 明确指定目标方法
        public static bool Prefix_GetCharacterWage(ref int __result, CharacterObject character)
        {
            int value = BaseCharacterWage(character);
            string characterId = character.StringId;

            TroopData troopData = CustomXmlLoader.GetDataById<TroopData>(characterId);
            if (troopData == null)
            {
                __result = value;
                return false; // 如果找不到 TroopData，返回原始值
                
            }
            // 获取自定义的 Wage 加成
            int customWageBonus = (int)troopData.Wage;

            __result = value + customWageBonus;

            return false;
        }

        public static int BaseCharacterWage(CharacterObject character)
        {
            if (character.IsUndead())
            {
                return 0;
            }
            if (character.IsTreeSpirit())
            {
                return 0;
            }
            int value = GetWageForTier(character.Tier);
            if (character.Culture.StringId == "vlandia" && character.IsKnightUnit())
            {
                value *= 2;
            }
            if (character.Culture.StringId == "eonir" && character.IsEliteTroop())
            {
                value *= 3;
            }
            return value;
        }

        private static int GetWageForTier(int tier)
        {
            switch (tier)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 5;
                case 4:
                    return 8;
                case 5:
                    return 12;
                case 6:
                    return 17;
                case 7:
                    return 23;
                case 8:
                    return 30;
                default:
                    return 40;
            }
        }
    }
}

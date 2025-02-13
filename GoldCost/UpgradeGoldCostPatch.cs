using HarmonyLib;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem;
using TOR_Core.Models;

namespace TORBalance.GoldCost
{
    [HarmonyPatch]
    internal class UpgradeGoldCostPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefaultPartyTroopUpgradeModel), "GetGoldCostForUpgrade")]
        public static void Postfix_GetGoldCostForUpgrade(ref int __result, PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
        {
            // 检查 upgradeTarget 是否为空
            if (upgradeTarget == null)
            {
                return; // 如果为空，直接返回
            }

            TroopData troopData = CustomXmlLoader.GetDataById<TroopData>(upgradeTarget.StringId);
            if (troopData == null) return; // 如果找不到 TroopData，返回原始值

            // 调用自定义的计算方法，获取 GOLD 加成
            int customGoldBonus = (int)troopData.GoldCostUpgrade;
            __result += customGoldBonus;
        }

    }
}


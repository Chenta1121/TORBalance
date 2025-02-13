using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
namespace TORBalance.XpCostUpgrade
{
    [Harmony]
    internal class XpCostUpgradePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefaultPartyTroopUpgradeModel), "GetXpCostForUpgrade")]
        public static void Postfix_GetXpCostForUpgrade(ref int __result, PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
        {
            // 调用自定义的计算方法，获取XP加成
            TroopData troopData = CustomXmlLoader.GetDataById<TroopData>(upgradeTarget.StringId);

            if (troopData == null) return;

            int customXpBonus = (int)troopData.XpCostUpgrade;
            __result += customXpBonus;
        }

    }
}

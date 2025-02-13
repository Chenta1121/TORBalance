using TOR_Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.Library;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TORBalance.MaxHealth
{
    [Harmony]
    internal class HitPointsPatch
    {
 
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TOR_Core.Models.TORCharacterStatsModel), "MaxHitpoints")]
        public static void Postfix_MaxHitpoints(ref ExplainedNumber __result, CharacterObject character, bool includeDescriptions)
        {
            string characterId = character.StringId;
            // 处理英雄角色
            if (character.IsHero)
            {
                LordData lordData = CustomXmlLoader.GetDataById<LordData>(characterId);

                if (lordData == null) {
                    
                    return;
                }

                string description = lordData.Description;
                float healthBonus = lordData.HealthBonus;
                __result.Add(healthBonus, new TextObject(description, null), null);
                return;  // 直接退出方法，避免后续操作
            }
            else {

                // 获取自定义加值和描述
                TroopData troopData = CustomXmlLoader.GetDataById<TroopData>(characterId);

                if (troopData == null)
                {
                    return;
                }

                string description = troopData.Description;
                float healthBonus = troopData.HealthBonus;

                // 将返回值增加自定义加值
                __result.Add(healthBonus, new TextObject(description, null), null);
            }

        }
    
    
    }
}

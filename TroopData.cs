using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TORBalance
{
    internal class TroopData
    {
        public string Id { get; set; }
        public float HealthBonus { get; set; }
        public string Description { get; set; }
        public float XpCostUpgrade { get; set; }
        public float GoldCostUpgrade { get; set; }
        public float Wage { get; set; }
        public TroopData()
        {
            // 设置默认值
            HealthBonus = 0f;
            Description = string.Empty;
            XpCostUpgrade = 0f;
            GoldCostUpgrade = 0f;
            Wage = 0f;
        }
    }
}

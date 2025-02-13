using TaleWorlds.ModuleManager;

namespace TORBalance
{
    public static class BalancePath
    {
        public static string TORBalanceModuleRootPath
        {
            get
            {
                return ModuleHelper.GetModuleFullPath("TORBalance");
            }
        }

        public static string TORBalanceModuleDataPath
        {
            get
            {
                return TORBalanceModuleRootPath + "ModuleData/";
            }
        }
    }
}

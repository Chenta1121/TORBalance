using HarmonyLib;
using System.Reflection;
using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using System.Linq;


namespace TORBalance
{

    public class SubModule : MBSubModuleBase
    {
        public bool _shouldLoadPatches = true;
        public static Harmony HarmonyInstance { get; private set; }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            UnloadXmlData();
            // 实例化 Harmony，确保在加载补丁前创建实例
            HarmonyInstance = new Harmony("com.yuef.patch");
            //卸载无关数据

            // 在补丁应用前加载数据
            CustomXmlLoader.LoadXmlData();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            // 清理补丁和实例，防止残留
            if (HarmonyInstance != null)
            {
                HarmonyInstance.UnpatchAll();  // 清理补丁
                HarmonyInstance = null;       // 释放实例
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            // 显示模块作者信息
            DisplayModuleInfo();
            
            // 检查是否应该加载补丁
            if (ShouldLoadPatches())
            {
                ApplyPatches();
            }
        }

        private void DisplayModuleInfo()
        {
            InformationManager.DisplayMessage(new InformationMessage("TOR_Balance Author: 艾查恩; YuefChen.", Colors.Green));
        }

        private bool ShouldLoadPatches()
        {
            // 只有当 _shouldLoadPatches 为 true 且 Harmony 实例化时才加载补丁
            return this._shouldLoadPatches && HarmonyInstance != null;
        }

        private void ApplyPatches()
        {
            try
            {
                // 使用 Harmony 为模块中的所有方法应用补丁
                HarmonyInstance.PatchAll(Assembly.GetAssembly(typeof(SubModule)));
                this._shouldLoadPatches = false;
            }
            catch (Exception ex)
            {
                // 捕获并记录错误，防止程序崩溃
                InformationManager.DisplayMessage(new InformationMessage($"Patch Application Failed: {ex.Message}\n{ex.StackTrace}", Colors.Red));

            }
        }

        private void UnloadXmlData()
        {
            RemoveXmlInformation("TOR_Core", "EquipmentRosters", "tor_equipment_sets");
        }

        public static void RemoveXmlInformation(string moduleName, string id, string name)
        {
            // 查找符合条件的对象
            var objToRemove = XmlResource.XmlInformationList
                .FirstOrDefault(obj => obj.ModuleName == moduleName && obj.Id == id && obj.Name == name);

            // 如果找到了该对象，则移除它
            if (objToRemove.Equals(default(MbObjectXmlInformation)))
            {
                // 如果找不到该对象，抛出异常
                var errorMessage = $"No object found with ModuleName: {moduleName}, Id: {id}, and Name: {name}";
                InformationManager.DisplayMessage(new InformationMessage(errorMessage, Colors.Red));
            }
            else
            {
                // 找到后移除对象
                XmlResource.XmlInformationList.Remove(objToRemove);
            }
        }

    }
}

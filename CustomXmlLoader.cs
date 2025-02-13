using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NLog;
using TOR_Core.Utilities;
using HarmonyLib;
using TOR_Core.Extensions.ExtendedInfoSystem;
using System.Xml.Linq;
using System.Reflection;


namespace TORBalance
{
    [Harmony]
    public static class CustomXmlLoader
    {
        // 显式字典
        private static Dictionary<string, TroopData> cachedTroopData = new Dictionary<string, TroopData>();
        private static Dictionary<string, LordData> cachedLordData = new Dictionary<string, LordData>();

        internal static string troopDataPath = BalancePath.TORBalanceModuleDataPath + "CustomLoadXMLLib/tor_balance_troops.xml";
        internal static string lordDataPath = BalancePath.TORBalanceModuleDataPath + "CustomLoadXMLLib/tor_balance_lords.xml";

        internal static void LoadXmlData()
        {
            cachedTroopData = LoadData<TroopData>(troopDataPath);
            cachedLordData = LoadData<LordData>(lordDataPath);
        }

        public static T GetDataById<T>(string id)
        {
            // 根据类型选择缓存字典
            if (typeof(T) == typeof(TroopData))
            {
                // 处理 TroopData 类型
                if (cachedTroopData.TryGetValue(id, out TroopData troopData))
                {
                    return (T)(object)troopData;  // 转换为泛型类型返回
                }
            }
            else if (typeof(T) == typeof(LordData))
            {
                // 处理 LordData 类型
                if (cachedLordData.TryGetValue(id, out LordData lordData))
                {
                    return (T)(object)lordData;  // 转换为泛型类型返回
                }
            }
            // 返回默认值，如果没有找到
            return default(T);
        }


        public static Dictionary<string, T> LoadData<T>(string dataPath)
        {
            // 日志文件路径，存储在和数据文件同一目录下
            string logFilePath = Path.Combine(Path.GetDirectoryName(dataPath), "Debug.txt");

            Dictionary<string, T> cacheDictionary = new Dictionary<string, T>();

            // 加载 XML 文件
            XElement rootElement = XElement.Load(dataPath);

            // 获取数据类型的名称
            string typeName = typeof(T).Name;
            string xmlRootElementName = typeName.EndsWith("Data") ? typeName.Substring(0, typeName.Length - 4): typeName;

            // 每次都通过反射获取属性信息
            PropertyInfo[] properties = typeof(T).GetProperties();

            // 遍历所有相应元素
            foreach (var element in rootElement.Elements(xmlRootElementName))
            {
                string id = element.Attribute("Id")?.Value;


                if (id != null)
                {

                    if (cacheDictionary.ContainsKey(id))
                        continue;

                    T dataObject = Activator.CreateInstance<T>();
                    PropertyInfo idProperty = typeof(T).GetProperty("Id");

                    // 给 Id 属性赋值
                    idProperty?.SetValue(dataObject, id);

                    // 使用反射动态查找并处理缺失的属性
                    foreach (var property in properties)
                    {
                        // 查找对应的 XML 元素名称
                        string xmlElementName = property.Name;
                        if (xmlElementName == "Id")continue;

                        var elementValue = element.Element(xmlElementName);
                        if (elementValue == null)
                        {
                            // 仅在缺失元素时创建默认值元素
                            elementValue = CreateDefaultElement(property);
                            element.Add(elementValue);
                           
                        }

                        AssignValueToProperty(property, elementValue, dataObject);
                    }

                    cacheDictionary[id] = dataObject;
                }
            }

            // 保存修改后的 XML 文件
            rootElement.Save(dataPath);
            return cacheDictionary;
        }


        private static XElement CreateDefaultElement(PropertyInfo property)
        {
            object defaultValue = GetDefaultValueForType(property.PropertyType);
            XElement element;

            if (property.PropertyType == typeof(float))
            {
                element = new XElement(property.Name, new XAttribute("value", defaultValue.ToString()));
            }
            else if (property.PropertyType == typeof(string))
            {
                element = new XElement(property.Name, new XAttribute("str", defaultValue.ToString()));
            }
            else
            {
                // 其他类型的默认值（若有需要处理）
                element = new XElement(property.Name, new XAttribute("value", defaultValue.ToString()));
            }

            return element;
        }

        private static void AssignValueToProperty<T>(PropertyInfo property, XElement element, T dataObject)
        {
            // 判断属性类型并进行相应的赋值操作
            if (property.PropertyType == typeof(float))
            {
                var value = element.Attribute("value")?.Value;
                if (value != null)
                {
                    property.SetValue(dataObject, float.Parse(value));
                }
            }
            else if (property.PropertyType == typeof(string))
            {
                var strValue = element.Attribute("str")?.Value;
                if (strValue != null)
                {
                    property.SetValue(dataObject, strValue);
                }
            }
            // 支持更多常见类型
            else if (property.PropertyType == typeof(int))
            {
                var intValue = element.Attribute("value")?.Value;
                if (intValue != null)
                {
                    property.SetValue(dataObject, int.Parse(intValue));
                }
            }
            else if (property.PropertyType == typeof(bool))
            {
                var boolValue = element.Attribute("value")?.Value;
                if (boolValue != null)
                {
                    property.SetValue(dataObject, bool.Parse(boolValue));
                }
            }
            else
            {
                // 默认处理情形：对于未知的类型，记录警告信息并设置默认值
                Console.WriteLine($"Warning: Unsupported type '{property.PropertyType}' for property '{property.Name}'. Using default value.");
                property.SetValue(dataObject, GetDefaultValueForType(property.PropertyType));
            }
        }

        private static object GetDefaultValueForType(Type type)
        {
            if (type == typeof(float))
            {
                return 0f;  // 默认值：0
            }
            if (type == typeof(string))
            {
                return string.Empty;  // 默认值：空字符串
            }
            return null;  // 或者其他默认值
        }

        // 使用 Prefix 来替换原始方法中的路径
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExtendedInfoManager), "TryLoadCharacters")]
        public static bool Prefix_TryLoadCharacters(ref Dictionary<string, CharacterExtendedInfo> infos)
        {

            // 自定义路径（此处可以修改为实际的路径或通过其他方式动态获取）
            string path = BalancePath.TORBalanceModuleDataPath + "/CustomLoadXMLLib/tor_extendedunitproperties.xml"; // 自定义路径

            // 创建一个空的字典用于存储角色扩展信息
            Dictionary<string, CharacterExtendedInfo> unitlist = new Dictionary<string, CharacterExtendedInfo>();
            infos = unitlist;

            try
            {

                // 如果文件存在，则读取并反序列化文件内容
                if (File.Exists(path))
                {
                    foreach (CharacterExtendedInfo item in (new XmlSerializer(typeof(List<CharacterExtendedInfo>)).Deserialize(File.OpenRead(path)) as List<CharacterExtendedInfo>))
                    {
                        // 如果角色信息字典中没有该角色的扩展信息，则将其添加
                        if (!infos.ContainsKey(item.CharacterStringId))
                        {
                            infos.Add(item.CharacterStringId, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果发生异常，记录错误信息并抛出异常
                TORCommon.Log(ex.ToString(), LogLevel.Error);
                throw ex;
            }

            return false;
        }

    }
}


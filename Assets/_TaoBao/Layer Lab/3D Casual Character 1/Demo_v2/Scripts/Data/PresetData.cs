namespace Layer_lab._3D_Casual_Character.Demo2
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 预设数据 ScriptableObject - 用于存储和管理角色预设配置
    /// </summary>
    [CreateAssetMenu(fileName = "PresetData", menuName = "Character/PresetData")]
    public class PresetData : ScriptableObject
    {
        public List<PresetItem> presetItems = new();  // 预设列表

        /// <summary>
        /// 保存预设
        /// </summary>
        /// <param name="index">预设索引</param>
        /// <param name="itemList">部位配置数据</param>
        public void SavePreset(int index, Dictionary<PartType, int> itemList)
        {
            // 删除旧数据后添加新数据
            presetItems.RemoveAll(p => p.index == index);
            presetItems.Add(new PresetItem(index, itemList));
        }

        /// <summary>
        /// 加载预设
        /// </summary>
        /// <param name="index">预设索引</param>
        /// <returns>部位配置数据</returns>
        public Dictionary<PartType, int> LoadPreset(int index)
        {
            var preset = presetItems.Find(p => p.index == index);
            return preset != null ? new Dictionary<PartType, int>(preset.itemList) : new Dictionary<PartType, int>();
        }

        /// <summary>
        /// 清除指定预设
        /// </summary>
        /// <param name="index">预设索引</param>
        public void ClearPreset(int index)
        {
            presetItems.RemoveAll(p => p.index == index);
        }
    }

    /// <summary>
    /// 预设项 - 存储单个预设的配置数据
    /// </summary>
    [Serializable]
    public class PresetItem
    {
        public int index;              // 预设索引
        public List<PartItem> parts = new();  // 部位配置列表

        /// <summary>
        /// 获取部位配置字典
        /// </summary>
        public Dictionary<PartType, int> itemList
        {
            get
            {
                Dictionary<PartType, int> dict = new();
                foreach (var part in parts)
                {
                    dict[part.partType] = part.value;
                }
                return dict;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PresetItem(int index, Dictionary<PartType, int> itemList)
        {
            this.index = index;
            foreach (var kvp in itemList)
            {
                parts.Add(new PartItem(kvp.Key, kvp.Value));
            }
        }
    }

    /// <summary>
    /// 部位项 - 存储单个部位的配置
    /// </summary>
    [Serializable]
    public class PartItem
    {
        public PartType partType;  // 部位类型
        public int value;          // 选中的索引

        public PartItem(PartType partType, int value)
        {
            this.partType = partType;
            this.value = value;
        }
    }
}
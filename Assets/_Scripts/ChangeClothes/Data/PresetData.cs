using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "PresetData", menuName = "ChangeClothes/PresetData")]
public class PresetData : ScriptableObject
{
    public List<PresetItem> presetItems = new();  // 预设列表

    /// <summary>
    /// 加载预设
    /// </summary>
    /// <param name="index">预设索引</param>
    /// <returns>部位配置数据</returns>
    public Dictionary<PartType, int> LoadPreset(int index)
    {
        var preset = presetItems.Find(p => p.index == index);
        if (preset != null)
        {
            return new Dictionary<PartType, int>(preset.itemList);
        }
        return new Dictionary<PartType, int>();
    }

    /// <summary>
    /// 清除指定预设
    /// </summary>
    /// <param name="index">预设索引</param>
    public void RemovePreset(int index)
    {
        presetItems.RemoveAll(p => p.index == index);
    }

}

[Serializable]
public class PresetItem
{
    public int index;              // 预设索引
    public List<PartItem> parts = new();  // 部位配置列表

    public Dictionary<PartType, int> itemList // 解析部位配置字典
    {
        get
        {
            Dictionary<PartType, int> dict = new();
            foreach (var part in parts)
            {
                dict[part.partType] = part.partIndex;
            }
            return dict;
        }
    }
}

[Serializable]
public class PartItem
{
    public PartType partType;  // 部位类型
    public int partIndex;     // 部位索引

    public PartItem(PartType type, int index)
    {
        partType = type;
        partIndex = index;
    }
}

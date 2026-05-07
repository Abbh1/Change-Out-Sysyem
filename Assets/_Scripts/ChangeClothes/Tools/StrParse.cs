using System;
using UnityEngine;

public static class StrParse
{
    /// <summary>
    /// 解析部件名称，返回数组 [基础部位, 编号]
    /// 支持格式: Hair_Black_1 → ["Hair", "1"]
    ///          Eye_1 → ["Eye", "1"]
    /// </summary>
    public static string[] ParsePartName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        int lastUnderscore = name.LastIndexOf('_');
        if (lastUnderscore <= 0 || lastUnderscore == name.Length - 1)
            return null;

        string prefix = name.Substring(0, lastUnderscore);
        string numStr = name.Substring(lastUnderscore + 1);

        if (!int.TryParse(numStr, out _))
            return null;

        string matchedPrefix = null;
        int maxLength = 0;

        foreach (var kv in PartNameMap.PrefixByType)
        {
            if (prefix.StartsWith(kv.Value) && kv.Value.Length > maxLength)
            {
                maxLength = kv.Value.Length;
                matchedPrefix = kv.Value;
            }
        }

        if (matchedPrefix == null)
            return null;

        return new[] { matchedPrefix, numStr };
    }

    /// <summary>
    /// 解析部件名称，返回编号（如果存在）
    /// 支持格式: Hair_Black_1 → 1
    ///          Eye_1 → 1
    /// </summary>
    public static int? ParsePartIndex(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        int lastUnderscore = name.LastIndexOf('_');
        if (lastUnderscore <= 0 || lastUnderscore == name.Length - 1)
            return null;

        string numStr = name.Substring(lastUnderscore + 1);

        if (int.TryParse(numStr, out int index))
            return index;

        return null;
    }


    /// <summary>
    /// 判断字符串是否为数字
    /// </summary>
    public static bool IsNumeric(string s)
    {
        return int.TryParse(s, out _);
    }


    /// <summary>
    /// 将部件名称转换为 PartType（需根据实际命名补充）
    /// </summary>
    public static PartType ParsePartType(string partName)
    {
        // 示例：将 "Hair" 转为 PartType.Hair，"Eyewear" 转为 PartType.Eyewear
        if (Enum.TryParse(partName, out PartType partType))
        {
            return partType;
        }
        return PartType.None;
    }

}

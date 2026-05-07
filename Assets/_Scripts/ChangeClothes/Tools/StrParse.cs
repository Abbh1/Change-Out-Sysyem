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
        if (string.IsNullOrEmpty(name)) return null;

        int lastUnderscore = name.LastIndexOf('_');
        if (lastUnderscore <= 0) return null;

        // 提取编号
        string numStr = name.Substring(lastUnderscore + 1);
        if (!int.TryParse(numStr, out _)) return null;

        // 提取基础部位（第一个_之前的部分）
        int firstUnderscore = name.IndexOf('_');
        if (firstUnderscore <= 0)
        {
            // 没有其他_，说明是 Eye_1 这种格式
            return new[] { name.Substring(0, lastUnderscore), numStr };
        }

        // 返回第一个_之前的部分
        return new[] { name.Substring(0, firstUnderscore), numStr };
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

using System.Collections.Generic;

/// <summary>
/// 角色部位类型枚举
/// </summary>
public enum PartType
{
    None,
    // 面部特征
    Beard,
    Brow,
    Earring,
    Eye,
    Eyewear,
    Hair,
    Mouth,
    // Body,

    // 装备部位
    Back,
    Chest,
    Foot,
    Hand,
    Head,
    Leg,
    Wield_Gear
}


public static class PartNameMap
{
    /// <summary>
    /// PartType 到其基础名称字符串的映射
    /// 用于解析 Hair_Black_1、Wield_Gear_1 这类资源名称
    /// </summary>
    public static readonly Dictionary<PartType, string> PrefixByType =
        new Dictionary<PartType, string>
        {
            // 面部特征
            { PartType.Beard, "Beard" },
            { PartType.Brow, "Brow" },
            { PartType.Earring, "Earring" },
            { PartType.Eye, "Eye" },
            { PartType.Eyewear, "Eyewear" },
            { PartType.Hair, "Hair" },
            { PartType.Mouth, "Mouth" },

            // 装备部位
            { PartType.Back, "Back" },
            { PartType.Chest, "Chest" },
            { PartType.Foot, "Foot" },
            { PartType.Hand, "Hand" },
            { PartType.Head, "Head" },
            { PartType.Leg, "Leg" },
            { PartType.Wield_Gear, "Wield_Gear" }
        };
}
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 部位项标记组件 - 用于标记特殊物品属性
    /// </summary>
    public class CharacterPartItem : MonoBehaviour
    {
        [field: SerializeField] public bool IsOnlyHeadItem { get; set; }  // 是否为"仅头部"物品（穿戴时需隐藏头发）
    }
}
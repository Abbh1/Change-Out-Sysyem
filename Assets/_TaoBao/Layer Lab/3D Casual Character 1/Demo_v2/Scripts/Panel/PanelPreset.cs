using System.Collections.Generic;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 预设面板 - 管理预设的保存和加载
    /// </summary>
    public class PanelPreset : MonoBehaviour
    {
        [field: SerializeField] private PresetSlot PresetSlot { get; set; }  // 预设槽模板
        private List<PresetSlot> _presetSlots = new();                        // 预设槽列表
        [SerializeField] private Transform parent;                             // 预设槽父容器
        
        /// <summary>
        /// 初始化面板 - 创建预设槽实例
        /// </summary>
        public void Init()
        {
            PresetSlot.gameObject.SetActive(false);
            // 创建10个预设槽
            for (var i = 0; i < 10; i++)
            {
                var preset = Instantiate(PresetSlot, parent);
                preset.InitSlot(i);
            }
        }
        
        /// <summary>
        /// 移除预设
        /// </summary>
        public void RemovePreset(int index)
        {
            Destroy(_presetSlots[index].gameObject);
            _presetSlots.RemoveAt(index);
        }
    }
}
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 演示主控制器 - 负责整个系统的初始化
    /// </summary>
    public class DemoControl : MonoBehaviour
    {
        public static DemoControl Instance { get; set; }  // 单例实例
        
        [field: SerializeField] public string ItemImagePath { get; set; }  // 物品图片资源路径
        [field: SerializeField] public PresetData PresetData { get; set; }  // 预设数据 ScriptableObject
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // 初始化角色系统和UI系统
            Demo2Character.Instance.Init();
            UIControl.Instance.Init();
            // 触发随机换装
            Demo2Character.Instance.OnRandomChanged.Invoke();
        }
    }
}
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// UI主控制器 - 管理所有UI面板和交互
    /// </summary>
    public class UIControl : MonoBehaviour
    {
        public static UIControl Instance { get; private set; }  // 单例实例
        public PartType FocusPartTYpe { get; private set; }     // 当前聚焦的部位类型

        [field: SerializeField] public PanelItem PanelItem { get; set; }           // 物品选择面板
        [field: SerializeField] public PanelPreset PanelPreset { get; set; }       // 预设面板
        [field: SerializeField] public PanelAnimation PanelAnimation { get; set; } // 动画选择面板
        [field: SerializeField] private AnimationBar AnimationBar { get; set; }    // 动画栏
        private ItemSlot[] ItemSlot { get; set; }                                   // 所有物品槽位
        [field: SerializeField] public ItemFocusSlot ItemFocusSlot { get; set; }   // 聚焦槽位组件

        public Sprite[] spriteActiveIcons;  // 激活状态图标（显示/隐藏）
        public Sprite[] spriteBgs;          // 背景图（空/有物品）

        public GameObject buttonExport;     // 导出按钮

        private void Awake()
        {
            Instance = this;

            // WebGL平台隐藏导出按钮（不支持）
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                buttonExport.SetActive(false);
            }
        }

        /// <summary>
        /// 设置当前聚焦的部位槽位
        /// </summary>
        public void SetFocusPart(ItemSlot itemSlot)
        {
            if (itemSlot == null)
            {
                ItemFocusSlot.Hide();
                return;
            }

            FocusPartTYpe = itemSlot.PartType;
            ItemFocusSlot.transform.SetParent(itemSlot.transform, false);
            ItemFocusSlot.transform.position = itemSlot.transform.position;
            ItemFocusSlot.Show();
        }

        /// <summary>
        /// 初始化UI系统
        /// </summary>
        public void Init()
        {
            ItemSlot = transform.GetComponentsInChildren<ItemSlot>();

            // 初始化各面板
            AnimationBar.Init();
            PanelItem.Init();
            PanelPreset.Init();
            PanelAnimation.Init();

            // 初始化每个物品槽位
            foreach (var t in ItemSlot)
            {
                t.SetSlot();
            }
        }

        /// <summary>
        /// 随机换装按钮点击
        /// </summary>
        public void OnClick_Random()
        {
            Demo2Character.Instance.OnRandomChanged.Invoke();
        }

        /// <summary>
        /// 导出预制件按钮点击
        /// </summary>
        public void OnClick_Export()
        {
#if UNITY_EDITOR
            Demo2Character.Instance.CharacterPrefabSaver.SaveAsPrefab();
#endif
        }

        /// <summary>
        /// 关闭物品选择面板
        /// </summary>
        public void CloseItemPanel()
        {
            PanelItem.Hide();
        }

        /// <summary>
        /// 从Resources加载Sprite
        /// </summary>
        public static Sprite GetSprite(string spriteName)
        {
            return Resources.Load<Sprite>(spriteName);
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 物品选择面板 - 显示特定部位的所有可选物品
    /// </summary>
    public class PanelItem : MonoBehaviour
    {
        [field: SerializeField] private ItemSelectSlot ItemSelectSlot { get; set; }  // 物品选择槽模板
        private readonly List<ItemSelectSlot> _itemSelectSlots = new ();  // 物品槽列表
        
        [field: SerializeField] private ScrollRect scrollRect;   // 滚动容器
        [SerializeField] private TMP_Text textTitle;             // 标题文本
        [SerializeField] private Transform parent;               // 物品槽父容器
        [SerializeField] private Transform focus;                // 聚焦指示器
        private PartType _myPartType;                            // 当前显示的部位类型

        /// <summary>
        /// 初始化面板 - 创建物品槽实例
        /// </summary>
        public void Init()
        {
            ItemSelectSlot.gameObject.SetActive(false);
                
            // 创建200个物品槽（预设最大数量）
            for (var i = 0; i < 200; i++)
            {
                _itemSelectSlots.Add(Instantiate(ItemSelectSlot, parent));
            }
        }

        /// <summary>
        /// 部位变更回调 - 更新聚焦位置
        /// </summary>
        private void OnPartChanged(PartType partType, int partIndex)
        {
            if(_myPartType != partType) return;
            StartCoroutine(SetFocusCo(partType));
        }
        
        /// <summary>
        /// 设置面板内容
        /// </summary>
        public void SetPanel(List<GameObject> items, PartType partType)
        {
            _myPartType = partType;
            
            focus.gameObject.SetActive(false);
            
            // 重置滚动位置
            scrollRect.verticalNormalizedPosition = 1;
            textTitle.text = $"{partType}";

            // 设置每个物品槽
            for (var i = 0; i < items.Count; i++)
            {
                _itemSelectSlots[i].SetSlot(i, items[i].name, partType);
            }
            
            gameObject.SetActive(true);
            Demo2Character.Instance.OnPartChanged += OnPartChanged;
            StartCoroutine(SetFocusCo(partType));
            
        }

        /// <summary>
        /// 设置聚焦位置协程
        /// </summary>
        private IEnumerator SetFocusCo(PartType partType)
        {
            yield return null;
            var focusIndex = Demo2Character.Instance.CurrentCharacterPartByType(partType).CurrentIndex;
            if (focusIndex == -1)
            {
                focus.gameObject.SetActive(false);
                yield break;
            }
            
            focus.gameObject.SetActive(true);
            focus.transform.position = _itemSelectSlots[focusIndex].transform.position;
        }


        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void Hide()
        {
            Demo2Character.Instance.OnPartChanged -= OnPartChanged;
            foreach (var t in _itemSelectSlots) t.Hide();
            gameObject.SetActive(false);
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 物品选择槽 - 在物品面板中显示单个可选物品
    /// </summary>
    public class ItemSelectSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image imageItem;  // 物品图片
        private PartType _partType;                 // 部位类型
        private int _index;                         // 物品索引

        /// <summary>
        /// 设置槽位内容
        /// </summary>
        public void SetSlot(int index, string partsName, PartType partType)
        {
            _index = index;
            _partType = partType;

            imageItem.sprite = UIControl.GetSprite($"{DemoControl.Instance.ItemImagePath}/ScreenShot/{partsName}");
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 点击选择该物品
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Demo2Character.Instance.OnPartChanged.Invoke(_partType, _index);
        }
    }
}
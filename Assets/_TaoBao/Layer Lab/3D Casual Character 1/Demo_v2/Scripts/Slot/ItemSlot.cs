using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 物品槽位 - 显示单个部位的当前选择状态
    /// </summary>
    public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isHide;           // 是否隐藏该部位
        private Image _imageIcon;       // 显示/隐藏图标
        private Image _imagePicto;      // 默认占位图标
        private Image _imageItem;       // 当前物品图标
        private Image _imageBg;         // 背景图
        [field: SerializeField] public PartType PartType { get; set; }  // 部位类型

        private void Awake()
        {
            // 获取子对象的Image组件
            _imageBg = transform.GetChild(0).GetComponent<Image>();
            _imageItem = _imageBg.transform.GetChild(0).GetComponent<Image>();
            _imagePicto = transform.GetChild(1).GetComponent<Image>();
            _imageIcon = transform.GetChild(2).GetComponent<Image>();
            
            // 从对象名称解析部位类型
            PartType = Enum.Parse<PartType>(gameObject.name);
        }

        /// <summary>
        /// 初始化槽位
        /// </summary>
        public void SetSlot()
        {
            try
            {
                // 如果是必须装备的部位，隐藏显示/隐藏按钮
                _imageIcon.gameObject.SetActive(!Demo2Character.Instance.CurrentCharacterPartByType(PartType).IsOnlyEquip);
                Demo2Character.Instance.OnPartChanged += SetItemImage;
                Demo2Character.Instance.OnPreset += OnActive;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        /// <summary>
        /// 预设应用时激活部位
        /// </summary>
        public void OnActive()
        {
            _isHide = false;
            _imageIcon.sprite = UIControl.Instance.spriteActiveIcons[1];
            Demo2Character.Instance.OnPartActiveChanged.Invoke(PartType, _isHide);
        }
        
        /// <summary>
        /// 点击显示/隐藏按钮
        /// </summary>
        public void OnClick_Active()
        {
            _isHide = !_isHide;
            _imageIcon.sprite = UIControl.Instance.spriteActiveIcons[_isHide ? 0 : 1];
            Demo2Character.Instance.OnPartActiveChanged.Invoke(PartType, _isHide);
        }
        
        /// <summary>
        /// 鼠标点击处理
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 右键点击取消装备（非必须装备部位）
            if (eventData.button == PointerEventData.InputButton.Right && !Demo2Character.Instance.CurrentCharacterPartByType(PartType).IsOnlyEquip)
            {
                Demo2Character.Instance.OnPartChanged.Invoke(PartType, -1);
                return;
            }
            
            if(eventData.button == PointerEventData.InputButton.Right) return;
            
            // 左键点击打开物品选择面板
            UIControl.Instance.PanelItem.SetPanel(Demo2Character.Instance.CurrentPartsType(PartType), PartType);
        }

        /// <summary>
        /// 鼠标进入处理
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            UIControl.Instance.SetFocusPart(this);
        }

        /// <summary>
        /// 鼠标离开处理
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            UIControl.Instance.SetFocusPart(null);
        }

        /// <summary>
        /// 设置物品图标
        /// </summary>
        private void SetItemImage(PartType partsType, int index)
        {
            if(PartType != partsType) return;

            if (index < 0)
            {
                // 未装备状态
                _imagePicto.gameObject.SetActive(true);
                _imageItem.gameObject.SetActive(false);
                _imageBg.sprite = UIControl.Instance.spriteBgs[0];
                return;
            }
            
            // 装备状态
            _imagePicto.gameObject.SetActive(false);
            _imageItem.gameObject.SetActive(true);
            _imageBg.sprite = UIControl.Instance.spriteBgs[1];
            _imageItem.sprite = UIControl.GetSprite($"{DemoControl.Instance.ItemImagePath}/ScreenShot/{Demo2Character.Instance.CurrentCharacterPartByType(partsType).CurrentName()}");
        }
    }
    
}
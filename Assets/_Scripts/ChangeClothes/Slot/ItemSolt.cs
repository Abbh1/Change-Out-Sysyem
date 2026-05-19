using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSolt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool _isHide;                            // 是否隐藏该部位
    [SerializeField] private Image _imageBg;         // 背景图片
    [SerializeField] private Image _imageItem;       // 物品图片
    [SerializeField] private Image _imageSelect;     // 选中图片
    [SerializeField] private Image _imageHide;       // 隐藏图片

    public PartType PartType;       // 部位类型

    public void Awake()
    {
        // 从对象名称解析部位类型
        PartType = Enum.Parse<PartType>(gameObject.name);

        _imageBg = transform.GetChild(0).GetComponent<Image>();
        _imageItem = _imageBg.transform.GetChild(0).GetComponent<Image>();
        _imageSelect = _imageBg.transform.GetChild(1).GetComponent<Image>();
        _imageHide = transform.GetChild(2).GetComponent<Image>();
    }


    #region 初始化

    public void InitSolt()
    {
        var part = AvatarSystem.Instance.CurrentCharacterPartByType(PartType);
        if (part == null)
        {
            Debug.LogError($"未找到部位: {PartType}");
            return;
        }
        
        Button hideButton = _imageHide.GetComponent<Button>();
        if (hideButton == null)
        {
            hideButton = _imageHide.gameObject.AddComponent<Button>();
        }
        hideButton.onClick.RemoveAllListeners();
        hideButton.onClick.AddListener(OnClickHideButton);
        
        _imageHide.gameObject.SetActive(!part.IsOnlyEquip);
        EventController.Instance.OnPartChanged += SetItemImage;
        
        SetItemImage(PartType, part.CurrentIndex);
    }

    private void OnDestroy()
    {
        // 取消订阅事件，避免场景切换后触发已销毁对象的方法
        if (EventController.Instance != null)
        {
            EventController.Instance.OnPartChanged -= SetItemImage;
        }
    }

    /// <summary>
    /// 设置槽位图片
    /// </summary>
    /// <param name="partType">部位类型</param>
    /// <param name="index">索引</param>
    private void SetItemImage(PartType partType, int index)
    {
        // 空值检查：如果对象已销毁或组件不存在，直接返回
        if (this == null || _imageItem == null || _imageBg == null)
        {
            return;
        }
        
        if (PartType != partType) return;

        if (index < 0)
        {
            _imageItem.gameObject.SetActive(false);
            if (_imageHide != null)
            {
                _imageHide.gameObject.SetActive(false);
            }
            if (UIController.Instance != null)
            {
                _imageBg.sprite = UIController.Instance.spriteBgs[0];
            }
            return;
        }

        _imageItem.gameObject.SetActive(true);
        if (_imageHide != null)
        {
            _imageHide.gameObject.SetActive(true);
        }
        if (UIController.Instance != null)
        {
            _imageBg.sprite = UIController.Instance.spriteBgs[1];
        }

        string name = $"{PartType}_{index}";
        string url = $"ChangeClothes/ScreenShot/{name}";
        _imageItem.sprite = UIController.GetSprite(url);
    }

    #endregion


    #region 鼠标事件
    /// <summary>
    /// 鼠标进入槽槽位时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.SetFocusPart(this);
        if (_imageSelect != null)
        {
            _imageSelect.gameObject.SetActive(true);
        }
    }

    public void HideSelect()
    {
        if (_imageSelect != null)
        {
            _imageSelect.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 鼠标退出槽槽位时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // UIController.Instance.SetFocusPart(null);
        // _imageSelect.gameObject.SetActive(false);
    }

    /// <summary>
    /// 鼠标点击槽位时触发 - 取消装备
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 右键点击槽位背景 - 取消装备（非必须装备部位）
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var part = AvatarSystem.Instance.CurrentCharacterPartByType(PartType);
            if (part != null && !part.IsOnlyEquip)
            {
                EventController.Instance.RaisePartChanged(PartType, -1);
                // Debug.Log($"ItemSolt {PartType}: 点击槽位取消装备");
            }
        }
    }

    #endregion


    #region 隐藏/显示事件

    /// <summary>
    /// 点击隐藏按钮时触发 - 隐藏/显示部位
    /// </summary>
    public void OnClickHideButton()
    {
        _isHide = !_isHide;
        Debug.Log($"【点击隐藏按钮】ItemSolt {PartType}: ，当前状态: {_isHide}");
        EventController.Instance.RaiseClickHide(PartType, _isHide);
        _imageHide.sprite = UIController.Instance.spriteActiveIcons[_isHide ? 0 : 1];
        // Debug.Log($"ItemSolt {PartType}: 点击隐藏按钮，状态: {_isHide}");
    }

    #endregion

}
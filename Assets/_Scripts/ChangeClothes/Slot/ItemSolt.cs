using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSolt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isHide;                            // 是否隐藏该部位
    [SerializeField] private Image _imageBg;         // 背景图片
    [SerializeField] private Image _imageIcon;       // 图标图片
    [SerializeField] private Image _imageItem;       // 物品图片
    [SerializeField] private Image _imageSelect;     // 选中图片

    public PartType PartType;       // 部位类型

    public void Awake()
    {
        // 从对象名称解析部位类型
        PartType = Enum.Parse<PartType>(gameObject.name);
    }

    #region 初始化

    

    #endregion

    #region 鼠标事件
    /// <summary>
    /// 鼠标进入槽槽位时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.SetFocusPart(this);
    }

    /// <summary>
    /// 鼠标退出槽槽位时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.SetFocusPart(null);
    }
    #endregion





}

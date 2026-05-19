using UnityEngine;
using UnityEngine.UI;
using ChangeClothes.Avatar;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class PresetSolt : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image imageCharacter;  // 角色图像组件
    private Dictionary<PartType, int> _itemList = new();  // 预设配置数据
    private int _index;                               // 预设索引
    // public bool isPreset;                             // 是否已保存预设

    public void Init(int index)
    {
        _index = index;
        LoadPreset();
    }

    private void LoadPreset()
    {
        _itemList = UIController.Instance.presetData.LoadPreset(_index);
        if (_itemList.Count == 0)
        {
            imageCharacter.sprite = null;  // 没有预设时清空图片
            return;
        }
        string url = $"ChangeClothes/Preset/{_index}";
        imageCharacter.sprite = UIController.GetSprite(url);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (var item in _itemList)
        {
            EventController.Instance.RaisePartChanged(item.Key, item.Value);
        }
    }

}

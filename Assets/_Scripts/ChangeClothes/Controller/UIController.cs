using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public PartType FocusPartType { get; private set; }

    [Header("UI组件")]
    [SerializeField] private GameObject itemFocusSlot;      // 物品焦点槽位


    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 设置当前聚焦的部位槽位
    /// </summary>
    /// <param name="itemSlot"></param>
    public void SetFocusPart(ItemSolt itemSlot)
    {
        if (itemSlot == null)
        {
            itemFocusSlot.SetActive(false);
            return;
        }

        FocusPartType = itemSlot.PartType;
        itemFocusSlot.transform.SetParent(itemSlot.transform, false);
        itemFocusSlot.transform.position = itemSlot.transform.position;
        itemFocusSlot.SetActive(true);
    }


}

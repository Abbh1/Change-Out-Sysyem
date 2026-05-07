using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 聚焦槽位 - 显示在物品槽上方，提供快速切换按钮
    /// </summary>
    public class ItemFocusSlot : MonoBehaviour
    {
        /// <summary>
        /// 点击下一个按钮
        /// </summary>
        public void OnClick_Next()
        {
            Demo2Character.Instance.OnNextPart.Invoke();
        }

        /// <summary>
        /// 点击上一个按钮
        /// </summary>
        public void OnClick_Previous()
        {
            Demo2Character.Instance.OnPreviousPart.Invoke();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
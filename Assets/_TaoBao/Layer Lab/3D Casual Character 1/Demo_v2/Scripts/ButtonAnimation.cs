using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 动画按钮 - 显示单个动画选项
    /// </summary>
    public class ButtonAnimation : MonoBehaviour
    {
        [SerializeField] private TMP_Text textAnimationName;  // 动画名称显示
        [SerializeField] private Button button;                 // 按钮组件
        private int _index;                                     // 动画索引

        /// <summary>
        /// 设置按钮属性
        /// </summary>
        public void SetButton(int index, AnimationClip clip)
        {
            _index = index;
            textAnimationName.text = clip.name;
            button.onClick.AddListener(() =>
            {
                UIControl.Instance.PanelAnimation.PlayAnimationByIndex(_index);
            });
        }
    }
}
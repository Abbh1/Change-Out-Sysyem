using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 动画栏 - 显示当前动画并提供切换控制
    /// </summary>
    public class AnimationBar : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text textAnimation;  // 当前动画名称显示
        private bool _isEnter;                              // 鼠标是否在栏上

        /// <summary>
        /// 初始化动画栏
        /// </summary>
        public void Init()
        {
            textAnimation.text = UIControl.Instance.PanelAnimation.CurrentAnimationName;
            UIControl.Instance.PanelAnimation.OnChangeAnimation += OnChangeAnimation;
        }


        private void Update()
        {
            // 鼠标不在动画栏上时，点击空白处关闭动画面板
            if (!_isEnter)
            {
                if (Input.GetMouseButton(0)) SetAnimationPanel(false);
            }
        }

        /// <summary>
        /// 动画变更回调
        /// </summary>
        private void OnChangeAnimation(string animationName)
        {
            textAnimation.text = animationName;
        }
        
        /// <summary>
        /// 点击左箭头切换上一个动画
        /// </summary>
        public void OnClick_Left()
        {
            UIControl.Instance.PanelAnimation.PreviousAnimation();
            SetAnimationPanel(false);
        }

        /// <summary>
        /// 点击右箭头切换下一个动画
        /// </summary>
        public void OnClick_Right()
        {
            UIControl.Instance.PanelAnimation.NextAnimation();
            SetAnimationPanel(false);
        }
        
        /// <summary>
        /// 点击动画栏切换面板显示状态
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (UIControl.Instance.PanelAnimation.IsShow)
            {
                SetAnimationPanel(false);
            }
            else
            {
                SetAnimationPanel(true);
            }
        }

        /// <summary>
        /// 设置动画面板显示状态
        /// </summary>
        private void SetAnimationPanel(bool isOn)
        {
            if (isOn)
            {
                UIControl.Instance.PanelAnimation.Show();
            }
            else
            {
                UIControl.Instance.PanelAnimation.Close();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isEnter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isEnter = false;
        }
    }
}
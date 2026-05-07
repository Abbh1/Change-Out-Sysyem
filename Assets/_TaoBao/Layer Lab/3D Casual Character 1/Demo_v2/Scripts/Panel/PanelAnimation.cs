using System;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 动画选择面板 - 管理动画切换和播放
    /// </summary>
    public class PanelAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationClip[] animationClip;  // 可用动画片段列表
        [SerializeField] private ButtonAnimation buttonAnimation; // 按钮模板
        [SerializeField] private Transform buttonParent;          // 按钮父容器
        public bool IsShow { get; set; }                          // 面板是否显示
        private int CurrentAnimationIndex { get; set; }           // 当前动画索引
        public Action<string> OnChangeAnimation { get; set; }     // 动画变更事件
        public string CurrentAnimationName => animationClip[CurrentAnimationIndex].name;  // 当前动画名称
        
        /// <summary>
        /// 初始化面板 - 创建动画选择按钮
        /// </summary>
        public void Init()
        {
            for (var i = 0; i < animationClip.Length; i++)
            {
                var button = Instantiate(buttonAnimation, buttonParent);
                button.SetButton(i, animationClip[i]);
            }
    
            buttonAnimation.gameObject.SetActive(false);
        }

        /// <summary>
        /// 根据索引播放动画
        /// </summary>
        public void PlayAnimationByIndex(int index)
        {
            CurrentAnimationIndex = index;
            ChangeAnimation();
            Close();
        }

        /// <summary>
        /// 上一个动画
        /// </summary>
        public void PreviousAnimation()
        {
            if (CurrentAnimationIndex <= 0)
            {
                CurrentAnimationIndex = animationClip.Length - 1;
            }
            else
            {
                CurrentAnimationIndex--;
            }
            
            ChangeAnimation();
        }
        
        /// <summary>
        /// 下一个动画
        /// </summary>
        public void NextAnimation()
        {
            if (CurrentAnimationIndex >= animationClip.Length - 1)
            {   
                CurrentAnimationIndex = 0;
            }
            else
            {
                CurrentAnimationIndex++;
            }

            ChangeAnimation();
        }

        /// <summary>
        /// 切换动画
        /// </summary>
        private void ChangeAnimation()
        {
            OnChangeAnimation.Invoke(animationClip[CurrentAnimationIndex].name);
            Demo2Character.Instance.PlayAnimation(animationClip[CurrentAnimationIndex]);
        }
        
        private void SetActive()
        {
            IsShow = !IsShow;
            gameObject.SetActive(IsShow);
        }
        
        public void Show()
        {
            gameObject.SetActive(IsShow = true);
        }
        
        public void Close()
        {
            gameObject.SetActive(IsShow = false);
        }
    }
}
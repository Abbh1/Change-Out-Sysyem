using UnityEngine;
using ChangeClothes.Avatar;
using System;

namespace ChangeClothes.Avatar
{
    /// <summary>
    /// 角色部件（头发 / 眼睛 / 衣服等）
    /// 自身即为部件物体，名称格式：Hair_1
    /// </summary>
    public class CharacterPart : MonoBehaviour
    {
        public PartType PartType { get; set; }        // 部件类型
        public bool IsOnlyEquip { get; set; }                 // 是否独占
        public float EquipChance { get; set; } = 50f;        // 随机概率

        public int CurrentIndex { get; set; } = -1;   // 当前索引
        public int CurrentMaxLen { get; set; }        // 最大数量

        private EventController Event => EventController.Instance;
        private AvatarSystem Avatar => AvatarSystem.Instance;
        private UIController UI => UIController.Instance;

        #region Unity

        private void Awake()
        {
            UpdateMaxLen();
        }

        private void OnEnable()
        {
            if (!Event) return;
            Event.OnRandomRequested += RandomPart;
            Event.OnNextPartRequested += NextPart;
            Event.OnPreviousPartRequested += PrevPart;
        }

        private void OnDisable()
        {
            if (!Event) return;
            Event.OnRandomRequested -= RandomPart;
            Event.OnNextPartRequested -= NextPart;
            Event.OnPreviousPartRequested -= PrevPart;
        }

        #endregion

        #region 事件

        private void RandomPart()
        {
            if (!CanChange()) return;
            CurrentIndex = UnityEngine.Random.Range(0, CurrentMaxLen);
            Apply();
        }

        private void NextPart()
        {
            if (!CanChange()) return;
            CurrentIndex = (CurrentIndex + 1) % CurrentMaxLen;
            Apply();
        }

        private void PrevPart()
        {
            if (!CanChange()) return;
            CurrentIndex = (CurrentIndex - 1 + CurrentMaxLen) % CurrentMaxLen;
            Apply();
        }

        #endregion

        /// <summary>
        /// 通知换装系统
        /// </summary>
        public void Apply()
        {
            if (CurrentIndex < 0 || CurrentIndex >= CurrentMaxLen) return;
            Event?.RaisePartChanged(PartType, CurrentIndex);
        }

        #region 私有

        private bool CanChange() =>
            UI && UI.FocusPartType == PartType && CurrentMaxLen > 0;

        private void UpdateMaxLen()
        {
            if (Avatar) CurrentMaxLen = Avatar.GetPartCount(PartType);
        }

        #endregion
    }
}
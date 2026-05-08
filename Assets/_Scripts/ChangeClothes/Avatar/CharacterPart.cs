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
        public bool IsOnlyEquip { get; set; }                 // 是否必须装备
        public float EquipChance { get; set; } = 50f;        // 随机概率

        public int CurrentIndex { get; set; } = -1;   // 当前索引
        public int CurrentMaxLen { get; set; }        // 最大数量

        private EventController Event => EventController.Instance;
        private AvatarSystem Avatar => AvatarSystem.Instance;
        private UIController UI => UIController.Instance;

        #region Unity

        private void Awake()
        {
        }

        private void Start()
        {
            // 再次确认初始化
            UpdateMaxLen();
        }

        private void OnEnable()
        {
            if (!Event) return;
            Event.OnRandomRequested += RandomPart;
            Event.OnNextPartRequested += NextPart;
            Event.OnPreviousPartRequested += PrevPart;
            Event.OnClickHided += HandleClickHided;

            // 重新获取最大数量
            UpdateMaxLen();
        }

        private void OnDisable()
        {
            if (!Event) return;
            Event.OnRandomRequested -= RandomPart;
            Event.OnNextPartRequested -= NextPart;
            Event.OnPreviousPartRequested -= PrevPart;
            Event.OnClickHided -= HandleClickHided;
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
            CurrentIndex++;
            if (CurrentIndex >= CurrentMaxLen)
            {
                CurrentIndex = 1; // 到达最大值后回到1，而不是0
            }
            Apply();
        }

        private void PrevPart()
        {
            if (!CanChange()) return;
            CurrentIndex = (CurrentIndex - 1 + CurrentMaxLen) % CurrentMaxLen;
            Apply();
        }

        /// <summary>
        /// 处理点击隐藏事件
        /// </summary>
        private void HandleClickHided(PartType partType, bool isHide)
        {
            if (partType != PartType) return;
            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            if (smr != null) smr.enabled = isHide;
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

        private bool CanChange() => UI && UI.FocusPartType == PartType && CurrentMaxLen > 0;

        private void UpdateMaxLen()
        {
            if (PartType == PartType.None)
            {
                Debug.LogWarning($"CharacterPart {gameObject.name}: PartType 未设置");
                return;
            }
            int count = Avatar.GetPartCount(PartType);
            CurrentMaxLen = count;
        }

        /// <summary>
        /// 延迟初始化最大数量
        /// </summary>
        private void UpdateMaxLenDelayed()
        {
            UpdateMaxLen();
        }

        #endregion
    }
}
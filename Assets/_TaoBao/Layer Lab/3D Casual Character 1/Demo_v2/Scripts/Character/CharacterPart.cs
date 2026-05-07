using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 角色部位组件 - 管理单个部位的所有可选项
    /// </summary>
    public class CharacterPart : MonoBehaviour
    {
        [field: SerializeField] public PartType PartType { get; set; } = PartType.None;  // 部位类型
        public List<GameObject> CurrentPartsObjects { get; set; }= new();  // 该部位的所有可选对象
        [field: SerializeField] public bool IsOnlyEquip { get; set; }  // 是否必须装备（不能隐藏）
        [field: SerializeField] public float EquipChance { get; set; } = 50f;  // 随机时装备概率
        public int CurrentIndex { get; private set; } = -1;  // 当前选中的索引（-1表示未装备）

        /// <summary>
        /// 获取当前选中部位的名称
        /// </summary>
        public string CurrentName() => CurrentIndex == -1 ? "" : CurrentPartsObjects[CurrentIndex].name;
        private int CurrentMaxIndex => CurrentPartsObjects.Count - 1;  // 最大索引
        private List<CharacterPartItem> _characterPartItems = new();  // 部位项列表（用于特殊标记）
        
        private void Awake()
        {
            // 从对象名称解析部位类型
            PartType = Enum.Parse<PartType>(gameObject.name);
            _characterPartItems = transform.GetComponentsInChildren<CharacterPartItem>().ToList();
        }

        /// <summary>
        /// 初始化部位 - 收集所有子对象并注册事件监听
        /// </summary>
        public void SetPart()
        {
            // 收集所有子对象并禁用它们
            foreach (Transform child in transform)
            {
                CurrentPartsObjects.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }

            // 注册事件监听
            Demo2Character.Instance.OnPartActiveChanged += OnChangeActive;
            Demo2Character.Instance.OnPartChanged += OnPartChanged;
            Demo2Character.Instance.OnRandomChanged += OnRandomChanged;
            Demo2Character.Instance.OnNextPart += OnNextPart;
            Demo2Character.Instance.OnPreviousPart += OnPreviousPart;
        }

        /// <summary>
        /// 设置当前部位显示状态
        /// </summary>
        public void SetParts()
        {
            HideAllParts();

            if(CurrentIndex < 0) return;
            CurrentPartsObjects[CurrentIndex].SetActive(true);
        }

        /// <summary>
        /// 隐藏所有部位对象
        /// </summary>
        private void HideAllParts()
        {
            foreach (Transform child in transform) child.gameObject.SetActive(false);
        }
        
        
        
        #region 事件处理
        
        /// <summary>
        /// 切换到下一个部位选项
        /// </summary>
        private void OnNextPart()
        {
            if(UIControl.Instance.FocusPartTYpe != PartType) return;
            
            if (CurrentIndex < CurrentMaxIndex)
            {
                CurrentIndex++;
            }
            else
            {
                CurrentIndex = 0;   // 循环回到第一个
            }

            SetParts();
            Demo2Character.Instance.OnPartChanged.Invoke(PartType, CurrentIndex);
        }
        
        /// <summary>
        /// 切换到上一个部位选项
        /// </summary>
        private void OnPreviousPart()
        {
            if(UIControl.Instance.FocusPartTYpe != PartType) return;
            
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
            }
            else
            {
                CurrentIndex = CurrentMaxIndex;   // 循环回到最后一个
            }
            
            SetParts();
            Demo2Character.Instance.OnPartChanged.Invoke(PartType, CurrentIndex);
        }
        
        /// <summary>
        /// 切换部位的激活状态（显示/隐藏）
        /// </summary>
        /// <param name="type">部位类型</param>
        /// <param name="active">是否隐藏（true=隐藏）</param>
        private void OnChangeActive(PartType type, bool active)
        {
            if(type != PartType) return;
            gameObject.SetActive(!active);
        }

        /// <summary>
        /// 通过类型和索引切换部位
        /// </summary>
        /// <param name="type">部位类型</param>
        /// <param name="index">部位索引</param>
        private void OnPartChanged(PartType type, int index)
        {
            if(type != PartType) return;
            CurrentIndex = index;
            SetParts();
        }

        /// <summary>
        /// 随机选择部位
        /// </summary>
        private void OnRandomChanged()
        {
            try
            {
                HideAllParts();

                // 如果不是必须装备，根据概率决定是否装备
                if (!IsOnlyEquip && EquipChance * 0.01f <= Random.value)
                {
                    Demo2Character.Instance.OnPartChanged(PartType, -1);
                    CurrentIndex = -1;
                    return;
                }

                // 随机选择一个部位
                CurrentIndex = Random.Range(0, CurrentPartsObjects.Count);

                // 如果选择的是头部且是"仅头部"物品，需要隐藏头发
                if (PartType == PartType.Head && _characterPartItems[CurrentIndex].IsOnlyHeadItem && 
                    Demo2Character.Instance.CurrentCharacterPartByType(PartType.Hair).CurrentIndex != -1)
                {
                    Demo2Character.Instance.OnPartChanged(PartType.Hair, -1);
                }
            
                SetParts();
                Demo2Character.Instance.OnPartChanged.Invoke(PartType, CurrentIndex);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        #endregion
        
 

    }
}
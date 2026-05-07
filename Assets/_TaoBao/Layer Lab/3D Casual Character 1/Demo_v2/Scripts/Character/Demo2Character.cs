using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 角色部位类型枚举
    /// </summary>
    public enum PartType
    {
        None,           // 无
        //A - 面部特征
        Beard,          // 胡须
        Brow,           // 眉毛
        Earring,        // 耳环
        Eye,            // 眼睛
        Eyewear,        // 眼镜/眼部配饰
        Hair,           // 头发
        Mouth,          // 嘴巴
        Body,           // 身体

        //B - 装备部位
        Back,           // 背部（背包）
        Chest,          // 上衣
        Foot,           // 鞋子
        Hand,           // 手套
        Head,           // 头部（帽子等）
        Leg,            // 裤子
        Wield_Gear_Left,    // 左手装备
        Wield_Gear_Right    // 右手装备
    }
    
    /// <summary>
    /// 角色主控制器 - 管理所有部位和动画
    /// </summary>
    public class Demo2Character : MonoBehaviour
    {
        public static Demo2Character Instance { get; private set; }  // 单例实例
        private CharacterPart[] _parts;         // 所有角色部位组件
        private Animator _animator;             // 角色动画器

        // 事件声明
        public Action<PartType, int> OnPartChanged;       // 部位变更事件 (部位类型, 索引)
        public Action<PartType, bool> OnPartActiveChanged; // 部位激活状态变更事件 (部位类型, 是否隐藏)
        public Action OnRandomChanged;            // 随机换装事件
        public Action OnNextPart;                 // 下一个部位事件
        public Action OnPreviousPart;             // 上一个部位事件
        public Action OnPreset;                   // 预设应用事件
        
#if UNITY_EDITOR
        public CharacterPrefabSaver CharacterPrefabSaver { get; set; }  // 编辑器预制件保存器
#endif

        /// <summary>
        /// 根据部位类型获取对应的CharacterPart组件
        /// </summary>
        public CharacterPart CurrentCharacterPartByType(PartType type) => _parts.FirstOrDefault(p => p.PartType == type);
        
        private void Awake()
        {
            // 单例模式初始化
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// 初始化角色系统
        /// </summary>
        public void Init()
        {
#if UNITY_EDITOR
            CharacterPrefabSaver = GetComponent<CharacterPrefabSaver>();
#endif
            _parts = transform.GetComponentsInChildren<CharacterPart>();
            _animator = transform.GetComponentInChildren<Animator>();
            // 初始化每个部位
            foreach (var part in _parts) part.SetPart();
        }

 
        /// <summary>
        /// 获取所有部位的当前状态（用于保存预设）
        /// </summary>
        /// <returns>部位类型与当前索引的字典</returns>
        public Dictionary<PartType, int> CurrentPartsTypeAndNameList()
        {
            var saveData = new Dictionary<PartType, int>();
            for (var i = 0; i < _parts.Length; i++)
            {
                saveData.Add(_parts[i].PartType, _parts[i].CurrentIndex); 
            }

            return saveData;
        }
        
        /// <summary>
        /// 根据类型获取该部位的所有可选对象列表
        /// </summary>
        /// <param name="type">部位类型</param>
        /// <returns>该部位的所有游戏对象</returns>
        public List<GameObject> CurrentPartsType(PartType type)
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                if (_parts[i].PartType == type)
                {
                    return _parts[i].CurrentPartsObjects;
                }
            }
            return new List<GameObject>();
        }
        
        /// <summary>
        /// 播放角色动画
        /// </summary>
        /// <param name="clip">要播放的动画片段</param>
        public void PlayAnimation(AnimationClip clip)
        {
            _animator.CrossFadeInFixedTime(clip.name, 0.25f);
        }
        
        
        private void OnDisable()
        {
            // 清理事件引用，防止内存泄漏
            OnPartChanged = null;
            OnPartActiveChanged = null;
            OnRandomChanged = null;
        }

        private void Update()
        {
            #if UNITY_EDITOR
            // 编辑器中按S键保存预制件
            if (Input.GetKeyDown(KeyCode.S))
            {
                CharacterPrefabSaver.SaveAsPrefab();
            }
            #endif
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using System.Linq;
using ChangeClothes.Avatar;
using Unity.VisualScripting;

public class AvatarSystem : MonoBehaviour
{
    public static AvatarSystem Instance { get; private set; }

    [Header("生成位置")]
    public Transform spawnPoint; // 角色生成位置

    [Header("角色模型配置")]
    public GameObject characterModelPrefab; // 角色模型预制体(含所有部位)
    public GameObject characterTargetPrefab; // 角色目标预制体（只含骨骼和初始模型）

    [Header("角色部件")]
    [SerializeField] private List<CharacterPart> _parts = new();

    [Header("默认装备配置")]
    public List<DefaultAvatarItem> defaultAvatarItems = new List<DefaultAvatarItem>(); // 默认装备列表（部位类型 -> 索引）

    // 换装参数 //
    private GameObject characterTarget; // 角色目标模型
    private Transform characterSourceTrans; // 角色源模型变换组件
    private Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>> characterData; // 角色数据字典（部位类型 -> 编号 -> SkinnedMeshRenderer）
    private Dictionary<PartType, SkinnedMeshRenderer> characterSmr; // 角色当前SkinnedMeshRenderer字典（部位类型 -> SkinnedMeshRenderer）
    private Dictionary<PartType, int> currentPartNumbers; // 当前选中部位编号
    private Dictionary<string, Transform> boneMap; // 骨骼映射字典（骨骼名称 -> 骨骼变换组件）
    private Transform[] characterHips; // 角色骨骼数组


    #region Unity生命周期
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        characterData = new Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>>();
        characterSmr = new Dictionary<PartType, SkinnedMeshRenderer>();
        currentPartNumbers = new Dictionary<PartType, int>();
        boneMap = new Dictionary<string, Transform>();
    }

    private void OnEnable()
    {
        if (EventController.Instance != null) EventController.Instance.OnPartChanged += ChangePart;
    }

    private void OnDisable()
    {
        if (EventController.Instance != null) EventController.Instance.OnPartChanged -= ChangePart;
    }
    #endregion


    #region 初始化模型
    /// <summary>
    /// 初始化角色模型（ChangeClothes 场景使用）
    /// </summary>
    public void Init()
    {
        if (characterModelPrefab == null || characterTargetPrefab == null)
        {
            Debug.LogError("角色模型或目标预制体未设置！");
            return;
        }

        InitSourceCharacter(); // 初始化源模型
        InitTargetCharacter(); // 初始化目标模型
        SaveData(characterSourceTrans, characterData, characterSmr, characterTarget);
    }

    /// <summary>
    /// 初始化角色模型（非 ChangeClothes 场景使用，从已有角色读取）
    /// </summary>
    /// <param name="existingCharacter">场景中已存在的角色对象</param>
    public void InitFromExistingCharacter(GameObject existingCharacter)
    {
        if (characterModelPrefab == null)
        {
            Debug.LogError("角色模型预制体未设置！");
            return;
        }

        if (existingCharacter == null)
        {
            Debug.LogError("传入的角色对象为空！");
            return;
        }

        characterTarget = existingCharacter;
        
        // 查找 Bone 子物体
        Transform boneRoot = characterTarget.transform.Find("Bone");
        if (boneRoot != null)
        {
            characterHips = boneRoot.GetComponentsInChildren<Transform>(true);
        }
        else
        {
            Debug.LogWarning("目标模型中未找到 Bone 节点，将查找所有子物体");
            characterHips = characterTarget.GetComponentsInChildren<Transform>(true);
        }

        CreateBoneDictionary();
        InitSourceCharacter(); // 初始化源模型（用于获取换装数据）
        SaveData(characterSourceTrans, characterData, characterSmr, characterTarget);
    }

    /// <summary>
    /// 初始化角色源模型，隐藏在场景中
    /// </summary>
    private void InitSourceCharacter()
    {
        GameObject go = Instantiate(characterModelPrefab); // 生成完整角色模型
        characterSourceTrans = go.transform; // 获取角色源模型Transform组件
        go.SetActive(false); // 隐藏角色源模型
    }

    /// <summary>
    /// 初始化角色目标模型，显示在场景中，并获取骨骼信息
    /// </summary>
    private void InitTargetCharacter()
    {
        if (characterTargetPrefab == null)
        {
            Debug.LogError("未设置角色目标预制体！");
            return;
        }

        characterTarget = Instantiate(characterTargetPrefab);
        characterTarget.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        characterTarget.transform.localScale = spawnPoint.localScale;
        characterTarget.SetActive(true);

        // 查找 Bone 子物体
        Transform boneRoot = characterTarget.transform.Find("Bone");
        if (boneRoot != null)
        {
            // 只获取 Bone 下的骨骼
            characterHips = boneRoot.GetComponentsInChildren<Transform>(true);
        }
        else
        {
            Debug.LogWarning("目标模型中未找到 Bone 节点，将查找所有子物体");
            characterHips = characterTarget.GetComponentsInChildren<Transform>(true);
        }

        // 创建骨骼字典
        CreateBoneDictionary();
    }


    /// <summary>
    /// 创建骨骼字典，优化骨骼查找
    /// </summary>
    private void CreateBoneDictionary()
    {
        boneMap.Clear();
        if (characterHips == null || characterHips.Length == 0)
        {
            Debug.LogWarning("未找到骨骼信息！");
            return;
        }

        foreach (var bone in characterHips)
        {
            // Debug.Log($"添加骨骼到字典: {bone.name}");
            if (bone != null && !string.IsNullOrEmpty(bone.name))
            {
                boneMap[bone.name] = bone;
            }
        }

    }

    /// <summary>
    /// 初始化默认装备
    /// </summary>
    public void InitDefaultAvatar()
    {
        if (defaultAvatarItems.Count == 0) InitDefaultItems();
        foreach (var item in defaultAvatarItems)
        {
            EventController.Instance.RaisePartChanged(item.partType, item.index);
            // ChangeMesh(item.partType, item.index, characterData, characterSmr, characterHips);
        }
    }

    /// <summary>
    /// 初始化初始装备
    /// </summary>
    private void InitDefaultItems()
    {
        defaultAvatarItems.Clear();
        foreach (PartType item in Enum.GetValues(typeof(PartType)))
        {
            if (item == PartType.None) continue;

            defaultAvatarItems.Add(new DefaultAvatarItem
            {
                partType = item,
                index = 1
            });
        }
    }
    #endregion


    #region 保存数据
    /// <summary>
    /// 保存角色数据，构建部位与SkinnedMeshRenderer的映射关系
    /// </summary>
    /// <param name="sourceTrans">源角色的 Transform（包含 Parts 节点）</param>
    /// <param name="data">存储 PartType → 编号 → SkinnedMeshRenderer 的字典</param>
    /// <param name="smr">存储 PartType → 对应部位 Renderer 的字典</param>
    /// <param name="target">用于挂载部位容器的父节点</param>
    private void SaveData(
     Transform sourceTrans,
     Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>> data,
     Dictionary<PartType, SkinnedMeshRenderer> smr,
     GameObject target)
    {
        if (sourceTrans == null) return;

        // 查找 Parts 根节点(全部部位)
        Transform partsRoot = sourceTrans.Find("Parts");
        if (partsRoot == null)
        {
            Debug.LogError("源模型中未找到 Parts 节点！");
            return;
        }

        // 递归收集所有 SkinnedMeshRenderer（包括 PartA/PartB 下的）
        SkinnedMeshRenderer[] parts = partsRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var part in parts)
        {
            // 解析部件名称（支持 Beard_Black_1、Eyewear_1 格式）
            string[] names = StrParse.ParsePartName(part.name);
            if (names == null || names.Length < 2) continue;

            string partName = names[0];
            string numStr = names[1];

            // 转换为 PartType（需实现 PartType 解析逻辑）
            PartType partType = StrParse.ParsePartType(partName);
            if (partType == PartType.None) continue;

            // 转换编号为 int
            if (!int.TryParse(numStr, out int num)) continue;

            // 初始化数据结构
            if (!data.ContainsKey(partType))
            {
                GameObject go = new GameObject();
                go.name = partName + "_" + num;
                go.transform.SetParent(target.transform, false);

                // ✅ 添加 CharacterPart
                var cp = go.AddComponent<CharacterPart>();
                cp.PartType = StrParse.ParsePartType(partName);
                cp.CurrentIndex = num;
                cp.IsOnlyEquip = false;
                // ✅ 关键：加入 _parts 列表
                _parts.Add(cp);


                smr[partType] = go.AddComponent<SkinnedMeshRenderer>();
                data[partType] = new Dictionary<int, SkinnedMeshRenderer>();
            }

            data[partType][num] = part;
        }

    }

    #endregion


    #region 换装实现
    /// <summary>
    /// 切换角色部位的装备
    /// </summary>
    /// <param name="part">部位类型</param>
    /// <param name="num">编号</param>
    /// <param name="data">数据字典</param>
    /// <param name="smr">SkinnedMeshRenderer字典</param>
    /// <param name="hips">骨骼数组</param>
    private bool ChangeMesh(
    PartType part,
    int num,
    Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>> data,
    Dictionary<PartType, SkinnedMeshRenderer> smr,
    Transform[] hips)
    {
        // 特殊处理：num为-1表示取消装备，隐藏该部位
        if (num < 0)
        {
            if (smr.ContainsKey(part) && smr[part] != null)
            {
                smr[part].sharedMesh = null;  // 清空网格
                smr[part].materials = new Material[0];  // 清空材质
                currentPartNumbers[part] = -1;
                Debug.Log($"取消装备: {part}");
                return true;
            }
            return false;
        }

        // 空值检查
        if (data == null || !data.ContainsKey(part) || !data[part].ContainsKey(num))
        {
            Debug.LogWarning($"ChangeMesh: 找不到对应的部位或编号 - 部位: {part}, 编号: {num}");
            return false;
        }

        SkinnedMeshRenderer skm = data[part][num];
        if (skm == null)
        {
            Debug.LogWarning($"ChangeMesh: 找不到对应的SkinnedMeshRenderer - 部位: {part}, 编号: {num}");
            return false;
        }

        // 从源模型的SkinnedMeshRenderer中获取骨骼并映射到目标模型的骨骼上
        var hipMap = hips.ToDictionary(h => h.name, h => h);

        List<Transform> bones = new List<Transform>(skm.bones.Length);

        foreach (var bone in skm.bones)
        {
            if (hipMap.TryGetValue(bone.name, out var mappedBone))
            {
                bones.Add(mappedBone);
            }
            else
            {
                Debug.LogWarning($"[SkinnedMesh] 未找到对应的骨骼: {bone.name}", bone);
            }
        }

        // 换装实现
        if (smr.ContainsKey(part) && smr[part] != null)
        {
            var targetSmr = smr[part];
            if (bones.Count > 0)
            {
                targetSmr.bones = bones.ToArray();      // 更新骨骼
                targetSmr.rootBone = bones[0];          // 更新根骨骼
            }
            targetSmr.materials = skm.materials;    // 更新材质
            targetSmr.sharedMesh = skm.sharedMesh;  // 更新网格
            currentPartNumbers[part] = num;
            // CurrentCharacterPartByType(part).CurrentIndex = num;  // 更新CharacterPart的CurrentIndex
            // Debug.Log($"换装成功: {part} - {num}");
            return true;
        }
        else
        {
            Debug.LogWarning($"ChangeMesh: 找不到对应的目标SkinnedMeshRenderer - 部位: {part}");
            return false;
        }
    }

    /// <summary>
    /// 通过字符串切换部位
    /// </summary>
    public void ChangeMesh(string partName, int index)
    {
        if (Enum.TryParse(partName, out PartType partType))
        {
            if (!ChangeMesh(partType, index, characterData, characterSmr, characterHips))
            {
                Debug.LogWarning($"AvatarSystem.ChangeMesh: 无效部位或索引 - {partName}:{index}");
            }
        }
        else
        {
            Debug.LogWarning($"无效的部位名称: {partName}");
        }
    }

    /// <summary>
    /// 通过部位类型切换部位
    /// </summary>
    public void ChangePart(PartType partType, int index)
    {
        ChangeMesh(partType, index, characterData, characterSmr, characterHips);
    }

    #endregion


    #region 工具方法
    /// <summary>
    /// 根据部位类型获取对应的 CharacterPart 组件
    /// </summary>
    public CharacterPart CurrentCharacterPartByType(PartType type) => _parts.FirstOrDefault(p => p.PartType == type);

    /// <summary>
    /// 获取指定部位的所有可用编号
    /// </summary>
    public List<int> GetAvailableIndexes(PartType partType)
    {
        List<int> indexes = new List<int>();

        if (characterData.TryGetValue(partType, out var partDict))
        {
            indexes.AddRange(partDict.Keys);
        }

        return indexes;
    }

    /// <summary>
    /// 获取指定部位的可用数量
    /// </summary>
    public int GetPartCount(PartType partType)
    {
        return characterData.TryGetValue(partType, out var partDict) ? partDict.Count : 0;
    }

    /// <summary>
    /// 获取所有部位的可用数量
    /// </summary>
    public Dictionary<PartType, int> GetAllPartCounts()
    {
        return characterData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
    }

    /// <summary>
    /// 获取当前所有部位的配置数据
    /// </summary>
    public Dictionary<PartType, int> GetCurrentEquipConfig()
    {
        return new Dictionary<PartType, int>(currentPartNumbers);
    }

    /// <summary>
    /// 获取角色目标对象的 Transform（用于场景切换时移动角色）
    /// </summary>
    public Transform GetCharacterTargetTransform()
    {
        return characterTarget != null ? characterTarget.transform : null;
    }

    #endregion

}
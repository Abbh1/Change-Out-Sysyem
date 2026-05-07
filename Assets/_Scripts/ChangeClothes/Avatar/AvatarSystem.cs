using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using System.Linq;
using ChangeClothes.Avatar;

public class AvatarSystem : MonoBehaviour
{
    public static AvatarSystem Instance { get; private set; }

    [Header("角色模型配置")]
    public GameObject characterModelPrefab; // 角色模型预制体(含所有部位)
    public GameObject characterTargetPrefab; // 角色目标预制体（只含骨骼和初始模型）

    [Header("默认装备配置"), SerializeField]
    private List<DefaultAvatarItem> defaultAvatarItems; // 默认装备列表（部位类型 -> 索引）

    [Header("事件")]
    public UnityEvent<PartType, int> OnPartChanged;
    public UnityEvent OnRandomChanged;


    #region 换装参数
    private GameObject characterTarget; // 角色目标模型
    private Transform characterSourceTrans; // 角色源模型变换组件
    private Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>> characterData; // 角色数据字典（部位类型 -> 编号 -> SkinnedMeshRenderer）
    private Dictionary<PartType, SkinnedMeshRenderer> characterSmr; // 角色当前SkinnedMeshRenderer字典（部位类型 -> SkinnedMeshRenderer）
    private Dictionary<string, Transform> boneMap; // 骨骼映射字典（骨骼名称 -> 骨骼变换组件）
    private Transform[] characterHips; // 角色骨骼数组
    #endregion


    #region Unity生命周期
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            characterData = new Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>>();
            characterSmr = new Dictionary<PartType, SkinnedMeshRenderer>();
            boneMap = new Dictionary<string, Transform>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeAvatar();
    }

    void OnDisable()
    {
        OnPartChanged = null;
    }
    #endregion


    #region 初始化模型
    /// <summary>
    /// 初始化角色模型，加载数据并设置默认装备
    /// </summary>
    private void InitializeAvatar()
    {
        if (characterModelPrefab == null || characterTargetPrefab == null)
        {
            Debug.LogError("角色模型或目标预制体未设置！");
            return;
        }

        InitSourceCharacter(); // 初始化源模型
        InitTargetCharacter(); // 初始化目标模型
        SaveData(characterSourceTrans, characterData, characterSmr, characterTarget);
        CreateBoneDictionary();
        InitDefaultAvatar();
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
                go.name = partName;
                go.transform.SetParent(target.transform);
                smr[partType] = go.AddComponent<SkinnedMeshRenderer>();
                data[partType] = new Dictionary<int, SkinnedMeshRenderer>();
            }

            data[partType][num] = part;
        }
    }

    #endregion


    #region 换装实现
    /// <summary>
    /// 初始化默认装备
    /// </summary>
    private void InitDefaultAvatar()
    {
        foreach (var item in defaultAvatarItems)
        {
            ChangeMesh(item.partType, item.index, characterData, characterSmr, characterHips);
        }
    }

    /// <summary>
    /// 切换角色部位的装备
    /// </summary>
    /// <param name="part">部位类型</param>
    /// <param name="num">编号</param>
    /// <param name="data">数据字典</param>
    /// <param name="smr">SkinnedMeshRenderer字典</param>
    /// <param name="hips">骨骼数组</param>
    private void ChangeMesh(
    PartType part,
    int num,
    Dictionary<PartType, Dictionary<int, SkinnedMeshRenderer>> data,
    Dictionary<PartType, SkinnedMeshRenderer> smr,
    Transform[] hips)
    {
        // 空值检查
        if (data == null || !data.ContainsKey(part) || !data[part].ContainsKey(num))
        {
            Debug.LogWarning($"ChangeMesh: 找不到对应的部位或编号 - 部位: {part}, 编号: {num}");
            return;
        }

        SkinnedMeshRenderer skm = data[part][num];
        if (skm == null)
        {
            Debug.LogWarning($"ChangeMesh: 找不到对应的SkinnedMeshRenderer - 部位: {part}, 编号: {num}");
            return;
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
            smr[part].bones = bones.ToArray();      // 更新骨骼
            smr[part].rootBone = bones[0];          // 更新根骨骼
            smr[part].materials = skm.materials;    // 更新材质
            smr[part].sharedMesh = skm.sharedMesh;  // 更新网格
            Debug.Log($"换装成功: {part} - {num}");
        }
        else
        {
            Debug.LogWarning($"ChangeMesh: 找不到对应的目标SkinnedMeshRenderer - 部位: {part}");
        }

    }

    /// <summary>
    /// 通过字符串切换部位
    /// </summary>
    public void ChangeMesh(string partName, int index)
    {
        if (Enum.TryParse(partName, out PartType partType))
        {
            ChangeMesh(partType, index, characterData, characterSmr, characterHips);
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
        RaisePartChangedEvent(partType, index);
    }
    #endregion


    #region 工具方法
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
    /// 根据部位类型获取对应的 AvatarSystem CharacterPart 数据对象
    /// </summary>
    /// <param name="partType">部位类型</param>
    public CharacterPart GetCharacterPartByType(PartType partType)
    {
        if (characterData.TryGetValue(partType, out var partDict) && partDict.Count > 0)
        {
            var sortedIndexes = partDict.Keys.OrderBy(x => x).ToList();
            return new CharacterPart
            {
                PartType = partType,
                CurrentPartsObjects = sortedIndexes.Select(index => partDict[index].gameObject).ToList(),
                PartIndexes = sortedIndexes,
                CurrentIndex = 0
            };
        }
        return null;
    }
    
    #endregion


    #region 事件触发
    public void RaisePartChangedEvent(PartType partType, int index)
    {
        OnPartChanged?.Invoke(partType, index);
    }

    public void RaiseRandomChangedEvent()
    {
        OnRandomChanged?.Invoke();
    }
    #endregion
}
using UnityEngine;
using UnityEngine.SceneManagement;
using ChangeClothes.Data;

public class ChangeOutController : MonoBehaviour
{
    public static ChangeOutController Instance { get; private set; }

    [Header("场景切换配置")]
    public Transform spawnPoint; // 角色生成位置（在每个场景中设置）

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 初始化逻辑已移至 OnSceneLoaded 回调中处理
        // 这样可以避免重复初始化（Start 和 sceneLoaded 都会被调用）
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("ChangeClothes");
        }
    }

    public void Init()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName == "ChangeClothes")
            {
                // ChangeClothes 场景：使用预制体创建角色
                AvatarSystem.Instance.Init();
                UIController.Instance.Init();
                
                if (EquipDataManager.Instance.HasSavedEquip())
                {
                    // 有保存的配置，加载并应用
                    Debug.Log("检测到保存的换装配置，正在加载...");
                    EquipDataManager.Instance.LoadAndApplyEquip();
                }
                else
                {
                    // 没有保存的配置，使用默认装备
                    AvatarSystem.Instance.InitDefaultAvatar();
                }
            }
        else
        {
            // 非 ChangeClothes 场景：查找场景中的 Character_Basic 对象
            GameObject existingCharacter = GameObject.Find("Character_Basic");
            if (existingCharacter != null)
            {
                AvatarSystem.Instance.InitFromExistingCharacter(existingCharacter);
                
                // 读取并应用保存的换装数据
                if (EquipDataManager.Instance.HasSavedEquip())
                {
                    Debug.Log("检测到保存的换装配置，正在加载...");
                    EquipDataManager.Instance.LoadAndApplyEquip();
                }
            }
            else
            {
                Debug.LogWarning("场景中未找到 Character_Basic 对象");
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载完成: {scene.name}");
        
        // 查找新场景中的生成点
        FindSpawnPointInScene();
        
        // 如果是 ChangeClothes 场景，重新初始化换装系统
        if (scene.name == "ChangeClothes")
        {
            InitChangeClothesScene();
        }
        else
        {
            // 非 ChangeClothes 场景：重新初始化角色模型
            ReInitForNewScene(scene);
        }
    }

    /// <summary>
    /// 初始化 ChangeClothes 场景
    /// </summary>
    private void InitChangeClothesScene()
    {
        // 显示鼠标光标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // 使用预制体创建角色
        AvatarSystem.Instance.Init();
        UIController.Instance.Init();
        
        if (EquipDataManager.Instance.HasSavedEquip())
        {
            Debug.Log("检测到保存的换装配置，正在加载...");
            EquipDataManager.Instance.LoadAndApplyEquip();
        }
        else
        {
            AvatarSystem.Instance.InitDefaultAvatar();
        }
    }

    /// <summary>
    /// 在新场景中重新初始化（针对非 ChangeClothes 场景）
    /// </summary>
    private void ReInitForNewScene(Scene scene)
    {
        // 查找场景中的 Character_Basic 对象
        GameObject existingCharacter = GameObject.Find("Character_Basic");
        if (existingCharacter != null)
        {
            // 重新初始化 AvatarSystem（使用场景中的角色）
            AvatarSystem.Instance.InitFromExistingCharacter(existingCharacter);
            
            // 读取并应用保存的换装数据
            if (EquipDataManager.Instance.HasSavedEquip())
            {
                Debug.Log("检测到保存的换装配置，正在加载...");
                EquipDataManager.Instance.LoadAndApplyEquip();
            }
        }
        else
        {
            Debug.LogWarning("场景中未找到 Character_Basic 对象");
        }
    }

    /// <summary>
    /// 在新场景中查找生成点并移动角色
    /// </summary>
    private void FindSpawnPointInScene()
    {
        GameObject spawnObj = GameObject.Find("spawnPoint");
        if (spawnObj != null)
        {
            spawnPoint = spawnObj.transform;
            Debug.Log($"找到生成点: {spawnPoint.position}");
            
            MoveCharacterToSpawnPoint();
        }
        else
        {
            Debug.LogWarning("场景中未找到 spawnPoint");
        }
    }

    /// <summary>
    /// 将角色移动到生成点
    /// </summary>
    private void MoveCharacterToSpawnPoint()
    {
        if (spawnPoint != null && AvatarSystem.Instance != null)
        {
            // 获取角色目标对象并移动
            Transform characterTarget = AvatarSystem.Instance.GetCharacterTargetTransform();
            if (characterTarget != null)
            {
                characterTarget.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                characterTarget.localScale = spawnPoint.localScale;
                Debug.Log("角色已移动到新场景的生成点");
            }
        }
    }
}
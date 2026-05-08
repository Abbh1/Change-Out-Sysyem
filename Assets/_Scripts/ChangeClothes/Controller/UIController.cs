using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public PartType FocusPartType { get; private set; }

    [Header("UI组件")]
    [SerializeField] private GameObject itemFocusSlot;      // 物品焦点槽位
    [SerializeField] private ItemSolt[] itemSlots;

    [Header("图标资源")]
    public Sprite[] spriteActiveIcons;  // 激活状态图标（显示/隐藏）
    public Sprite[] spriteBgs;          // 背景图（空/有物品）
    private ItemSolt currentHoverSlot;


    private void Awake()    
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        // 初始化物品槽位
        itemSlots = transform.GetComponentsInChildren<ItemSolt>();
        foreach (var slot in itemSlots)
        {
            slot.InitSolt();
        }
    }

    /// <summary>
    /// 设置当前聚焦的部位槽位
    /// </summary>
    /// <param name="itemSlot"></param>
    public void SetFocusPart(ItemSolt itemSlot)
    {
        if (itemSlot == currentHoverSlot)
        {
            return;
        }

        if (currentHoverSlot != null && itemSlot != null)
        {
            currentHoverSlot.HideSelect();
        }

        currentHoverSlot = itemSlot;

        if (itemSlot == null)
        {
            return;
        }

        FocusPartType = itemSlot.PartType;
        itemFocusSlot.transform.SetParent(itemSlot.transform, false);
        itemFocusSlot.transform.position = itemSlot.transform.position;
        itemFocusSlot.SetActive(true);
    }

    /// <summary>
    /// 从Resources文件夹中加载Sprite
    /// </summary>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public Sprite GetSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            Debug.LogWarning("GetSprite: spriteName 为空");
            return null;
        }

        var sprite = Resources.Load<Sprite>(spriteName);
        if (sprite != null)
            return sprite;

        Debug.LogWarning($"Resources.Load<Sprite> 未找到：{spriteName}。尝试从 Assets/Resources 加载文件。");

        string filePath = Path.Combine(Application.dataPath, "Resources", spriteName.Replace('/', Path.DirectorySeparatorChar) + ".png");
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"GetSprite: 资源文件不存在：{filePath}");
            return null;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(fileData))
        {
            Debug.LogWarning($"GetSprite: 读取图片失败：{filePath}");
            return null;
        }

        return null;
        // return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

}

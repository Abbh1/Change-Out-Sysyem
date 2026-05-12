using UnityEngine;

public class ChangeOutController : MonoBehaviour
{
    public static ChangeOutController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        AvatarSystem.Instance.Init();
        UIController.Instance.Init();  
        AvatarSystem.Instance.InitDefaultAvatar(); 
    }
}

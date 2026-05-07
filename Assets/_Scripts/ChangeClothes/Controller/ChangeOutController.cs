using UnityEngine;

public class ChangeOutController : MonoBehaviour
{
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

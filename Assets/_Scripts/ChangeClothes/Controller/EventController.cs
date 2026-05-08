using System;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public static EventController Instance { get; private set; }

    public event Action<PartType, int> OnPartChanged;
    public event Action<PartType, bool> OnClickHided;
    public event Action OnRandomRequested;
    public event Action OnNextPartRequested;
    public event Action OnPreviousPartRequested;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RaisePartChanged(PartType partType, int index) => OnPartChanged?.Invoke(partType, index);
    public void RaiseClickHide(PartType partType, bool isHide) => OnClickHided?.Invoke(partType, isHide);

    public void RaiseRandomRequested() => OnRandomRequested?.Invoke();

    public void RaiseNextPart() => OnNextPartRequested?.Invoke();

    public void RaisePreviousPart() => OnPreviousPartRequested?.Invoke();
}
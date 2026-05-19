using UnityEngine;

public class DragView : MonoBehaviour
{
    private Transform target;

    [SerializeField]
    private float rotateSpeed;
    private bool isDraging;
    private Vector3 starPos;

    private void Awake()    
    {
        target = transform.GetComponent<Transform>();
    }

    private void OnMouseDown()
    {
        isDraging = true;
        starPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!isDraging) return;
        var currentPos = Input.mousePosition;
        var delta = currentPos - starPos;
        starPos = currentPos;
        var rotationX = delta.x * rotateSpeed;
        target.transform.Rotate(Vector3.up, -rotationX, Space.World);
    }

    private void OnMouseUp()
    {
        isDraging = false;
    }
}
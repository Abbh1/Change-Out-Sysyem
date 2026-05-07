using UnityEngine;
using UnityEngine.EventSystems;

namespace Layer_lab._3D_Casual_Character.Demo2
{
    /// <summary>
    /// 拖拽旋转视图 - 支持鼠标拖拽旋转角色
    /// </summary>
    public class DragView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private float rotationSpeed = 0.18f;  // 旋转速度
        private bool _isDragging;                               // 是否正在拖拽
        private Vector2 _startDragPos;                          // 拖拽起始位置
        [SerializeField] private Transform target;               // 旋转目标

        /// <summary>
        /// 鼠标按下
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            _startDragPos = eventData.position;
        }

        /// <summary>
        /// 鼠标拖拽中
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            var currentDragPos = eventData.position;
            var dragDelta = currentDragPos - _startDragPos;

            var rotationX = dragDelta.x * rotationSpeed;
            target.transform.Rotate(Vector3.up, -rotationX, Space.World);

            _startDragPos = currentDragPos;
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
        }
    }
}
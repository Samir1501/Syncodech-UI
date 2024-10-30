using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UISystem
{
    [ExecuteAlways]
    public class Scroll : Selectable, IDragHandler,IBeginDragHandler
    {
        private RectTransform _containerRect;
        [SerializeField] private Direction direction;
        [SerializeField] [Range(0, 1)] public float value;
        [SerializeField] [Range(0, 1)] private float size;
        [SerializeField] [Range(1, 11)] private int numberOfSteps;
        [SerializeField] private Handle handle;
        private Vector2 _sizeDelta;
        private Vector2 _offset;
        public UnityEvent<float> onChange;

        private void OnEnable()
        {
            _containerRect = handle.rect.GetComponentsInParent<RectTransform>()[1];
        }
        
        protected override void OnValidate()
        {
            Set(value);
        }

        public void Set(float value, float size, bool callback = true)
        {
            this.size = Mathf.Clamp01(size);
            Set(value,callback);
        }
        
        public void Set(float value, bool callback = true)
        {
            this.value = value;
          
            this.value = numberOfSteps > 1 ? Mathf.Round(Mathf.Lerp(0, numberOfSteps-1, value)) / (numberOfSteps-1) : value;

            handle.Set(this.value, size, direction);
            if(callback) onChange.Invoke(this.value);
        }
        
        private void OnMove(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_containerRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) return;
            int axis = (int) UIVisual.GetAxis(direction);
            
            float remainingSize = _sizeDelta[axis] * (1 - size);
            Vector2 handlePoint = localPoint;
            
            handlePoint *= 2;
            
            handlePoint[axis] = Mathf.Clamp(handlePoint[axis] /= remainingSize, -1, 1);

            float normalizedValue = (1 + handlePoint[axis]) / 2;
            
            if (UIVisual.CheckReverse(direction)) normalizedValue = 1 - normalizedValue;

            value = normalizedValue;
            
            Set(value);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if(!IsInteractable()) return;
            OnMove(eventData);
        }
        

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!IsInteractable()) return;
            _sizeDelta = _containerRect.rect.size;
            OnMove(eventData);
        }
    }
}
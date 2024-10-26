using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UISystem
{
    [ExecuteAlways]
    public class Slider : Selectable, IDragHandler
    {
        private RectTransform _baseRect;
        private Vector2 _localPoint;

        [SerializeField] private bool wholeNumbers;
        
        [SerializeField] private float value;
        
        [SerializeField] private float min;
        [SerializeField] private float max;
        
        [SerializeField] private Direction direction;
        [SerializeField] private Fill fill;
        [SerializeField] private Handle handle;

        public UnityEvent<float, Vector2> onChange;

        private void OnEnable()
        {
            _baseRect = fill.rect.GetComponentsInParent<RectTransform>()[1];
        }

        public void Set(float value, bool callback = true)
        {
            value = Mathf.Clamp(value, min, max);
            
            float tmpValue = value;
            
            value = (value - min) / (max - min);
            fill?.Set(value, direction);
            handle?.Set(value, direction);

            if(!callback) return;
            onChange.Invoke(tmpValue,_localPoint);
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            Set(value);
        }

        private void OnMove(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, eventData.position, eventData.pressEventCamera, out _localPoint)) return;
            Vector2 sizeDelta = _baseRect.rect.size;
            _localPoint *= 2;
                
            _localPoint.x /= sizeDelta.x;
            _localPoint.y /= sizeDelta.y;

            _localPoint.x = Mathf.Clamp(_localPoint.x,-1,1);
            _localPoint.y = Mathf.Clamp(_localPoint.y,-1,1);
            
            float normalizedValue = UIVisual.GetAxis(direction) switch
            {
                Axis.Horizontal => (1 + _localPoint.x) / 2,
                Axis.Vertical => (1 + _localPoint.y) / 2,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (UIVisual.CheckReverse(direction)) normalizedValue = 1 - normalizedValue;

            value = wholeNumbers
                ? Mathf.Round(Mathf.Lerp(min, max, normalizedValue))
                : Mathf.Lerp(min, max, normalizedValue);
            
            Set(value);
            //testRect.anchoredPosition = delta/2*sizeDelta;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if(!IsInteractable()) return;
            OnMove(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if(!IsInteractable()) return;
            base.OnPointerDown(eventData);
            OnMove(eventData);
        }
    }
}

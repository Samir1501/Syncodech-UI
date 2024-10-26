using System;
using UnityEngine;

namespace UISystem
{
    [Serializable]
    public class Handle : UIVisual
    {
        private float _size;

        public void Set(float value, float size, Direction direction)
        {
            Value = value;
            _size = size;
            Direction = direction;
            UpdateVisual();
        }
        
        public void Set(float value, Direction direction)
        {
            Value = value;
            Direction = direction;
            UpdateVisual();
        }
        
        public override void UpdateVisual()
        {
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;

            Value = Reverse ? 1 - Value : Value;
            
            float min = _size * Value;
            float max = _size * (1-Value);
            
            anchorMin[(int) Axis] = Value - min;
            anchorMax[(int) Axis] = Value + max;

            if (rect == null) return;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
    }
}
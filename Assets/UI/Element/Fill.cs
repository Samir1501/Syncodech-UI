using System;
using UnityEngine;

namespace Syncodech.UI
{
    [Serializable]
    public class Fill : UIVisual
    {
        public void Set(float value, Direction direction)
        {
            Value = value;
            Direction = direction;
            UpdateVisual();
        }

        public override void UpdateVisual()
        {
            Value = Reverse ? 1 - Value : Value;

            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;
            
            if (Reverse) anchorMin[(int) Axis] = Value;
            else anchorMax[(int) Axis] = Value;

            if (rect == null) return;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
    }
}
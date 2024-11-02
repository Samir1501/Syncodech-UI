using System;
using UnityEngine;
namespace Syncodech.UI
{
    public enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum Direction
    {
        LeftToRight = 0,
        RightToLeft = 1,
        BottomToTop = 2,
        TopToBottom = 3
    }

    [Serializable]
    public abstract class UIVisual
    {
        protected float Value;
        [SerializeField] internal RectTransform rect;
        protected bool Reverse => CheckReverse(Direction);
        protected Axis Axis => GetAxis(Direction);

        protected Direction Direction;
    
        public static Axis GetAxis(Direction direction)
        {
            return direction is Direction.RightToLeft or Direction.LeftToRight ? Axis.Horizontal : Axis.Vertical;
        }
        public static bool CheckReverse(Direction direction)
        {
            return direction is Direction.RightToLeft or Direction.TopToBottom;
        }

        public abstract void UpdateVisual();
    }
}
using UnityEngine;

namespace Syncodech.UI
{
    public enum Side
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3
    }

    [ExecuteAlways]
    public class Panel : WindowElement
    {
        private Side _side;

        [SerializeField] private Side openSide;
        [SerializeField] private Side closeSide;
        [SerializeField] private bool scaleAnimation;
        [SerializeField] private bool dontMove;

        private float PositionValue => _side is Side.Right or Side.Top ? 1 - value : -(1 - value);
        private Axis Axis => _side is Side.Left or Side.Right ? Axis.Horizontal : Axis.Vertical;

        private Vector2 _anchorMin = Vector2.zero;
        private Vector2 _anchorMax = Vector2.zero;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            OnValueChanged += () =>
            {
                if (dontMove)
                {
                    CanvasActive();
                    if (scaleAnimation) Scale();
                }
                else Move();
            };
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            Value = value;
        }

        public void Visible(bool visible)
        {
            _side = visible ? openSide : closeSide;
            CheckVisibility(visible);
        }

        public void Top(bool visible)
        {
            _side = Side.Top;
            CheckVisibility(visible);
        }

        public void Bottom(bool visible)
        {
            _side = Side.Bottom;
            CheckVisibility(visible);
        }

        public void Left(bool visible)
        {
            _side = Side.Left;
            CheckVisibility(visible);
        }

        public void Right(bool visible)
        {
            _side = Side.Right;
            CheckVisibility(visible);
        }

        private void Move()
        {
            if (!Rect) return;
            CanvasActive();
            Scale();

            _anchorMin = Vector2.zero;
            _anchorMax = Vector2.one;

            _anchorMin[(int)Axis] = PositionValue;
            _anchorMax[(int)Axis] = 1 + PositionValue;

            Rect.anchorMin = _anchorMin;
            Rect.anchorMax = _anchorMax;
        }
    }
}

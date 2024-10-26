using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    [Serializable]
    public struct TransitionBlock
    {
        public Color color;
        public Sprite sprite;
        public Rect rect;
        public Vector3 eulerAngles;
        public Vector3 scale;
    }

    public enum TransitionType : byte
    {
        Color = 0,
        Sprite = 1,
        Transform = 2
    }

    public enum TransitionMode : byte
    {
        Normal = 0,
        Hover = 1,
        Pressed = 2,
        Selected = 3,
        Disabled = 4
    }

    public enum ActiveMode
    {
        [InspectorName(null)] None = -1,
        Normal = 0,
        Active = 1
    }

    [Serializable]
    public class Transition
    {
        private Coroutine fade;

        private bool _isActive;

        public Graphic graphic;

        public Image Image => graphic as Image;
        public ActiveMode activeMode;
        public TransitionType transition;
        private TransitionMode _transitionMode;

        public TransitionBlock normal;
        public TransitionBlock hover;
        public TransitionBlock active;
        public TransitionBlock activeHover;
        public TransitionBlock pressed;
        public TransitionBlock activePressed;
        public TransitionBlock selected;
        public TransitionBlock activeSelected;
        public TransitionBlock disabled;
        public TransitionBlock activeDisabled;

        private TransitionBlock Normal => _isActive ? active : normal;
        private TransitionBlock Hover => _isActive ? activeHover : hover;
        private TransitionBlock Pressed => _isActive ? activePressed : pressed;
        private TransitionBlock Selected => _isActive ? activeSelected : selected;
        private TransitionBlock Disabled => _isActive ? activeDisabled : disabled;

        private TransitionBlock Used
        {
            get
            {
                return _transitionMode switch
                {
                    TransitionMode.Normal => Normal,
                    TransitionMode.Hover => Hover,
                    TransitionMode.Pressed => Pressed,
                    TransitionMode.Selected => Selected,
                    TransitionMode.Disabled => Disabled,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public float duration;

        private void SetActive(ActiveMode mode)
        {
            _isActive = mode switch
            {
                ActiveMode.Normal => false,
                ActiveMode.Active => true,
                ActiveMode.None => false,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public void Set(bool interactable, ActiveMode mode = ActiveMode.None)
        {
            if (graphic == null) return;
            if (mode != ActiveMode.None) SetActive(mode);
            switch (transition)
            {
                case TransitionType.Color:
                    SetColor(interactable ? TransitionMode.Normal : TransitionMode.Disabled);
                    break;
                case TransitionType.Sprite:
                    SetSprite(interactable ? TransitionMode.Normal : TransitionMode.Disabled);
                    break;
                case TransitionType.Transform:
                    SetTransform(interactable ? TransitionMode.Normal : TransitionMode.Disabled);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void Change(TransitionMode mode, ActiveMode activeMode = ActiveMode.None)
        {
            if (activeMode != ActiveMode.None) SetActive(activeMode);

            switch (transition)
            {
                case TransitionType.Color:
                    SetColor(mode, true);
                    break;
                case TransitionType.Sprite:
                    SetSprite(mode);
                    break;
                case TransitionType.Transform:
                    SetTransform(mode, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetColor(TransitionMode mode, bool useDuration = false)
        {
            _transitionMode = mode;
            graphic.CrossFadeColor(Used.color, useDuration ? duration : 0, true, true, true);
        }

        private void SetSprite(TransitionMode mode)
        {
            if (Image == null) return;
            _transitionMode = mode;
            Image.overrideSprite = Used.sprite;
        }

        private void SetTransform(TransitionMode mode, bool useDuration = false)
        {
            _transitionMode = mode;
            if (fade != null) graphic.StopCoroutine(fade);
            fade = graphic.StartCoroutine(SetTransform(Used, useDuration));
        }

        public IEnumerator Fade(TransitionMode mode)
        {
            switch (transition)
            {
                case TransitionType.Color:
                    SetColor(mode,true);
                    break;
                case TransitionType.Sprite:
                    SetSprite(mode);
                    break;
                case TransitionType.Transform:
                    SetTransform(mode,true);
                    break;
            }
            yield return new WaitForSeconds(duration);
        }
        
        IEnumerator SetTransform(TransitionBlock used, bool useDuration)
        {
            Vector2 position = graphic.rectTransform.anchoredPosition;
            Vector2 size = graphic.rectTransform.sizeDelta;
            Vector3 eulerAngles = graphic.rectTransform.localEulerAngles;
            Vector3 scale = graphic.rectTransform.localScale;
            if (useDuration)
            {
                float time = 0;
                while (time < 1)
                {
                    time += (duration == 0 ? 1 : 1 / duration) * 0.005f;
                    graphic.rectTransform.localEulerAngles = Vector3.LerpUnclamped(eulerAngles, used.eulerAngles, time);
                    graphic.rectTransform.localScale = Vector3.LerpUnclamped(scale, used.scale, time);
                    graphic.rectTransform.anchoredPosition = Vector2.LerpUnclamped(position, used.rect.position, time);
                    graphic.rectTransform.sizeDelta = Vector2.LerpUnclamped(size, used.rect.size, time);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                graphic.rectTransform.localEulerAngles = used.eulerAngles;
                graphic.rectTransform.localScale = used.scale;
                graphic.rectTransform.anchoredPosition = used.rect.position;
                graphic.rectTransform.sizeDelta = used.rect.size;
            }
        }
    }

    public class UIBehaviour : MonoBehaviour { }
}
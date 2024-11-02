using System;
using System.Collections;
using UnityEngine;

namespace Syncodech.UI
{
    public class WindowElement : UIBehaviour
    {
        private bool _isVisible;
        public FieldElement field;
        [SerializeField] protected float duration = 1f;
        [SerializeField] protected AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] [Range(0, 1)] protected float value;
        protected RectTransform Rect;
        [SerializeField] protected CanvasGroup canvasGroup;
        protected Coroutine VisibleCoroutine;

        protected virtual void OnEnable()
        {
            Rect = GetComponent<RectTransform>();
            if(canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void OnDisable()
        {
            OnValueChanged = null;
        }

        protected float Value
        {
            set
            {
                this.value = value;
                OnValueChanged?.Invoke();
            }   
        }
        
        protected Action OnValueChanged;
        

        protected void CheckVisibility(bool visible)
        {
            if (_isVisible == visible) return;
            _isVisible = visible;
            if (Application.isPlaying)
            {
                if (VisibleCoroutine != null) StopCoroutine(VisibleCoroutine);
                StartCoroutine(VisibleTween());
            }
            else
            {
                Value = visible ? 1 : 0;
            }
        }

        private IEnumerator VisibleTween()
        {
            float t = 0;
            while (t < 1)
            {
                t += duration * 0.008f;
                Value = _isVisible ? Mathf.Lerp(0, 1, moveCurve.Evaluate(t)) : Mathf.Lerp(1, 0, moveCurve.Evaluate(t));
                yield return new WaitForEndOfFrame();
            }
        }
        
        protected void Scale()
        {
            Rect.localScale = Vector3.one * value;
        }

        protected void CanvasActive()
        {
            if (!canvasGroup) return;
            canvasGroup.alpha = value;
            canvasGroup.interactable = value > 0.5f;
            canvasGroup.blocksRaycasts = value > 0.5f;
        }
    }
}
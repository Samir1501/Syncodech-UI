using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Syncodech.UI
{
    [ExecuteInEditMode]
    public class Selectable : UIBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerUpHandler, IPointerDownHandler,
        ISelectHandler, IDeselectHandler
    {
        private CanvasGroup[] _groups;

        private TransitionMode Mode
        {
            set => Change(value);
        }

        public List<Transition> transitions;

        private bool _isHover;
        private bool _hasSelected;

        [SerializeField] private bool interactable = true;

        private bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                _hasSelected = false;
                Change(IsInteractable() ? TransitionMode.Normal : TransitionMode.Disabled);
            }
        }

        private bool _groupInteractable;

        private bool GroupInteractable
        {
            get => _groupInteractable;
            set
            {
                _groupInteractable = value;
                _hasSelected = false;
                Change(IsInteractable() ? TransitionMode.Normal : TransitionMode.Disabled);
            }
        }

        private void OnEnable()
        {
            _groups = GetComponentsInParent<CanvasGroup>();
            CheckCanvasGroup();
            if (transitions == null) return;
            foreach (Transition transition in transitions) transition.Set(IsInteractable());
        }

        protected virtual void OnValidate()
        {
            if (interactable != Interactable) Interactable = interactable;
            if (transitions == null) return;
            foreach (Transition transition in transitions) transition.Set(IsInteractable());
        }

        private void OnCanvasGroupChanged()
        {
            CheckCanvasGroup();
        }

        private void CheckCanvasGroup()
        {
            GroupInteractable = _groups == null || _groups.All(canvasGroup => canvasGroup.interactable);
        }

        protected bool IsInteractable()
        {
            return Interactable && GroupInteractable;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable()) return;
            _isHover = true;
            Mode = TransitionMode.Hover;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInteractable()) return;
            _isHover = false;
            Mode = TransitionMode.Normal;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable()) return;
            Mode = TransitionMode.Normal;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable()) return;
            Mode = TransitionMode.Pressed;
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            if(!IsInteractable()) return;
            _hasSelected = true;
            Mode = TransitionMode.Selected;
        }
        
        public virtual void OnDeselect(BaseEventData eventData)
        {
            if(!IsInteractable()) return;
            _hasSelected = false;
            Mode = TransitionMode.Normal;
        }

        protected void Change(TransitionMode mode, ActiveMode activeMode = ActiveMode.None)
        {
            mode = mode switch
            {
                TransitionMode.Normal => _isHover 
                    ? TransitionMode.Hover 
                    : _hasSelected ? TransitionMode.Selected : TransitionMode.Normal,
                _ => mode
            };
            foreach (Transition transition in transitions) transition.Change(mode, activeMode);
        }
    }
}

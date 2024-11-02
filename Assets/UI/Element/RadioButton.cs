using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Syncodech.UI
{
    [ExecuteAlways]
    public class RadioButton : Selectable, IPointerClickHandler
    {
        private RadioButtonGroup _group;
        public FieldElement field;
        [SerializeField] internal int index;
        [SerializeField] private bool isOn;

        public bool IsOn
        {
            get => isOn;
            set
            {
                if (IsOn != value) onChange.Invoke(value);
                isOn = value;
                Change(TransitionMode.Normal, isOn ? ActiveMode.Active : ActiveMode.Normal);
            }
        }

        public UnityEvent<bool> onChange;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            if (IsOn != isOn) IsOn = isOn;
            if (transitions == null) return;
            foreach (Transition transition in transitions)
            {
                transition.Set(IsInteractable(), isOn ? ActiveMode.Active : ActiveMode.Normal);
            }
        }

        public void SetGroup(RadioButtonGroup group)
        {
            _group = group;
        }
        
        private void OnBeforeTransformParentChanged()
        {
            Remove();
        }

        private void OnDestroy()
        {
            Remove();
        }

        public RadioButton Add()
        {
            SetGroup(GetComponentInParent<RadioButtonGroup>(true));
            if(_group == null) return null;
            _group.Add(this);
            return this;
        }

        public void Remove()
        {
            if(_group == null) return;
            _group.Remove(this);
        }
        
        private void OnTransformParentChanged()
        {
            Add();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable()) return;
            IsOn = !IsOn;
            if(_group != null && isOn) _group.Change(index);
        }
    }
}
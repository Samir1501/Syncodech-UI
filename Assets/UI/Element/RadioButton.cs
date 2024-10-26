using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UISystem
{
    [ExecuteAlways]
    public class RadioButton : Selectable, IPointerClickHandler
    {
        private RadioButtonGroup _group;
        public Field field;
        [FormerlySerializedAs("fieldComponent")] public FieldGroup fieldGroupComponent;
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
            if(_group == null) return;
            _group.Remove(this);
        }

        private void OnDestroy()
        {
            if(_group == null) return;
            _group.Remove(this);
        }

        private void OnTransformParentChanged()
        {
            SetGroup(GetComponentInParent<RadioButtonGroup>(true));
            if (_group != null)
            {
                _group.Add(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable()) return;
            IsOn = !IsOn;
            if(_group != null && isOn) _group.Change(this);
        }
    }
}
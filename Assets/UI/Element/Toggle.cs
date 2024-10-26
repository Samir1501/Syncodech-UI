using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UISystem
{
    public class Toggle : Selectable,IPointerClickHandler
    {
        public FieldElement field;
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
            if(transitions == null) return;
            foreach (Transition transition in transitions)
            {
                transition.Set(IsInteractable(),isOn ? ActiveMode.Active : ActiveMode.Normal);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsInteractable()) return;
            IsOn = !IsOn;
        }
    }
}
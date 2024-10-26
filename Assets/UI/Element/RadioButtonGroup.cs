using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UISystem
{
    [ExecuteAlways]
    public class RadioButtonGroup : UIBehaviour
    {
        [SerializeField] private int currentIndex;
        [SerializeField] private int nextIndex;
        [SerializeField] private List<RadioButton> radioButtons;

        public UnityEvent<int> onChange;

        private void OnEnable()
        {
            foreach (RadioButton radioButton in radioButtons)
            {
                radioButton.SetGroup(this);
            }
        }

        public void Add(RadioButton radioButton)
        {
            radioButton.index = nextIndex;
            radioButtons.Add(radioButton);
            nextIndex = radioButtons.Count;
        }

        public void Remove(RadioButton radioButton)
        {
            if (radioButtons.Remove(radioButton))
            {
                nextIndex = radioButton.index;
            }
        }

        public void Change(RadioButton radioButton)
        {
            currentIndex = radioButton.index;
            foreach (var button in radioButtons)
            {
                button.IsOn = button.index == currentIndex;
            }
            onChange.Invoke(currentIndex);
        }
    }
}
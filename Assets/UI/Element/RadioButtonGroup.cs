using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Syncodech.UI
{
    [ExecuteAlways]
    public class RadioButtonGroup : UIBehaviour
    {
        #if ENABLE_INPUT_SYSTEM
        private InputAction _previousAction;
        private InputAction _nextAction;
        #endif
        
        [SerializeField] private int currentIndex;
        [SerializeField] private int nextIndex;
        [SerializeField] private RadioButton prefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private List<RadioButton> radioButtons;

        public UnityEvent<int> onChange;

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            _previousAction = InputSystem.actions.FindAction("Previous");
            _nextAction = InputSystem.actions.FindAction("Next");
            _previousAction.started += PreviousActionOnStarted;
            _nextAction.started += NextActionOnStarted;
#endif
            if(radioButtons == null) return;
            foreach (RadioButton radioButton in radioButtons)
            {
                radioButton.SetGroup(this);
            }
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            _previousAction.started -= PreviousActionOnStarted;
            _nextAction.started -= NextActionOnStarted;
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private void NextActionOnStarted(InputAction.CallbackContext obj) => Change(currentIndex + 1);
        
        private void PreviousActionOnStarted(InputAction.CallbackContext obj) => Change(currentIndex - 1);
        
        #endif

        public void Set(FieldContext[] contextList)
        {
            foreach (FieldContext context in contextList)       
            {
                Instantiate(prefab, container).Add().field.Set(context);
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
        
        public void Change(int index)
        {
            currentIndex = Mathf.Clamp(index,0,radioButtons.Count-1);
            foreach (var button in radioButtons)
            {
                button.IsOn = button.index == currentIndex;
            }
            onChange.Invoke(currentIndex);
        }
    }
}
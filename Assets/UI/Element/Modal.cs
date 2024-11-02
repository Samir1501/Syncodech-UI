using UnityEngine;
using UnityEngine.Events;

namespace Syncodech.UI
{
    [ExecuteAlways]
    public class Modal : WindowElement
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button rejectButton;
        
        public UnityEvent onConfirmed;
        public UnityEvent onRejected;
        protected override void OnEnable()
        {
            base.OnEnable();
            OnValueChanged += () =>
            {
                Scale();
                CanvasActive();
            };
            confirmButton?.onClick.AddListener(Confirm);
            rejectButton?.onClick.AddListener(Reject);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            confirmButton?.onClick.RemoveListener(Confirm);
            rejectButton?.onClick.RemoveListener(Reject);
        }

        public void ShowButtons(bool value)
        {
            confirmButton?.gameObject.SetActive(value);
            rejectButton?.gameObject.SetActive(value);
        }
        
        private void Confirm()
        {
            onConfirmed.Invoke();
            Close();
        }

        private void Reject()
        {
            onRejected.Invoke();
            Close();
        }
        
        public void Open()
        {
            CheckVisibility(true);
        }

        public void Close()
        {
            CheckVisibility(false);
        }
    }
}
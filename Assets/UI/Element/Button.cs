using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Syncodech.UI
{
    public class Button : Selectable, IPointerClickHandler
    {
        public FieldElement field;
        
        public UnityEvent onClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsInteractable()) return;
            onClick.Invoke();
        }
    }
}

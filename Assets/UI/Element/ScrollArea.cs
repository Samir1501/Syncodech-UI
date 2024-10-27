using System;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class Scrollbar
{
    public bool enable = true;
    public Scroll scroll;
    public bool autoHide;
}

[ExecuteAlways]
public class ScrollArea : MonoBehaviour,IBeginDragHandler,IDragHandler
{
    public RectTransform content;
    public RectTransform viewport;
    public Scrollbar horizontal;
    public Scrollbar vertical;

    private Vector2 size;
    private Vector2 _contentPosition;
    private Vector2 _offset;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out _contentPosition)) return;
        _offset = content.anchoredPosition - _contentPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out _contentPosition)) return;
        content.anchoredPosition = _offset + _contentPosition;
        size.x = viewport.rect.size.x / content.rect.size.x;
        size.y = viewport.rect.size.y / content.rect.size.y;
        horizontal.scroll.Set(0, size.x);
        vertical.scroll.Set(0, size.y);
    }
}

using System;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class Scrollbar
{
    public bool enable = true;
    public Scroll scroll;
    public bool autoHide;
    public float spacing;

    public RectTransform rectTransform;
    
    private bool _show;

    public void UpdateCatch()
    {
        rectTransform = scroll == null ? null : scroll.transform as RectTransform; 
    }
    
    public float SetVisibility(bool calculateResult)
    {
        _show = enable;
        if (enable && autoHide) _show = calculateResult;
        scroll.gameObject.SetActive(_show);
        
        return _show ? spacing : 0;
    }
}

[SelectionBase]
[ExecuteAlways]
public class ScrollArea : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
{
    private RectTransform _baseRect;
    public RectTransform content;
    public RectTransform viewport;
    
    public Scrollbar horizontal;
    public Scrollbar vertical;

    private Vector2 MaxSize
    {
        get
        {
            if(content == null || viewport == null) return Vector2.zero;
            return Vector2.Max(Vector2.zero, content.rect.size - viewport.rect.size);
        }
    }
    private Vector2 _prevMaxSize;

    private Vector2 _velocity;
    private Vector2 _scrollSize;
    private Vector2 _contentPosition;
    private Vector2 _offset;

    private Vector2 ScrollSize
    {
        get
        {
            if (content == null || viewport == null) return Vector2.zero;
            return Vector2.Max(Vector2.zero, content.rect.size - viewport.rect.size);
        }
    }

    private Vector2 _prevBaseSize;
    private Vector2 _prevViewportSize;
    private Vector2 _prevContentSize;
    
    private bool _dragging;

    private void OnEnable()
    {
        UpdateCatch();
        horizontal.scroll.onChange.AddListener(x => ScrollbarControl());
        vertical.scroll.onChange.AddListener(y => ScrollbarControl());
        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
    }

    private void UpdateCatch()
    {
        _baseRect = transform as RectTransform;
        horizontal.UpdateCatch();
        vertical.UpdateCatch();
    }
    
    private void OnDisable()
    {
        horizontal.scroll.onChange.RemoveListener(x => ScrollbarControl());
        vertical.scroll.onChange.RemoveListener(y => ScrollbarControl());
        CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
    }

    private void OnValidate()
    {
        UpdateCatch();
        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
    }

    private void ScrollVisibility()
    {
        Vector2 viewportAnchorMin = Vector2.zero;
        Vector2 viewportAnchorMax = Vector2.one;
        
        viewportAnchorMax.x = 1-vertical.SetVisibility(ScrollSize.y > 0)/_baseRect.rect.size.x;
        viewportAnchorMin.y = horizontal.SetVisibility(ScrollSize.x > 0)/_baseRect.rect.size.y;
        
        Vector2 horizontalScrollbarAnchorMin = Vector2.zero;
        Vector2 horizontalScrollbarAnchorMax = Vector2.one;

        horizontalScrollbarAnchorMax.y = viewportAnchorMin.y;
        horizontalScrollbarAnchorMax.x = viewportAnchorMax.x;
        
        horizontal.rectTransform.anchorMin = horizontalScrollbarAnchorMin;
        horizontal.rectTransform.anchorMax = horizontalScrollbarAnchorMax;
        
        Vector2 verticalScrollbarAnchorMin = Vector2.zero;
        Vector2 verticalScrollbarAnchorMax = Vector2.one;
        
        verticalScrollbarAnchorMin.y = viewportAnchorMin.y;
        verticalScrollbarAnchorMin.x = viewportAnchorMax.x;
        
        vertical.rectTransform.anchorMin = verticalScrollbarAnchorMin;
        vertical.rectTransform.anchorMax = verticalScrollbarAnchorMax;
        
        viewport.anchorMin = viewportAnchorMin;
        viewport.anchorMax = viewportAnchorMax;
    }

    private void LateUpdate()
    {
        if (!CanvasUpdateRegistry.IsRebuildingLayout())
            Canvas.ForceUpdateCanvases();
        if (!_dragging)
        {
            if (_velocity.magnitude >= 0.01f)
            {
                _velocity = Vector2.Lerp(_velocity, Vector2.zero, 5 * Time.unscaledDeltaTime);
                _contentPosition += _velocity;
                UpdateScrollbar();
            }
            else if (content.anchoredPosition != _contentPosition || _prevMaxSize != MaxSize)
            {
                UpdateScrollbar();
                _prevMaxSize = MaxSize;
            }
        }

        if (_prevBaseSize != _baseRect.rect.size || _prevViewportSize != viewport.rect.size || _prevContentSize != content.rect.size)
        {
            _prevBaseSize = _baseRect.rect.size;
            _prevViewportSize = viewport.rect.size;
            _prevContentSize = content.rect.size;
            ScrollVisibility();
        }
    }

    private void ScrollbarControl()
    {
        _contentPosition = new Vector2(-horizontal.scroll.value, vertical.scroll.value) * MaxSize;
        UpdateVisual();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out _contentPosition)) return;
        _offset = content.anchoredPosition - _contentPosition;
        _dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out Vector2 mousePosition)) return;
        _contentPosition = _offset + mousePosition;
        UpdateScrollbar();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _velocity = eventData.delta;
        _dragging = false;
    }

    private void UpdateVisual()
    {
        content.anchoredPosition = _contentPosition;
        
        _scrollSize.x = viewport.rect.size.x / content.rect.size.x;
        _scrollSize.y = viewport.rect.size.y / content.rect.size.y;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateScrollbar()
    {
        _contentPosition.x = Mathf.Clamp(_contentPosition.x,-MaxSize.x,0);
        _contentPosition.y = Mathf.Clamp(_contentPosition.y,0,MaxSize.y);

        UpdateVisual();
        Vector2 scrollValue = _contentPosition / MaxSize;
        
        scrollValue.x = MaxSize.x <= 0 ? _contentPosition.x / 1 : _contentPosition.x / MaxSize.x;
        scrollValue.y = MaxSize.y <= 0 ? _contentPosition.y / 1 : _contentPosition.y / MaxSize.y;
        
        horizontal.scroll.Set(-scrollValue.x, _scrollSize.x,false);
        vertical.scroll.Set(scrollValue.y, _scrollSize.y,false);
    }

    public void SetLayoutHorizontal()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    public void SetLayoutVertical()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    public void OnScroll(PointerEventData eventData)
    {
        _contentPosition += eventData.scrollDelta;
        UpdateScrollbar();
    }

    public void Rebuild(CanvasUpdate executing)
    {
        if (executing == CanvasUpdate.Prelayout)
        {
            UpdateCatch();
        }
        if (executing == CanvasUpdate.PostLayout)
        {
            UpdateCatch();
            UpdateScrollbar();
        }
    }

    public void LayoutComplete() { }

    public void GraphicUpdateComplete() { }

    public bool IsDestroyed()
    {
        return this == null; 
    }
    public void CalculateLayoutInputHorizontal() { }
    public void CalculateLayoutInputVertical() { }

    public float minWidth { get; }
    public float preferredWidth { get; }
    public float flexibleWidth { get; }
    public float minHeight { get; }
    public float preferredHeight { get; }
    public float flexibleHeight { get; }
    public int layoutPriority { get; }
}

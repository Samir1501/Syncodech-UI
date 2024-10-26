using UISystem;
using UnityEngine;

[ExecuteAlways]
public class Modal : WindowElement
{
    protected override void OnEnable()
    {
        base.OnEnable();
        OnValueChanged += () =>
        {
            Scale();
            CanvasActive();
        };
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

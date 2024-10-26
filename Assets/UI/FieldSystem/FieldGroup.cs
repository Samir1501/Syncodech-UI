using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Field
{
    public TextMeshProUGUI text;
    public Image image;

    public void Set(FieldContext context)
    {
        SetText(context.content);
        SetImage(context.sprite);
    }
    
    public void SetText(string content)
    {
        if(!text) return;
        text.SetText(content);
    }

    public void SetImage(Sprite sprite)
    {
        if(!image || !sprite) return;
        image.sprite = sprite;
        image.SetAllDirty();
    }
}
[Serializable]
public class FieldContext
{
    public string content;
    public Sprite sprite;

    public FieldContext(string content)
    {
        this.content = content;
    }

    public FieldContext(Sprite sprite)
    {
        this.sprite = sprite;
    }
    
    public FieldContext(string content, Sprite sprite)
    {
        this.content = content;
        this.sprite = sprite;
    }
}


public class FieldGroup : MonoBehaviour
{
    public Field[] fieldList;

    public void Set(FieldContext context)
    {
        fieldList[0].Set(context);
    }

    public bool Set(int index, FieldContext context)
    {
        if(index < 0 || index >= fieldList.Length) return false;
        fieldList[index].Set(context);
        return true;
    }

    public void Set(FieldContext[] fieldContextList)
    {
        int max = Mathf.Min(fieldList.Length, fieldContextList.Length);
        for (int i = 0; i < max; i++)
        {
            fieldList[i].Set(fieldContextList[i]);
        }
    }
}

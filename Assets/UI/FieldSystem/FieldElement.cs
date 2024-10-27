using System;

[Serializable]
public class FieldElement
{
    public Field field;
    public FieldGroup group;

    public void Set(FieldContext context)
    {
        if (group == null) field.Set(context);
        else group.Set(context);
    }

    public void Set(FieldContext[] contexts)
    {
        if(group == null) return;
        group.Set(contexts);
    }
}
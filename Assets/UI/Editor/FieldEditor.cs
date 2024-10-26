#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(FieldGroup))]
public class FieldEditor : Editor
{
    private SerializedProperty _fieldListPrp;
    
    private ReorderableList _reorderableFieldList;

    private void OnEnable()
    {
        _fieldListPrp = serializedObject.FindProperty("fieldList");
        
        _reorderableFieldList = new ReorderableList(serializedObject, _fieldListPrp, true, true, true, true);
        _reorderableFieldList.drawElementCallback += DrawElementCallback;
        _reorderableFieldList.headerHeight = 20;
        _reorderableFieldList.drawHeaderCallback += DrawHeaderCallback;
    }

    private void DrawHeaderCallback(Rect rect)
    {
        rect.x += 12;
        float margin = 8f;
        float halfWidth = (rect.width / 2) - margin;
        GUI.Label(new Rect(rect.x,rect.y,halfWidth,rect.height), "Text",EditorStyles.largeLabel);
        GUI.Label(new Rect(rect.x+halfWidth,rect.y,halfWidth,rect.height), "Image",EditorStyles.largeLabel);
    }

    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _fieldListPrp.GetArrayElementAtIndex(index);
        SerializedProperty textPrp = element.FindPropertyRelative("text");
        SerializedProperty imagePrp = element.FindPropertyRelative("image");

        float margin = 4f;
        float halfWidth = (rect.width / 2) - margin;
        EditorGUI.PropertyField(new Rect(rect.x, rect.y+margin/2, halfWidth, rect.height-margin), textPrp, GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x+halfWidth+margin, rect.y+margin/2, halfWidth, rect.height-margin), imagePrp, GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        _reorderableFieldList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
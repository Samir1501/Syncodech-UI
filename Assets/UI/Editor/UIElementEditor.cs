#if UNITY_EDITOR
using System;
using UISystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(UIBehaviour),true)]
public class UIElementEditor : Editor
{
    private SerializedProperty _interactablePrp;
    private SerializedProperty _durationPrp;
    private SerializedProperty _canvasGroupPrp;
    
    private SerializedProperty _fieldElementPrp;
    private SerializedProperty _fieldPrp;
    private SerializedProperty _fieldTextPrp;
    private SerializedProperty _fieldImagePrp;
    private SerializedProperty _fieldGroupPrp;

    private SerializedProperty _panelIndexPrp;
    private SerializedProperty _panelsPrp;
    
    private SerializedProperty _transitionsPrp;
    
    private SerializedProperty _dontMovePrp;
    private SerializedProperty _moveCurvePrp;
    private SerializedProperty _openSidePrp;
    private SerializedProperty _closeSidePrp;

    private SerializedProperty _currentIndexPrp;
    private SerializedProperty _nextIndexPrp;
    private SerializedProperty _radioButtonsPrp;
    
    private SerializedProperty _indexPrp;
    private SerializedProperty _isOnPrp;
    
    private SerializedProperty _barTypePrp;
    private SerializedProperty _textPrp;

    private SerializedProperty _direction;
    private SerializedProperty _wholeNumbers;
    private SerializedProperty _value;
    private SerializedProperty _min;
    private SerializedProperty _max;

    private SerializedProperty _size;
    private SerializedProperty _numberOfSteps;
    
    private SerializedProperty _fill;
    private SerializedProperty _handle;
    
    private SerializedProperty _onChangeEventPrp;
    private SerializedProperty _onClickEventPrp;

    private ReorderableList _transitionList;

    private float _elementHeight;
    private const float Margin = 3;
    private float VerticalPos => _elementHeight + Margin;

    private void InitProperty()
    {
        _elementHeight = EditorGUIUtility.singleLineHeight;
        _interactablePrp = serializedObject.FindProperty("interactable");
        _durationPrp = serializedObject.FindProperty("duration");
        _canvasGroupPrp = serializedObject.FindProperty("canvasGroup");
        _fieldElementPrp = serializedObject.FindProperty("field");
        if (_fieldElementPrp != null)
        {
            _fieldPrp = _fieldElementPrp.FindPropertyRelative("field");
            if (_fieldPrp != null)
            {
                _fieldTextPrp = _fieldPrp.FindPropertyRelative("text");
                _fieldImagePrp = _fieldPrp.FindPropertyRelative("image");
            }
            _fieldGroupPrp = _fieldElementPrp.FindPropertyRelative("group");
        }
        
        _panelIndexPrp = serializedObject.FindProperty("panelIndex");
        _panelsPrp = serializedObject.FindProperty("panels");
        
        _transitionsPrp = serializedObject.FindProperty("transitions");
        _dontMovePrp = serializedObject.FindProperty("dontMove");
        _openSidePrp = serializedObject.FindProperty("openSide");
        _closeSidePrp = serializedObject.FindProperty("closeSide");
        _moveCurvePrp = serializedObject.FindProperty("moveCurve");

        _currentIndexPrp = serializedObject.FindProperty("currentIndex");
        _nextIndexPrp = serializedObject.FindProperty("nextIndex");
        _radioButtonsPrp = serializedObject.FindProperty("radioButtons");
        
        _indexPrp = serializedObject.FindProperty("index");
        
        _isOnPrp = serializedObject.FindProperty("isOn");
        _barTypePrp = serializedObject.FindProperty("type");
        _textPrp = serializedObject.FindProperty("text");

        _direction = serializedObject.FindProperty("direction");
        _wholeNumbers = serializedObject.FindProperty("wholeNumbers");
        _value = serializedObject.FindProperty("value");
        _min = serializedObject.FindProperty("min");
        _max = serializedObject.FindProperty("max");
        
        _size = serializedObject.FindProperty("size");
        _numberOfSteps = serializedObject.FindProperty("numberOfSteps");
        
        _fill = serializedObject.FindProperty("fill");
        _handle = serializedObject.FindProperty("handle");
        
        _onClickEventPrp = serializedObject.FindProperty("onClick");
        _onChangeEventPrp = serializedObject.FindProperty("onChange");
    }
    
    private void OnEnable()
    {
        InitProperty();
        if(_transitionsPrp == null) return;
        _transitionList = new ReorderableList(serializedObject, _transitionsPrp,true,true,true,true);
        _transitionList.drawHeaderCallback += DrawHeaderCallback;
        _transitionList.drawElementCallback += target switch
        {
            Toggle => DrawActiveElementCallback,
            RadioButton => DrawActiveElementCallback,
            _ => DrawElementCallback,
        };
        _transitionList.elementHeightCallback += ElementHeightCallback;
    }

    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect,new GUIContent("Graphics"));
    }

    private float ElementHeightCallback(int index)
    {
        SerializedProperty element = _transitionsPrp.GetArrayElementAtIndex(index);
        TransitionType type = (TransitionType)element.FindPropertyRelative("transition").enumValueIndex;
        return type switch
        {
            TransitionType.Color => element.isExpanded ? VerticalPos * target switch
            {
                Toggle => 7,
                RadioButton => 7,
                _ => 6
            } : VerticalPos,
            TransitionType.Sprite => element.isExpanded ? VerticalPos * target switch
            {
                Toggle => 7,
                RadioButton => 7,
                _ => 6
            } : VerticalPos,
            TransitionType.Transform => element.isExpanded ? VerticalPos * target switch
            {
                Toggle => 27,
                RadioButton => 27,
                _ => 26
            } : VerticalPos,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void SingleLineProperty(int order,Rect rect,SerializedProperty prp, string displayName, string relativePropertyName = "")
    {
        float y = rect.y + 2.5f;
        SerializedProperty property = string.IsNullOrEmpty(relativePropertyName) ? prp : prp.FindPropertyRelative(relativePropertyName);
        EditorGUI.PropertyField(new Rect(rect.x, y + VerticalPos * order, rect.width, _elementHeight), property, new GUIContent(displayName));
    }
    
    private void DrawActiveElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _transitionsPrp.GetArrayElementAtIndex(index);

        SerializedProperty activeMode = element.FindPropertyRelative("activeMode");
        
        SerializedProperty normal = element.FindPropertyRelative(activeMode.boolValue ? "active" : "normal");
        SerializedProperty hover = element.FindPropertyRelative(activeMode.boolValue ? "activeHover" : "hover");
        SerializedProperty pressed = element.FindPropertyRelative(activeMode.boolValue ? "activePressed" : "pressed");
        SerializedProperty selected = element.FindPropertyRelative(activeMode.boolValue ? "activeSelected" : "selected");
        SerializedProperty disabled = element.FindPropertyRelative(activeMode.boolValue ? "activeDisabled" : "disabled");

        int order = 1;
        
        float y = rect.y + 2.5f;
        float x = rect.x + 12;
        
        float width = rect.width - 12;
        float halfWidth = width / 2 - Margin;
        float halfWidthHalf = halfWidth / 2;
        
        element.isExpanded = EditorGUI.Foldout(new Rect(x, y, rect.width, _elementHeight + 2), element.isExpanded, GUIContent.none);
        EditorGUI.PropertyField(new Rect(x + Margin + 2, y, halfWidth - Margin - 2, _elementHeight), element.FindPropertyRelative("graphic"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(x + Margin + halfWidth, y, halfWidthHalf, _elementHeight), element.FindPropertyRelative("transition"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(x + Margin * 2 + halfWidth + halfWidthHalf, y, halfWidthHalf, _elementHeight), element.FindPropertyRelative("duration"), GUIContent.none);
        if(!element.isExpanded) return;
        
        TransitionType type = (TransitionType)element.FindPropertyRelative("transition").enumValueIndex;
        SingleLineProperty(order++, rect, activeMode, "Active mode"); 
        GraphicsListElementGUI(type, ref order, rect, normal, hover, pressed, selected, disabled);
    }
    
    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _transitionsPrp.GetArrayElementAtIndex(index);
        
        SerializedProperty normal = element.FindPropertyRelative("normal");
        SerializedProperty hover = element.FindPropertyRelative("hover");
        SerializedProperty pressed = element.FindPropertyRelative("pressed");
        SerializedProperty selected = element.FindPropertyRelative("selected");
        SerializedProperty disabled = element.FindPropertyRelative("disabled");

        int order = 1;
        
        float y = rect.y + 2.5f;
        float x = rect.x + 12;
        
        float width = rect.width - 12;
        float halfWidth = width / 2 - Margin;
        float halfWidthHalf = halfWidth / 2;
        
        element.isExpanded = EditorGUI.Foldout(new Rect(x, y, rect.width, _elementHeight + 2), element.isExpanded, GUIContent.none);
        EditorGUI.PropertyField(new Rect(x + Margin + 2, y, halfWidth - Margin - 2, _elementHeight), element.FindPropertyRelative("graphic"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(x + Margin + halfWidth, y, halfWidthHalf, _elementHeight), element.FindPropertyRelative("transition"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(x + Margin * 2 + halfWidth + halfWidthHalf, y, halfWidthHalf, _elementHeight), element.FindPropertyRelative("duration"), GUIContent.none);
        if(!element.isExpanded) return;
        
        TransitionType type = (TransitionType)element.FindPropertyRelative("transition").enumValueIndex;

        GraphicsListElementGUI(type, ref order, rect, normal, hover, pressed, selected, disabled);
    }

    private void GraphicsListElementGUI(TransitionType type, ref int order, Rect rect,SerializedProperty normal, SerializedProperty hover, SerializedProperty pressed, SerializedProperty selected, SerializedProperty disabled)
    {
        float y = rect.y + 2.5f;

        switch (type)
        {
            case TransitionType.Color:
                SingleLineProperty(order++, rect, normal, "Normal", "color");
                SingleLineProperty(order++, rect, hover, "Hover", "color");
                SingleLineProperty(order++, rect, pressed, "Pressed", "color");
                SingleLineProperty(order++, rect, selected, "Selected", "color");
                SingleLineProperty(order, rect, disabled, "Disabled", "color");
                break;
            case TransitionType.Sprite:
                SingleLineProperty(order++, rect, normal, "Normal", "sprite");
                SingleLineProperty(order++, rect, hover, "Hover", "sprite");
                SingleLineProperty(order++, rect, pressed, "Pressed", "sprite");
                SingleLineProperty(order++, rect, selected, "Selected", "sprite");
                SingleLineProperty(order, rect, disabled, "Disabled", "sprite");
                break;
            case TransitionType.Transform:
                EditorGUI.SelectableLabel(new Rect(rect.x, y + VerticalPos*order++, rect.width, _elementHeight), "Normal",GUI.skin.box);
                SingleLineProperty(order++, rect, normal, "Rect", "rect");
                order++;
                SingleLineProperty(order++, rect, normal, "Rotation", "eulerAngles");
                SingleLineProperty(order++, rect, normal, "Scale", "scale");
                EditorGUI.SelectableLabel(new Rect(rect.x, y + VerticalPos * order++, rect.width, _elementHeight), "Hover", GUI.skin.box);
                SingleLineProperty(order++, rect, hover, "Rect", "rect");
                order++;
                SingleLineProperty(order++, rect, hover, "Rotation", "eulerAngles");
                SingleLineProperty(order++, rect, hover, "Scale", "scale");
                EditorGUI.SelectableLabel(new Rect(rect.x, y + VerticalPos*order++, rect.width, _elementHeight), "Pressed",GUI.skin.box);
                SingleLineProperty(order++, rect, pressed, "Rect", "rect");
                order++;
                SingleLineProperty(order++, rect, pressed, "Rotation", "eulerAngles");
                SingleLineProperty(order++, rect, pressed, "Scale", "scale");
                EditorGUI.SelectableLabel(new Rect(rect.x, y + VerticalPos*order++, rect.width, _elementHeight), "Selected",GUI.skin.box);
                SingleLineProperty(order++, rect, selected, "Rect", "rect");
                order++;
                SingleLineProperty(order++, rect, selected, "Rotation", "eulerAngles");
                SingleLineProperty(order++, rect, selected, "Scale", "scale");
                EditorGUI.SelectableLabel(new Rect(rect.x, y + VerticalPos*order++, rect.width, _elementHeight), "Disabled",GUI.skin.box);
                SingleLineProperty(order++, rect, disabled, "Rect", "rect");
                order++;
                SingleLineProperty(order++, rect, disabled, "Rotation", "eulerAngles");
                SingleLineProperty(order, rect, disabled, "Scale", "scale");
                break;
        }
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        PanelManagerGUI();
        ModalGUI();
        PanelGUI();
        SelectableGUI();
        BarGUI();
        ScrollBarGUI();
        ButtonGUI();
        RadioButtonGroupGUI();
        RadioButtonGUI();
        ToggleGUI();
        SliderGUI();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void Field()
    {
        if (_fieldGroupPrp.objectReferenceValue == null)
        {
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_fieldTextPrp);
            EditorGUILayout.PropertyField(_fieldImagePrp);
        }
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(_fieldGroupPrp);
    }
    
    private void PanelManagerGUI()
    {
        if(serializedObject.targetObject is not PanelManager) return;
        _panelIndexPrp.intValue = EditorGUILayout.IntSlider(_panelIndexPrp.intValue, 0, _panelsPrp.arraySize - 1);
        EditorGUILayout.PropertyField(_panelsPrp);
    }

    private void ModalGUI()
    {
        if (serializedObject.targetObject is not Modal modal) return;
        EditorGUILayout.PropertyField(_durationPrp);
        EditorGUILayout.PropertyField(_moveCurvePrp);
        Field();
        EditorGUILayout.PropertyField(_canvasGroupPrp);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if(GUILayout.Button("Open")) modal.Open();
        if(GUILayout.Button("Close")) modal.Close();
        EditorGUILayout.EndHorizontal();
    }
    private void PanelGUI()
    {
        if(serializedObject.targetObject is not Panel) return;
        Field();
        EditorGUILayout.PropertyField(_durationPrp);
        EditorGUILayout.PropertyField(_moveCurvePrp);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Open side");
        _openSidePrp.enumValueIndex = GUILayout.SelectionGrid(_openSidePrp.enumValueIndex, _openSidePrp.enumDisplayNames, 4,EditorStyles.miniButtonMid);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Close side");
        _closeSidePrp.enumValueIndex = GUILayout.SelectionGrid(_closeSidePrp.enumValueIndex, _closeSidePrp.enumDisplayNames, 4,EditorStyles.miniButtonMid);
        EditorGUILayout.EndVertical();
        EditorGUILayout.PropertyField(_value);
        EditorGUILayout.PropertyField(_dontMovePrp);
    }
    
    private void SelectableGUI()
    {
        if (serializedObject.targetObject is not Selectable) return;
        _transitionList.DoLayoutList();
        EditorGUILayout.PropertyField(_interactablePrp);
    }

    private void ScrollBarGUI()
    {
        if(serializedObject.targetObject is not Scroll) return;
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_handle.FindPropertyRelative("rect"),new GUIContent("Handle rect"));
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_direction);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_value);
        EditorGUILayout.PropertyField(_size);
        EditorGUILayout.PropertyField(_numberOfSteps);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_onChangeEventPrp);
    }

    private void BarGUI()
    {
        if(serializedObject.targetObject is not Bar) return;
        _barTypePrp.enumValueIndex = GUILayout.SelectionGrid(_barTypePrp.enumValueIndex, _barTypePrp.enumDisplayNames, 2,EditorStyles.miniButtonMid);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_textPrp);
        EditorGUILayout.PropertyField(_fill.FindPropertyRelative("rect"), new GUIContent("Fill rect"));
        if (_fill.FindPropertyRelative("rect").objectReferenceValue == null) return;
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_direction);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_wholeNumbers);
        _value.floatValue = _wholeNumbers.boolValue 
            ? Mathf.Round(EditorGUILayout.Slider("Value",_value.floatValue, _min.floatValue, _max.floatValue)) 
            : EditorGUILayout.Slider("Value",_value.floatValue, _min.floatValue, _max.floatValue);
        EditorGUILayout.PropertyField(_min);
        EditorGUILayout.PropertyField(_max);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_onChangeEventPrp);
    }

    private void ButtonGUI()
    {
        if(serializedObject.targetObject is not Button) return;
        Field();
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(_onClickEventPrp);
    }

    private void RadioButtonGroupGUI()
    {
        if(serializedObject.targetObject is not RadioButtonGroup) return;
        EditorGUILayout.LabelField($"Current index : {_currentIndexPrp.intValue}",EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Next index : {_nextIndexPrp.intValue}",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_radioButtonsPrp);
        EditorGUILayout.PropertyField(_onChangeEventPrp);
    }
    
    private void RadioButtonGUI()
    {
        if(serializedObject.targetObject is not RadioButton) return;
        Field();     
        
        EditorGUILayout.LabelField($"Index : {_indexPrp.intValue}",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_isOnPrp);
        
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(_onChangeEventPrp);
    }
    
    private void ToggleGUI()
    {
        if(serializedObject.targetObject is not Toggle) return;
        Field();
        
        EditorGUILayout.PropertyField(_isOnPrp);
        
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(_onChangeEventPrp);
    }

    private void SliderGUI()
    {
        if(serializedObject.targetObject is not Slider) return;
        
        EditorGUILayout.Space(5);
       
        EditorGUILayout.PropertyField(_fill.FindPropertyRelative("rect"), new GUIContent("Fill rect"));
        if (_fill.FindPropertyRelative("rect").objectReferenceValue == null) return;
        
        EditorGUILayout.PropertyField(_handle.FindPropertyRelative("rect"),new GUIContent("Handle rect"));

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_direction);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_wholeNumbers);
        _value.floatValue = _wholeNumbers.boolValue 
            ? Mathf.Round(EditorGUILayout.Slider("Value",_value.floatValue, _min.floatValue, _max.floatValue)) 
            : EditorGUILayout.Slider("Value",_value.floatValue, _min.floatValue, _max.floatValue);
        EditorGUILayout.PropertyField(_min);
        EditorGUILayout.PropertyField(_max);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_onChangeEventPrp);
    }
}
#endif
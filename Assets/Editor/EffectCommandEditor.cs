using UnityEngine;
using UnityEditor;

// [CustomPropertyDrawer(typeof(EffectCommand))]
public class EffectCommandDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Indent the content
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Save original indent level
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var _actionRect = new Rect(position.x, position.y, 100, position.height);
        var _subjectRect = new Rect(position.x, position.y + 20, 100, position.height);
        var _whoReceiverIndexRect = new Rect(position.x, position.y + 40, 100, position.height);
        var _whereReceiverIndexRect = new Rect(position.x, position.y + 60, 100, position.height);
        var _fromReceiverIndexRect = new Rect(position.x, position.y + 80, 100, position.height);
        var _toReceiverIndexRect = new Rect(position.x, position.y +100, 100, position.height);
        var _newCardRect = new Rect(position.x, position.y +120, 100, position.height);

        // Get properties
        SerializedProperty _actionProp = property.FindPropertyRelative("_action");
        SerializedProperty _subject = property.FindPropertyRelative("_subject");
        SerializedProperty _whoReceiverIndexProp = property.FindPropertyRelative("_whoReceiverIndex");
        SerializedProperty _whereReceiverIndexProp = property.FindPropertyRelative("_whereReceiverIndex");
        SerializedProperty _fromReceiverIndexProp = property.FindPropertyRelative("_fromReceiverIndex");
        SerializedProperty _toReceiverIndexProp = property.FindPropertyRelative("_toReceiverIndex");
        SerializedProperty _newCardProp = property.FindPropertyRelative("_newCard");
        

        // Draw Action field
        EditorGUI.PropertyField(_actionRect, _actionProp, GUIContent.none);

        
        if ((EffectCommand.What) _actionProp.enumValueIndex == EffectCommand.What.Discard)
        {
            EditorGUI.PropertyField(_whoReceiverIndexRect, _whoReceiverIndexProp, GUIContent.none);
            EditorGUI.PropertyField(_whereReceiverIndexRect, _whereReceiverIndexProp, GUIContent.none);
        }

        // Restore indent level
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
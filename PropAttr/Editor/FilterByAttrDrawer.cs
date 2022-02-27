using UnityEditor;
using UnityEngine;

namespace Util.PropAttr.Editor
{
    [CustomPropertyDrawer(typeof(FilterByAttribute))]
    public class FilterByAttrDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!((FilterByAttribute)attribute).CanDraw(property.serializedObject.targetObject)) return;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // CanDraw() 就 Draw
            return ((FilterByAttribute)attribute).CanDraw(property.serializedObject.targetObject) ? EditorGUI.GetPropertyHeight(property) : 0;
        }
    }

}

using FullCircleData.Editor.Editor.Extensions;
using FullCircleData.Editor.Editor.Utils;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;

namespace FullCircleData.Editor.Editor.Properties
{
    [CustomPropertyDrawer(typeof(Observable<>))]
    public class ObservablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawValueProperty(position, property, label);
        }

        public static void DrawValueProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.GetTarget() as IObservable;
            
            ObservableUtils.DrawStatus(position, target);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label, true);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                target.DispatchChange();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"), label, true);
        }
    }
}
using FullCircleData.Editor.Editor.Extensions;
using FullCircleData.Editor.Editor.Utils;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;

namespace FullCircleData.Editor.Editor.Properties
{
    [CustomPropertyDrawer(typeof(Signal<>))]
    public class GenericSignalPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.GetTarget() as IObservable;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            var valueProperty = property.FindPropertyRelative("value");
            ObservableUtils.DrawStatus(position, target);
            EditorGUILayout.PropertyField(valueProperty, GUIContent.none);
            
            if (GUILayout.Button("Send"))
            {
                property.serializedObject.ApplyModifiedProperties();
                target.DispatchChange();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}
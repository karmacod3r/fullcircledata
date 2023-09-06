using FullCircleData.Editor.Editor.Extensions;
using FullCircleData.Editor.Editor.Utils;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;

namespace FullCircleData.Editor.Editor.Properties
{
    [CustomPropertyDrawer(typeof(Signal))]
    public class SignalPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.GetTarget() as Signal;

            EditorGUILayout.BeginHorizontal();
            ObservableUtils.DrawStatus(position, target);
            EditorGUILayout.PrefixLabel(label);
            
            if (GUILayout.Button("Send"))
            {
                target.Send();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}
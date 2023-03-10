using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullCircleData.Attributes;
using FullCircleData.Editor.Editor.Extensions;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;

namespace FullCircleData.Editor.Editor.Properties
{
    [CustomPropertyDrawer(typeof(AutoDropdownAttribute))]
    public class AutoDropdownAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var options = new string[0];
            List<object> objectList = null;
            var index = 0;

            var dropDownAttribute = (AutoDropdownAttribute) attribute;
            var valueProperty = string.IsNullOrEmpty(dropDownAttribute.valuePropertyPath)
                ? property
                : property.FindPropertyRelative(dropDownAttribute.valuePropertyPath);

            var listTargetObject = ResolvePath(property.serializedObject.targetObject, dropDownAttribute.listPropertyPath.Split('.'));
            if (listTargetObject == null)
            {
                Debug.LogError("Couldn't find list " + dropDownAttribute.listPropertyPath);
                return;
            }

            if (listTargetObject is IEnumerable<object> list)
            {
                var optionsList = list.Select(o => o.ToString()).ToList();
                if (dropDownAttribute.allowNone)
                {
                    optionsList.Insert(0, "None");
                }

                options = optionsList.ToArray();

                objectList = list.ToList();
                if (dropDownAttribute.allowNone)
                {
                    objectList.Insert(0, null);
                }

                index = valueProperty.propertyType == SerializedPropertyType.String
                    ? objectList.FindIndex(o => o?.ToString() == valueProperty.stringValue)
                    : objectList.FindIndex(o => o == valueProperty.GetTarget());
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);

            index = EditorGUILayout.Popup(label, index, options);

            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck())
            {
                var observable = property.GetTarget() as IObservable;
                if (objectList != null && observable != null)
                {
                    object value;
                    value = valueProperty.propertyType == SerializedPropertyType.String 
                        ? dropDownAttribute.allowNone && index == 0 ? "" : objectList[index].ToString() 
                        : objectList[index];
                    observable.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public).SetValue(observable, value);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }

                if (observable == null && valueProperty.propertyType == SerializedPropertyType.String)
                {
                    valueProperty.stringValue = dropDownAttribute.allowNone && index == 0 ? "" : options[index];
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        private static object ResolvePath(object target, string[] path, int index = 0)
        {
            if (target == null) return null;
            object value = null;

            var targetType = target.GetType();
            var field = targetType.GetField(path[index], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (field != null)
            {
                value = field.GetValue(target);
            } else
            {
                var property = targetType.GetProperty(path[index], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (property == null) return null;

                value = property.GetValue(target);
            }

            if (index < path.Length - 1)
            {
                return ResolvePath(value, path, index + 1);
            }

            return value;
        }
    }
}
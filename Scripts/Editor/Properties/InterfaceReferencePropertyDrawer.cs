using System;
using System.Collections;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FullCircleData.Editor.Editor.Properties
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
    public class InterfaceReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetProperty = property.FindPropertyRelative("targetObject");

            var fieldRect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.LabelField(fieldRect, GetReferenceName(targetProperty), EditorStyles.objectField);

            GUI.enabled = targetProperty.objectReferenceValue != null;
            var buttonWidth = fieldRect.height + EditorGUIUtility.standardVerticalSpacing * 2;
            var buttonRect = new Rect(fieldRect.x + fieldRect.width - buttonWidth, fieldRect.y, buttonWidth,
                fieldRect.height);
            if (GUI.Button(buttonRect, "✕", EditorStyles.miniButton))
            {
                ClearValue(targetProperty);
            }

            GUI.enabled = true;

            ProcessDragAndDrop(GetInterfaceType(), targetProperty);
            ProcessMouseEvents(targetProperty, fieldRect);
            ProcessKeyboardEvents(targetProperty);

            EditorGUI.EndProperty();
        }

        private string GetReferenceName(SerializedProperty targetProperty)
        {
            return targetProperty.objectReferenceValue == null
                ? $"None ({GetInterfaceType().Name})"
                : $"{targetProperty.objectReferenceValue.name} ({targetProperty.objectReferenceValue.GetType().Name})";
        }

        private void ProcessMouseEvents(SerializedProperty property, Rect fieldRect)
        {
            if (Event.current.type == EventType.MouseDown && fieldRect.Contains(Event.current.mousePosition))
                if (property.objectReferenceValue != null)
                {
                    if (Event.current.clickCount == 2)
                    {
                        Selection.activeObject = property.objectReferenceValue;
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(property.objectReferenceValue);
                    }
                }
        }

        private void ProcessKeyboardEvents(SerializedProperty targetProperty)
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
                {
                    Event.current.Use();
                    ClearValue(targetProperty);
                }
            }
        }

        private static void ClearValue(SerializedProperty targetProperty)
        {
            ApplyValue(targetProperty, null);
        }

        private static void ApplyValue(SerializedProperty targetProperty, Object acceptableObject)
        {
            targetProperty.objectReferenceValue = acceptableObject;
            targetProperty.serializedObject.ApplyModifiedProperties();

            // trigger change to enable ObservablePropertyDrawer to dispatch change
            GUI.changed = true;
        }

        private void ProcessDragAndDrop(Type type, SerializedProperty targetProperty)
        {
            var targetScene = GetObjectReferenceScene(targetProperty.serializedObject.targetObject);
            
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = GetAcceptableObject(type, DragAndDrop.objectReferences[0], targetScene) != null
                    ? DragAndDropVisualMode.Copy
                    : DragAndDropVisualMode.Rejected;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                var acceptableObject = GetAcceptableObject(type, DragAndDrop.objectReferences[0], targetScene);
                if (acceptableObject == null)
                {
                    return;
                }

                // consume drag data.
                DragAndDrop.AcceptDrag();

                ApplyValue(targetProperty, acceptableObject);
            }
        }

        private Object GetAcceptableObject(Type interfaceType, Object objectReference, Scene targetScene)
        {
            if (interfaceType.IsAssignableFrom(objectReference.GetType()))
            {
                return FilterSameSceneReference(objectReference, targetScene);
            }

            if (objectReference is GameObject gameObject)
            {
                var component = gameObject.GetComponent(interfaceType);
                if (component != null)
                {
                    return FilterSameSceneReference(component, targetScene);
                }
            }

            return null;
        }

        private Object FilterSameSceneReference(Object objectReference, Scene targetScene)
        {
            var scene = GetObjectReferenceScene(objectReference);
            return PrefabUtility.IsPartOfAnyPrefab(objectReference) || scene == targetScene || scene == default
                ? objectReference
                : null;
        }

        private Scene GetObjectReferenceScene(Object objectReference)
        {
            if (objectReference is GameObject gameObject)
            {
                return gameObject.scene;
            }
            
            if (objectReference is Component component)
            {
                return component.gameObject.scene;
            }
            
            return default;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        System.Type GetInterfaceType()
        {
            System.Type type = fieldInfo.FieldType;
            System.Type[] typeArguments = type.GenericTypeArguments;
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                typeArguments = fieldInfo.FieldType.GenericTypeArguments[0].GenericTypeArguments;
            }

            if (typeArguments == null || typeArguments.Length == 0)
                return null;
            return typeArguments[0];
        }
    }
}
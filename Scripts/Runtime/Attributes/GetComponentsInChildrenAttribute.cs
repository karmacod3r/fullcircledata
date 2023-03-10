using System;
using System.Reflection;
using UnityEngine;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields of types extending Component[] to auto-fill them with transform.GetComponentsInChildren().
    /// </summary>
    /// <see cref="BestBehaviour"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentsInChildrenAttribute : Attribute
    {
        /// <summary>
        /// Include inactive game objects
        /// </summary>
        public bool includeInactive = true;

        public GetComponentsInChildrenAttribute(bool includeInactive)
        {
            this.includeInactive = includeInactive;
        }

        public GetComponentsInChildrenAttribute()
        {
        }
        
        public void TryInitialize(object obj, FieldInfo info, Transform transform)
        {
            if (! info.FieldType.IsArray || ! typeof(Component).IsAssignableFrom(info.FieldType.GetElementType()))
            {
                throw new InvalidCastException("Field type has to extend UnityEngine.Component[]");
            }

            var children = transform.GetComponentsInChildren(info.FieldType.GetElementType(), includeInactive);
            var elementType = info.FieldType.GetElementType();
            var convertedChildren = Array.CreateInstance(elementType, children.Length);
            Array.Copy(children, convertedChildren, children.Length);
            
            info.SetValue(obj, convertedChildren);
        }
    }
}
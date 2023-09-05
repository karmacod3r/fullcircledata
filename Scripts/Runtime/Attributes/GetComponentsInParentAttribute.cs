using System;
using System.Reflection;
using UnityEngine;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields of types extending Component[] to auto-fill them with transform.GetComponentsInParent().
    /// </summary>
    /// <see cref="BestBehaviour"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentsInParentAttribute : Attribute
    {
        /// <summary>
        /// Include inactive game objects
        /// </summary>
        public bool includeInactive = true;

        public GetComponentsInParentAttribute(bool includeInactive)
        {
            this.includeInactive = includeInactive;
        }

        public GetComponentsInParentAttribute()
        {
        }
        
        public void TryInitialize(object obj, FieldInfo info, Transform transform)
        {
            if (! info.FieldType.IsArray)
            {
                throw new InvalidCastException("Field type has to be array");
            }

            var parents = transform.GetComponentsInParent(info.FieldType.GetElementType(), includeInactive);
            var elementType = info.FieldType.GetElementType();
            var convertedparents = Array.CreateInstance(elementType, parents.Length);
            Array.Copy(parents, convertedparents, parents.Length);
            
            info.SetValue(obj, convertedparents);
        }
    }
}
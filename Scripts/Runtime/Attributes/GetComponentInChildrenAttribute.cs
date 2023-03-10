using System;
using System.Reflection;
using UnityEngine;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields of types extending Component to auto-fill them with transform.GetComponentInChildren().
    /// </summary>
    /// <see cref="BestBehaviour"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentInChildrenAttribute : Attribute
    {
        /// <summary>
        /// Include inactive game objects
        /// </summary>
        public bool includeInactive = true;

        public GetComponentInChildrenAttribute(bool includeInactive)
        {
            this.includeInactive = includeInactive;
        }

        public GetComponentInChildrenAttribute()
        {
        }
        
        public void TryInitialize(object obj, FieldInfo info, Transform transform)
        {
            if (! typeof(Component).IsAssignableFrom(info.FieldType))
            {
                throw new InvalidCastException("Field type has to extend UnityEngine.Component");
            }

            info.SetValue(obj, transform.GetComponentInChildren(info.FieldType, includeInactive));
        }
    }
}
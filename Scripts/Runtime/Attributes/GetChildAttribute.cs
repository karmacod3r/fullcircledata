using System;
using System.Reflection;
using UnityEngine;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields of type Transform to auto-fill them with transform.GetChild().
    /// </summary>
    /// <see cref="BestBehaviour"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class GetChildAttribute : Attribute
    {
        /// <summary>
        /// The child index
        /// </summary>
        public uint index;

        public GetChildAttribute(uint index)
        {
            this.index = index;
        }
        
        public void TryInitialize(object obj, FieldInfo info, Transform transform)
        {
            if (! typeof(Transform).IsAssignableFrom(info.FieldType))
            {
                throw new InvalidCastException("Field type has to extend UnityEngine.Transform");
            }

            info.SetValue(obj, index < transform.childCount ? transform.GetChild((int) index) : null);
        }
    }
}
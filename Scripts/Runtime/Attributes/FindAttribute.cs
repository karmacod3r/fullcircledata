using System;
using System.Reflection;
using UnityEngine;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields of type Transform to auto-fill them with transform.Find().
    /// </summary>
    /// <see cref="BestBehaviour"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class FindAttribute : Attribute
    {
        /// <summary>
        /// Argument for transform.Find()
        /// </summary>
        public string path;

        public FindAttribute(string path)
        {
            this.path = path;
        }
        
        public void TryInitialize(object obj, FieldInfo info, Transform transform)
        {
            var t = transform.Find(path);
            
            if (! typeof(Transform).IsAssignableFrom(info.FieldType))
            {
                info.SetValue(obj, t?.GetComponent(info.FieldType));
                return;
            }

            info.SetValue(obj, t);
        }
    }
}
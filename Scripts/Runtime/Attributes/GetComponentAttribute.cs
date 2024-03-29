using System;
using System.Reflection;
using UnityEngine;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields of types extending Component to auto-fill them with transform.GetComponent().
    /// </summary>
    /// <see cref="BestBehaviour"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentAttribute : Attribute
    {
        public void TryInitialize(object obj, FieldInfo info, Transform transform)
        {
            info.SetValue(obj, transform.GetComponent(info.FieldType));
        }
    }
}
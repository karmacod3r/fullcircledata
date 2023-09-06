using System;
using UnityEngine.Serialization;

namespace FullCircleData.Properties
{
    using UnityEngine;
 
    [Serializable]
    public struct InterfaceReference<T> where T : class
    {
        [SerializeReference] private Object targetObject;
        public T Target => targetObject as T;

        private bool Equals(InterfaceReference<T> other)
        {
            return Equals(targetObject, other.targetObject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((InterfaceReference<T>)obj);
        }

        public override int GetHashCode()
        {
            return (targetObject != null ? targetObject.GetHashCode() : 0);
        }
    }
}
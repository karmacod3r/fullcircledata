using UnityEngine;

namespace FullCircleData.Extensions
{
    public static class ComponentExtension
    {
        private static string GetTransformPath(Transform current) {
            if (current.parent == null)
            {
                return "/" + current.name;
            }
            return current.parent.GetPath() + "/" + current.name;
        }

        public static string GetPath(this Component component) {
            if (component is Transform)
            {
                return GetTransformPath(component.transform);
            }
            
            return GetTransformPath(component.transform) + "/" + component.GetType();
        }
    }
}
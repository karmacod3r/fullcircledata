using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FullCircleData.Extensions
{
    /// <summary>
    /// Collection of static extension methods for the Type class
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Get fields by binding flags optionally including all base types
        /// </summary>
        /// <param name="targetType">Current Type instance</param>
        /// <param name="bindingFlags">Binding flags used for filtering</param>
        /// <param name="includeBaseTypes">When true merge in all matching base type fields</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFields(this Type targetType, BindingFlags bindingFlags, bool includeBaseTypes, Type rootType = null)
        {
            var types = new List<Type> {targetType};

            if (includeBaseTypes)
            {
                while (types.Last().BaseType != null && types.Last().BaseType != rootType)
                {
                    types.Add(types.Last().BaseType);
                }
            }

            return types.Select(type => type.GetFields(bindingFlags))
                .SelectMany(fieldInfos => fieldInfos);
        }        
        
        /// <summary>
        /// Get methods by binding flags optionally including all base types
        /// </summary>
        /// <param name="targetType">Current Type instance</param>
        /// <param name="bindingFlags">Binding flags used for filtering</param>
        /// <param name="includeBaseTypes">When true merge in all matching base type methods</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethods(this Type targetType, BindingFlags bindingFlags, bool includeBaseTypes, Type rootType = null)
        {
            var types = new List<Type> {targetType};

            if (includeBaseTypes)
            {
                while (types.Last().BaseType != null && types.Last().BaseType != rootType)
                {
                    types.Add(types.Last().BaseType);
                }
            }

            return types.Select(type => type.GetMethods(bindingFlags))
                .SelectMany(infos => infos);
        }
        
        public static MethodInfo GetMethodIncludingBaseTypes(this Type target, string methodName, BindingFlags bindingFlags)
        {
            var type = target;
            
            while (type != null)
            {
                var methodInfo = type.GetMethod(methodName, bindingFlags);

                if (methodInfo != null)
                {
                    return methodInfo;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
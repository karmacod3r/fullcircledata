using System;
using System.Collections.Generic;

namespace FullCircleData.Extensions
{
    public static class ListExtension
    {
        public static bool IndexValid<T>(this List<T> list, int index) => index >= 0 && list.Count > index;

        public static void EnsureSize<T>(this List<T> list, int count, int capacityBump = 100)
        {
            if (count < 0) return;
            
            if (list.Count < count)
            {
                list.AddRange(new T[count - list.Count]);
            }
            if (list.Count > count)
            {
                list.RemoveRange(count, list.Count - count);
            }

            if (list.Count >= list.Capacity)
            {   
                list.Capacity += capacityBump;
            }
        }

        public static void EnsureSizeAndCreateInstances<T>(this List<T> list, int count, int capacityBump = 100)
        {
            if (count < 0) return;
            
            if (list.Count < count)
            {
                while (list.Count < count)
                {
                    list.Add(Activator.CreateInstance<T>());
                }
            }
            if (list.Count > count)
            {
                list.RemoveRange(count, list.Count - count);
            }

            if (list.Count >= list.Capacity)
            {   
                list.Capacity += capacityBump;
            }
        }

        public static void ForEachWithIndex<T>(this List<T> list, Action<T, int> callback)
        {
            for (var i = 0; i < list.Count; i++)
            {
                callback(list[i], i);
            }
        }
    }
}
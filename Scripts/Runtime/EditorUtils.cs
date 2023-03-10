
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace FullCircleData
{
    public static class EditorUtils
    {
        public static void SetDirty(Object target)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && target != null)
            {
                EditorUtility.SetDirty(target);
            }
#endif
        }
        
        public static void Destroy(Object o)
        {
            if (o == null) return;
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(o);
                return;
            }
#endif

            if (o is GameObject gameObject)
            {
                gameObject.SetActive(false);
            }
            Object.Destroy(o);
        }        
        
        public static void DestroyImmediate(Object o)
        {
            if (o == null) return;
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (Selection.activeObject == o)
                {
                    Selection.activeObject = null;
                }

                try
                {
                    Object.DestroyImmediate(o, true);
                }
                catch
                {
                    // GameObjects in Prefabs can't be removed. Deactivate instead. 
                    if (o is GameObject go && PrefabUtility.IsPartOfAnyPrefab(go))
                    {
                        go.SetActive(false);
                    }
                }

                return;
            }
#endif

            if (o is GameObject gameObject)
            {
                gameObject.SetActive(false);
            }
            Object.DestroyImmediate(o);
        }
    }
}
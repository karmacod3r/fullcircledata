using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullCircleData.Attributes;
using FullCircleData.Extensions;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FullCircleData
{
    /// <summary>
    /// This is a base behaviour that implements observer functionality and provides convenience features.
    /// Use it instead of MonoBehaviour and decorate with custom field attributes.
    /// </summary>
    /// <see cref="BindingAttribute"/>
    /// <see cref="FindAttribute"/>
    /// <see cref="GetChildAttribute"/>
    /// <see cref="GetComponentAttribute"/>
    /// <see cref="GetComponentInChildrenAttribute"/>
    /// <see cref="GetComponentInParentAttribute"/>
    /// <see cref="GetComponentsInChildrenAttribute"/>
    /// <see cref="GetComponentsInParentAttribute"/>
    [ExecuteAlways]
    public class BestBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Cached observers
        /// </summary>
        private List<IObserver> observers;

        /// <summary>
        /// cached observables
        /// </summary>
        private List<IObservable> observables;

        private Dictionary<IObservable, Action> observableChangeListeners;
        private bool initialized;

        [NonSerialized] public RectTransform rectTransform;

        private static Dictionary<Type, IEnumerable<FieldInfo>> fieldCache = new Dictionary<Type, IEnumerable<FieldInfo>>();

        private IEnumerable<FieldInfo> AllFields
        {
            get
            {
                var type = GetType();
                if (!fieldCache.ContainsKey(type))
                {
                    fieldCache[type] = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, true, typeof(BestBehaviour));
                }

                return fieldCache[type];
            }
        }
        
        private static Dictionary<Type, IEnumerable<MethodInfo>> methodCache = new Dictionary<Type, IEnumerable<MethodInfo>>();

        private IEnumerable<MethodInfo> AllMethods
        {
            get
            {
                var type = GetType();
                if (!methodCache.ContainsKey(type))
                {
                    methodCache[type] = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, true, typeof(BestBehaviour));
                }

                return methodCache[type];
            }
        }


        /// <summary>
        /// Unity event: Connect Observers and fill attributed fields
        /// </summary>
        protected virtual void OnEnable()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            if (initialized) return;
            initialized = true;

            rectTransform = transform as RectTransform;

            ConnectObservers();
            ConnectObservables();
            ParseCustomAttributes();
            StartObserving();
            ConnectSignalReceivers();
        }

        /// <summary>
        /// Iterates all fields that are decorated with custom attributes and tries to initialize them
        /// </summary>
        private void ParseCustomAttributes()
        {
            foreach (var field in AllFields)
            {
                if (!field.CustomAttributes.Any()) continue;

                field.GetCustomAttribute<GetComponentAttribute>()?.TryInitialize(this, field, transform);
                field.GetCustomAttribute<GetComponentInChildrenAttribute>()?.TryInitialize(this, field, transform);
                field.GetCustomAttribute<GetComponentsInChildrenAttribute>()?.TryInitialize(this, field, transform);
                field.GetCustomAttribute<GetComponentInParentAttribute>()?.TryInitialize(this, field, transform);
                field.GetCustomAttribute<GetComponentsInParentAttribute>()?.TryInitialize(this, field, transform);
                field.GetCustomAttribute<FindAttribute>()?.TryInitialize(this, field, transform);
                field.GetCustomAttribute<GetChildAttribute>()?.TryInitialize(this, field, transform);
            }
        }

        /// <summary>
        /// Unity event: disconnect observers
        /// </summary>
        protected virtual void OnDisable()
        {
            if (!initialized) return;
            initialized = false;

            DisconnectObservers();
            DisconnectObservables();
        }

        private void OnDestroy()
        {
            OnDisable();
        }

        protected virtual void OnBeforeTransformParentChanged()
        {
            OnDisable();
        }

        protected virtual void OnTransformParentChanged()
        {
            OnEnable();
        }

        /// <summary>
        /// Retrieves all fields implementing IObserver and tries to connect them to Model fields in the parent transform tree. 
        /// </summary>
        private void ConnectObservers()
        {
            observers = new List<IObserver>();

            foreach (var field in AllFields)
            {
                if (!field.CustomAttributes.Any()) continue;

                var binding = field.GetCustomAttribute<BindingAttribute>();
                if (binding == null) continue;

                if (!typeof(IObserver).IsAssignableFrom(field.FieldType))
                {
                    Debug.LogError($"BindingAttribute expects a field implementing IObserver at {field.Name}");
                    continue;
                }

                var observer = field.GetValue(this) as IObserver;
                if (observer == null)
                {
                    // create new instance
                    observer = Activator.CreateInstance(field.FieldType) as IObserver;
                    field.SetValue(this, observer);
                }

                if (!string.IsNullOrEmpty(binding.changeListener))
                {
                    var methodInfo = GetType().GetMethod(binding.changeListener, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo == null)
                    {
                        Debug.LogError("Change listener for " + binding + " not found");
                        continue;
                    }

                    observer.Connect(transform, binding.observableName, () =>
                    {
                        if (initialized) methodInfo.Invoke(this, null);
                    });
                } else
                {
                    observer.Connect(transform, binding.observableName);
                }

                observers.Add(observer);
            }
        }

        /// <summary>
        /// Retrieves all methods decorated with ReceiveAttrivute and tries to connect them to Model fields in the parent transform tree. 
        /// </summary>
        private void ConnectSignalReceivers()
        {
            foreach (var method in AllMethods)
            {
                if (!method.CustomAttributes.Any()) continue;
                
                var binding = method.GetCustomAttribute<ReceiveAttribute>();
                if (binding == null) continue;

                var parameters = method.GetParameters();
                var observableType = parameters.Length == 0 ? typeof(SignalObserver) : parameters[0].ParameterType;
                
                var observer = Activator.CreateInstance(observableType) as IObserver;
                if (parameters.Length == 0)
                {
                    observer.Connect(transform, binding.observableName, () => method.Invoke(this, null));
                } else if (parameters.Length == 1)
                {
                    observer.Connect(transform, binding.observableName, () => method.Invoke(this, new [] {observer}));
                }
                observer.StartObserving();

                if (!observer.Connected)
                {
                    Debug.LogWarning($"Couldn't connect signal receiver {method.Name}. Did you add the right parameter type?");
                }

                observers.Add(observer);
            }
        }

        /// <summary>
        /// Make all connected observers start observing and dispatch an initial change event
        /// </summary>
        private void StartObserving()
        {
            observers.ForEach(observer => observer.StartObserving());
            observables.ForEach(observable => observable.DispatchChange());
        }

        /// <summary>
        /// Disconnect all observers
        /// </summary>
        private void DisconnectObservers()
        {
            observers?.ForEach(observer => { observer?.Disconnect(); });
        }

        /// <summary>
        /// Retrieves all fields implementing IObservable and tries to connect them to change listeners. 
        /// </summary>
        private void ConnectObservables()
        {
            observables = new List<IObservable>();
            observableChangeListeners = new Dictionary<IObservable, Action>();
            foreach (var field in AllFields)
            {
                if (!field.CustomAttributes.Any()) continue;

                var changeListener = field.GetCustomAttribute<ChangeListenerAttribute>();
                if (changeListener == null) continue;

                if (!typeof(IObservable).IsAssignableFrom(field.FieldType))
                {
                    Debug.LogError("ChangeListenerAttribute expects a field implementing IObservable");

                    continue;
                }

                var observable = field.GetValue(this) as IObservable;
                if (observable == null) continue;

                var methodInfo = GetType().GetMethod(changeListener.changeListener, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (methodInfo == null)
                {
                    Debug.LogError("Change listener for " + changeListener + " not found");
                    continue;
                }

                observables.Add(observable);
                observableChangeListeners[observable] = () => methodInfo.Invoke(this, null);
                observable.ValueChanged += observableChangeListeners[observable];
            }
        }

        /// <summary>
        /// Disconnects all change listeners from observables
        /// </summary>
        private void DisconnectObservables()
        {
            if (observableChangeListeners == null) return;

            foreach (var entry in observableChangeListeners)
            {
                entry.Key.ValueChanged -= entry.Value;
            }

            observableChangeListeners.Clear();
        }


        /// <summary>
        /// Destroy a child of a Transform
        /// </summary>
        public void DestroyChild(Transform t, int index)
        {
            if (t == null || index < 0 || index >= t.childCount) return;

            var child = t.GetChild(index);
            if (child == null) return;
            EditorUtils.DestroyImmediate(child.gameObject);
        }

        /// <summary>
        /// Destroy a child of the associated transform
        /// </summary>
        public void DestroyChild(int index)
        {
            DestroyChild(transform, index);
        }

        /// <summary>
        /// Destroy all children of a Transform
        /// </summary>
        public void DestroyChildren(Transform t)
        {
            for (var i = t.childCount - 1; i >= 0; i--)
            {
                var child = t.GetChild(i).gameObject;
                EditorUtils.DestroyImmediate(child);
            }
        }

        /// <summary>
        /// Destroy all children of the associated transform
        /// </summary>
        public void DestroyChildren()
        {
            DestroyChildren(transform);
        }

        /// <summary>
        /// Instantiate a prefab in edit or play mode
        /// </summary>
        /// <param name="prefab">Prefab reference</param>
        /// <param name="parent">parent transform</param>
        /// <param name="siblingIndex">target sibling index, -1 appends at the end</param>
        /// <returns></returns>
        public Object InstantiatePrefab(GameObject prefab, Transform parent, int siblingIndex = -1)
        {
            Object ret = null;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ret = PrefabUtility.InstantiatePrefab(prefab, parent);
            } else
            {
                ret = Instantiate(prefab, parent);
            }
#else
            ret = Instantiate(prefab, parent);
#endif
            if (siblingIndex > -1 && ret is GameObject gameObject)
            {
                gameObject.transform.SetSiblingIndex(siblingIndex);
            }

            return ret;
        }

        /// <summary>
        /// Instantiate a prefab in edit or play mode and add it as a child 
        /// </summary>
        /// <param name="prefab">Prefab reference</param>
        /// <param name="siblingIndex">target sibling index, -1 appends at the end</param>
        /// <returns></returns>
        public Object InstantiatePrefab(GameObject prefab, int siblingIndex = -1)
        {
            return InstantiatePrefab(prefab, transform, siblingIndex);
        }

        /// <summary>
        /// Destroy children of a transform to reduce childCount to a desired value 
        /// </summary>
        /// <param name="targetChildCount">Desired childCount value</param>
        /// <param name="t">Target container transform</param>
        /// <param name="currentChildCount">optional childCount override</param>
        public void DecreaseChildCount(int targetChildCount, Transform t, int? currentChildCount = null)
        {
            if (t == null) return;
            for (var i = currentChildCount ?? t.childCount; i > targetChildCount; i--)
            {
                DestroyChild(t, i - 1);
            }
        }

        /// <summary>
        /// Destroy children of the associated transform to reduce childCount to a desired value 
        /// </summary>
        /// <param name="targetChildCount">Desired childCount value</param>
        /// <param name="currentChildCount"></param>
        public void DecreaseChildCount(int targetChildCount, int? currentChildCount = null)
        {
            DecreaseChildCount(targetChildCount, transform, currentChildCount);
        }


        public delegate void CreateChildCallback(int index);

        /// <summary>
        /// Increase childCount of a transform to a desired value
        /// </summary>
        /// <param name="targetChildCount">Desired childCount value</param>
        /// <param name="t">Target container transform</param>
        /// <param name="callback">Callback to create child</param>
        public void IncreaseChildCount(int targetChildCount, Transform t, CreateChildCallback callback)
        {
            for (var i = t.childCount; i < targetChildCount; i++)
            {
                callback.Invoke(i);
            }
        }

        /// <summary>
        /// Increase childCount of a transform to a desired value
        /// </summary>
        /// <param name="targetChildCount">Desired childCount value</param>
        /// <param name="t">Target container transform</param>
        /// <param name="prefab">Child prefab</param>
        public void IncreaseChildCount(int targetChildCount, Transform t, GameObject prefab)
        {
            for (var i = t.childCount; i < targetChildCount; i++)
            {
                InstantiatePrefab(prefab, t);
            }
        }

        /// <summary>
        /// Destroy or create children of a transform to get a desired childCount value
        /// </summary>
        /// <param name="targetChildCount">Desired childCount value</param>
        /// <param name="t">Target container transform</param>
        /// <param name="prefab">Child prefab</param>
        public void AdjustChildCount(int targetChildCount, Transform t, GameObject prefab)
        {
            DecreaseChildCount(targetChildCount, t);
            IncreaseChildCount(targetChildCount, t, prefab);
        }

        /// <summary>
        /// Destroy or create children to get a desired childCount value
        /// </summary>
        /// <param name="targetChildCount">Desired childCount value</param>
        /// <param name="t">Target container transform</param>
        /// <param name="prefab">Child prefab</param>
        public void AdjustChildCount(int targetChildCount, GameObject prefab)
        {
            DecreaseChildCount(targetChildCount, transform);
            IncreaseChildCount(targetChildCount, transform, prefab);
        }
    }
}
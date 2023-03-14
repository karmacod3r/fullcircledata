using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullCircleData.Properties;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace FullCircleData
{
    /// <summary>
    /// This is a base class for data sources, meaning MonoBehaviours that accumulate data and make it available for their children. 
    /// </summary>
    [ExecuteAlways, DefaultExecutionOrder(-100)]
    public abstract class DataSource : MonoBehaviour
    {
        internal static bool changeBlockActive;
        internal static Queue<Action> changeDispatcherQueue = new Queue<Action>();

        /// <summary>
        /// all observable fields
        /// </summary>
        protected List<FieldInfo> observableFields;

        /// <summary>
        /// Gets all fields that implement IObservable
        /// </summary>
        /// <returns>All fields implementing IObservable as List&lt;IObservable&gt;</returns>
        private List<FieldInfo> GetObservableFields()
            => GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(field => typeof(IObservable)
                    .IsAssignableFrom(field.FieldType)).ToList();

        /// <summary>
        /// Create instances for observable fields that are null - this affects nonserialized fields
        /// </summary>
        private void InitializeObservableFields()
        {
            observableFields.ForEach(field =>
            {
                if (field.GetValue(this) != null) return;

                var instance = Activator.CreateInstance(field.FieldType);
                field.SetValue(this, instance);
            });
        }

        /// <summary>
        /// Unity event OnEnable used to initialize field cache 
        /// </summary>
        protected virtual void OnEnable()
        {
            Initialize(true);
        }

        /// <summary>
        /// Cache all fields implementing IObservable
        /// </summary>
        /// <param name="force">Force-recache fields</param>
        private void Initialize(bool force = false)
        {
            if (!force && observableFields != null) return;

            observableFields = GetObservableFields();
            InitializeObservableFields();
        }

        /// <summary>
        /// Try to get a IObservable by name and encapsulated type
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="skipTypeCheck">Skip generic argument type check</param>
        /// <typeparam name="T">The encapsulated type</typeparam>
        /// <returns>Field, or null if not found</returns>
        public Observable<T> GetObservable<T>(string name, bool skipTypeCheck = false)
        {
            Initialize();
            
            var field = observableFields.Find(field => field.Name == name);
            if (field == null || !skipTypeCheck && field.FieldType.IsGenericType
                                                && field.FieldType.GenericTypeArguments[0].UnderlyingSystemType !=
                                                typeof(T))
            {
                return null;
            }

            return (Observable<T>) field.GetValue(this);
        }

        /// <summary>
        /// Find a field by name and encapsulated type in parent transforms
        /// </summary>
        /// <param name="context">Start point for the search in parent transforms</param>
        /// <param name="fieldName">Field name of the IObservable</param>
        /// <param name="skipTypeCheck">Skip generic argument type check</param>
        /// <typeparam name="T">The encapsulated type</typeparam>
        /// <returns></returns>
        public static Observable<T> Find<T>(Transform context, string fieldName, bool skipTypeCheck = false)
        {
            if (context == null) return null;

            var source = context.GetComponentInParent<IDataSource>() as DataSource;
            if (source == null) return null;

            // try to get the observable from first Model component
            var observable = source.GetObservable<T>(fieldName, skipTypeCheck);

            // try other possible other Models on the same transform
            if (observable == null)
            {
                var dataSources = source.transform.GetComponents<IDataSource>();
                foreach (var dataSource in dataSources)
                {
                    var d = dataSource as DataSource;
                    if (d == source || d == null) continue;
                    observable = d.GetObservable<T>(fieldName, skipTypeCheck);

                    if (observable != null) break;
                }
            }

            // traverse to next parent
            if (observable == null) return Find<T>(context.parent, fieldName, skipTypeCheck);

            return observable;
        }

        /// <summary>
        /// Activates a change block: all subsequent change callback calls will be queued.
        /// Use this to ensure interdependent referencing data stays in sync when you update multiple observables.  
        /// <see cref="EndChangeBlock"/> 
        /// </summary>
        public static void BeginChangeBlock()
        {
            changeBlockActive = true;
        }

        /// <summary>
        /// Closes a change block and, fires all queued up change listeners and empties the queue.
        /// <see cref="BeginChangeBlock"/>
        /// </summary>
        public static void EndChangeBlock()
        {
            changeBlockActive = false;

            while (changeDispatcherQueue.Count > 0)
            {
                changeDispatcherQueue.Dequeue().Invoke();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            changeBlockActive = false;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EditorInitializeOnLoad()
        {
            changeBlockActive = false;
        }
#endif
    }
}
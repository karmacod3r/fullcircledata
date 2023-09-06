using System;
using FullCircleData.Extensions;
using UnityEngine;

namespace FullCircleData.Properties
{
    /// <summary>
    /// Wrapper class used to connect to Model fields implementing IObservable and to observe them  
    /// </summary>
    /// <typeparam name="T">The encapsulated type</typeparam>
    /// <see cref="BindingAttribute"/>
    /// <see cref="BestBehaviour"/>
    /// <see cref="Model"/>
    /// <see cref="Observable"/>
    public class Observer<T> : IObserver
    {
        [NonSerialized] protected Observable<T> observable;
        [NonSerialized] protected bool connected;
        [NonSerialized] protected Action changeCallback;
        private readonly T initialValue;

        public bool Connected => connected;

        public Observer()
        {
        }
        
        public Observer(T initialValue)
        {
            this.initialValue = initialValue;
        }
        

        /// <summary>
        /// Try to connect to a corresponding Model field
        /// </summary>
        /// <param name="context">The transform of the surrounding Behaviour</param>
        /// <param name="name">The name of the observable</param>
        /// <param name="changeCallback">(optional) The callback that gets invoked when the IObservable changed</param>
        public virtual void Connect(Transform context, string name, Action changeCallback = null, bool skipTypeCheck = false)
        {
            Disconnect();
            
            // try to find a IObservable in the parent tree by name and type
            observable = DataSource.Find<T>(context, name, skipTypeCheck);
            connected = observable != null;
            if (!connected)
            {
                Debug.LogWarning($"Observable {name} not found for context {context.GetPath()}");    
            }
            
            // if we cannot find a matching observable, create an empty buffer
            observable ??= new Observable<T>(initialValue);
            this.changeCallback = changeCallback;
        }

        /// <summary>
        /// Start observing the matched IObservable if connected
        /// </summary>
        public virtual void StartObserving()
        {
            if (connected && changeCallback != null)
            {
                observable.ValueChanged -= changeCallback;
                observable.ValueChanged += changeCallback;
                changeCallback();
            }
        }

        /// <summary>
        /// Stop observing by removing the event listener 
        /// </summary>
        public virtual void StopObserving()
        {
            if (connected && changeCallback != null)
            {
                observable.ValueChanged -= changeCallback;
            }
        }

        /// <summary>
        /// Stop observing and disconnect
        /// </summary>
        public void Disconnect()
        {
            StopObserving();
            connected = false;
            observable = new Observable<T>();
        }
        
        /// <summary>
        /// Get/Set the encapsulated value and propagate it, if connected 
        /// </summary>
        public virtual T Value
        {
            get => observable.Value;
            set => observable.Value = value;
        }
        

        public virtual void SetValue(T newValue, bool forceDispatch = false)
        {
            observable.SetValue(newValue, forceDispatch);
        }
        

        public virtual void DispatchChange()
        {
            observable?.DispatchChange();
        }
    }
}
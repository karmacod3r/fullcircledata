using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace FullCircleData.Properties
{
    /// <summary>
    /// Encapsulates a serialized field and makes it observable
    /// </summary>
    /// <typeparam name="T">The encapsulated type</typeparam>
    /// <see cref="Model"/>
    /// <see cref="Observer"/>
    [Serializable]
    public class Observable<T> : Observer<T>, IObservable
    {
        /// <summary>
        /// Encapsulated serialized field
        /// </summary>
        [SerializeField] protected T value;

        /// <summary>
        /// This event is invoked on value change
        /// </summary>
        public event Action ValueChanged;
        
        private Action observerChangeCallback;

        [Preserve]
        public Observable()
        {
        }
        
        [Preserve]
        public Observable(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Get/Set encapsulated value and dispatch event on change
        /// </summary>
        public override T Value
        {
            get => connected ? base.Value : value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value)) return;
                this.value = value;

                if (connected)
                {
                    base.Value = value;
                }
                else
                {
                    DispatchChange();
                }
            }
        }

        public override void SetValue(T newValue, bool forceDispatch = false)
        {
            if (connected)
            {
                base.SetValue(newValue, forceDispatch);
                return;
            }
            
            if (forceDispatch)
            {
                value = newValue;
                DispatchChange();
                return;
            }

            Value = newValue;
        }

        /// <summary>
        /// Force dispatch change event
        /// </summary>
        public override void DispatchChange()
        {
            if (DataSource.changeBlockActive)
            {
                DataSource.changeDispatcherQueue.Enqueue(DispatchChange);
                return;
            }

            if (!connected)
            {
                ValueChanged?.Invoke();
                observerChangeCallback?.Invoke();
                return;
            }
            
            base.SetValue(value, true);
        }

        public object GetObjectValue()
        {
            return Value;
        }

        public void SetObjectValue(object obj, bool forceDispatch = false)
        {
            SetValue((T) obj, forceDispatch);
        }

        /// <summary>
        /// Remove all event listeners of ValueChanged
        /// </summary>
        public void ClearObservers()
        {
            if (ValueChanged == null) return;
            
            foreach (var d in ValueChanged.GetInvocationList())
            {
                ValueChanged -= (Action) d;
            }
        }

        public override void Connect(Transform context, string name, Action changeCallback = null, bool skipTypeCheck = false)
        {
            observerChangeCallback = changeCallback;
            base.Connect(context.parent, name, ChangeCallback, skipTypeCheck);
        }

        private void ChangeCallback()
        {
            Value = observable.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullCircleData.Properties;
using UnityEngine;

namespace FullCircleData
{
    [DefaultExecutionOrder(-100)]
    public class SyncedModel : Model
    {
        private static Dictionary<Type, List<SyncedModel>> instances = new Dictionary<Type, List<SyncedModel>>();
        private static bool dispatchingChange;
        
        private Dictionary<IObservable, Action> changeListeners = new Dictionary<IObservable, Action>();

        private void RegisterInstance()
        {
            if (!instances.ContainsKey(GetType()))
            {
                instances.Add(GetType(), new List<SyncedModel>());
            }

            var instancesList = instances[GetType()];
            if (instancesList.Contains(this)) return;

            instancesList.Add(this);
            CleanupInstances();

            if (instancesList.Count > 1)
            {
                instancesList[0].DispatchAllTo(this);   
            }
        }

        private void UnregisterInstance()
        {
            if (!instances.ContainsKey(GetType())) return;

            instances[GetType()].Remove(this);
            CleanupInstances();
        }

        private void CleanupInstances()
        {
            if (!instances.ContainsKey(GetType())) return;
            instances[GetType()] = instances[GetType()].Where(instance => instance != null).ToList();
        }

        internal void DispatchAllTo(SyncedModel targetInstance)
        {
            observableFields.ForEach(field =>
            {
                DispatchObservableChange(field, (IObservable) field.GetValue(this), targetInstance);
            });
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            RegisterInstance();
            StartObserving();
        }

        protected virtual void OnDisable()
        {
            UnregisterInstance();
            StopObserving();
        }

        private void StartObserving()
        {
            StopObserving();
            
            observableFields.ForEach(field =>
            {
                var observable = (IObservable) field.GetValue(this);

                void ChangeListener()
                {
                    ObservableChanged(field, observable);
                }

                changeListeners.Add(observable, ChangeListener);
                observable.ValueChanged += ChangeListener;
            });
        }

        private void ObservableChanged(FieldInfo field, IObservable observable)
        {
            if (dispatchingChange) return; 
            
            dispatchingChange = true;
            instances[GetType()].ForEach(instance => DispatchObservableChange(field, observable, instance));
            dispatchingChange = false;
        }

        private void DispatchObservableChange(FieldInfo field, IObservable observable, SyncedModel targetInstance)
        {
            if (targetInstance == this) return;

            var targetField = targetInstance.observableFields.Find(o => o.Name == field.Name);
            var targetObservable = (IObservable) targetField.GetValue(targetInstance);
            targetObservable.SetObjectValue(observable.GetObjectValue(), true);

            EditorUtils.SetDirty(targetInstance);
        }
        
        private void StopObserving()
        {
            foreach (var changeListener in changeListeners)
            {
                changeListener.Key.ValueChanged -= changeListener.Value;
            }
            
            changeListeners.Clear();
        }
    }
}
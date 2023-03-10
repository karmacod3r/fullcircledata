using FullCircleData.Attributes;
using FullCircleData.Properties;
using Unity.Collections;
using UnityEngine;

namespace FullCircleData.Examples
{
    public class ControllerWithObservablesDemo : BestBehaviour, IDataSource
    {
        [SerializeField, ChangeListener(nameof(OnValueChanged))] 
        private Observable<string> message;

        private void OnValueChanged()
        {
            Debug.Log("OnValueChanged: " + message.Value);
        }
    }
}
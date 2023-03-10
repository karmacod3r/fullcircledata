using FullCircleData.Attributes;
using FullCircleData.Properties;
using Unity.Collections;
using UnityEngine;

namespace FullCircleData.Examples
{
    public class ControllerDemo : BestBehaviour
    {
        [Binding(nameof(ModelDemo.message), nameof(OnValueChanged))] 
        private Observer<string> message;

        [SerializeField, ReadOnly] private string messageValue;

        private void OnValueChanged()
        {
            Debug.Log("OnValueChanged: " + message.Value);
            messageValue = message.Value;
        }
    }
}
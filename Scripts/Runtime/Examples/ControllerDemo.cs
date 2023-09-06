using FullCircleData.Attributes;
using FullCircleData.Properties;
using Unity.Collections;
using UnityEngine;

namespace FullCircleData.Examples
{
    public class ControllerDemo : BestBehaviour
    {
        [Binding(nameof(ModelDemo.message), nameof(OnValueChanged))] 
        public Observable<string> message;

        private void OnValueChanged()
        {
            Debug.Log("OnValueChanged: " + message.Value);
        }
    }
}
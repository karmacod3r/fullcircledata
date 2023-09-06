using FullCircleData.Attributes;
using FullCircleData.Properties;
using UnityEngine;

namespace FullCircleData.Examples
{
    public class SignalReceiverDemo: BestBehaviour
    {
        [Receive(nameof(ControllerWithObservablesDemo.signal))]
        private void Signal(SignalObserver<string> signal)
        {
            Debug.Log(signal.Value);
        }
    }
}
using FullCircleData.Attributes;
using FullCircleData.Properties;
using UnityEngine;

namespace FullCircleData.Examples
{
    public class DataProxyDemo : Model
    {
        [Binding(nameof(ModelDemo.message), nameof(MessageChanged))]
        public Observable<string> message;

        private void MessageChanged()
        {
            Debug.Log("Message changed");
        }
    }
}
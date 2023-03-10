using System;
using UnityEngine;

namespace FullCircleData.Properties
{
    public interface IObserver
    {
        bool Connected { get; }
        void Connect(Transform context, string name, Action changeCallback = null, bool skipTypeCheck = false);
        void StartObserving();
        void StopObserving();
        void Disconnect();
    }
}
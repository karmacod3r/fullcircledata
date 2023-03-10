using System;

namespace FullCircleData.Properties
{
    public interface IObservable : IObserver
    {
        event Action ValueChanged;
        void DispatchChange();
        object GetObjectValue();
        void SetObjectValue(object obj, bool forceDispatch = false);
    }
}
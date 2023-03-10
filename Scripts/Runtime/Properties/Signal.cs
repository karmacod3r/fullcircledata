using System;

namespace FullCircleData.Properties
{
    [Serializable]
    public class Signal : Observable<bool>
    {
        public void Send()
        {
            DispatchChange();
        }
    }
    
    [Serializable]
    public class Signal<T> : Observable<T>
    {
        public void Send(T value)
        {
            SetValue(value, true);
        }
    }
}
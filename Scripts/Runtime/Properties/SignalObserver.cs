namespace FullCircleData.Properties
{
    public class SignalObserver : Observer<bool>
    {
        public void Send()
        {
            (observable as Signal)?.Send();
        }

        public override void StartObserving()
        {
            if (!connected) return;

            if (changeCallback != null)
            {
                observable.ValueChanged -= changeCallback;
                observable.ValueChanged += changeCallback;
            }
        }
    }
    
    public class SignalObserver<T> : Observer<T>
    {
        public void Send(T value)
        {
            (observable as Signal<T>)?.Send(value);
        }

        public override void StartObserving()
        {
            if (!connected) return;

            if (changeCallback != null)
            {
                observable.ValueChanged -= changeCallback;
                observable.ValueChanged += changeCallback;
            }
        }
    }
}
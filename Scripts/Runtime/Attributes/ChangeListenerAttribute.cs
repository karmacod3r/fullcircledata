using System;
using UnityEngine.Scripting;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields implementing IObservable
    /// </summary>
    /// <see cref="BestBehaviour"/>
    /// <see cref="Model"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class ChangeListenerAttribute : PreserveAttribute
    {
        /// <summary>
        /// The method name of the callback in the surrounding class. Can be public or private.
        /// Hint: use nameof to make it refactoring safe
        /// </summary>
        public string changeListener;
        
        public ChangeListenerAttribute(string changeListener)
        {
            this.changeListener = changeListener;
        }

        public override string ToString()
        {
            return "ChangeListener (\"" + changeListener + "\")";
        }
    }
}
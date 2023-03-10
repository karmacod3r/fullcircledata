using System;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate fields implementing IObserver to auto-connect them to Model fields
    /// </summary>
    /// <see cref="BestBehaviour"/>
    /// <see cref="Model"/>
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : Attribute
    {
        /// <summary>
        /// The field name of the fields you want to observe. Hint: use nameof to make the binding refactoring safe 
        /// </summary>
        public string observableName;
        /// <summary>
        /// The method name of the callback in the surrounding class. Can be public or private.
        /// Hint: use nameof to make it refactoring safe
        /// </summary>
        public string changeListener;
        
        public BindingAttribute(string observableName, string changeListener = "")
        {
            this.observableName = observableName;
            this.changeListener = changeListener;
        }
        
        public override string ToString()
        {
            return "Binding (\"" + observableName + "\", \"" + changeListener + "\")";
        }
    }
}
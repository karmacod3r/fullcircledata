using System;
using UnityEngine.Scripting;

namespace FullCircleData.Attributes
{
    /// <summary>
    /// Use inside BetterBehaviour to decorate methods to receive signals
    /// </summary>
    /// <see cref="BestBehaviour"/>
    /// <see cref="Model"/>
    /// <see cref="Signal"/>
    [AttributeUsage(AttributeTargets.Method)]
    public class ReceiveAttribute : PreserveAttribute
    {
        /// <summary>
        /// The field name of the fields you want to observe. Hint: use nameof to make the binding refactoring safe 
        /// </summary>
        public string observableName;
        
        public ReceiveAttribute(string observableName, string changeListener = "")
        {
            this.observableName = observableName;
        }
    }
}
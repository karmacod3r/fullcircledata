using UnityEngine;

namespace FullCircleData
{
    /// <summary>
    /// This is a base class for models, meaning MonoBehaviours that accumulate data and make it available for their children. 
    /// </summary>
    [ExecuteAlways, DefaultExecutionOrder(-100)]
    public abstract class Model : BestBehaviour, IDataSource
    {

    }
}
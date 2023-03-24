using UnityEngine;

namespace Crosstales.BWF.Manager
{
    /// <summary>Base class for all managers.</summary>
    [ExecuteInEditMode]
    public abstract class BaseManager : MonoBehaviour
    {

        #region Variables

        [Header("Behaviour Settings")]
        /// <summary>Don't destroy gameobject during scene switches (default: true).</summary>
        [Tooltip("Don't destroy gameobject during scene switches (default: true).")]
        public bool DontDestroy = true;

        #endregion
    }
}
// © 2015-2019 crosstales LLC (https://www.crosstales.com)
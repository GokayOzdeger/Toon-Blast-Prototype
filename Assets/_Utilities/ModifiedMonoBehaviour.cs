using UnityEngine;

namespace Utilities
{
    public class ModifiedMonoBehaviour : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Allow debug
        /// </summary>
        [SerializeField] protected bool debug = false;

        #endregion

        protected virtual void Log(string msg)
        {
            if (!debug) return;
            Debug.Log($"[{GetType().Name}]: {msg}");
        }

        protected virtual void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning($"[{GetType().Name}]: {msg}");
        }
    }
}
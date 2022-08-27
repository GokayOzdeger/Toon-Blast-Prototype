using UnityEngine;

namespace Utilities
{
    public class PoolObject : MonoBehaviour
    {
        #region Variables
        [SerializeField] private bool poolAfterDelay;
        [SerializeField] private bool resetScaleAfterPool;
        [SerializeField] private float delay = 0;

        protected Transform parent;
        protected Vector3? originalScale = null;

        public Transform Parent
        {
            get => parent;
            set => parent = value;
        }
        
        #endregion

        #region Public Methods
        
        public virtual void GoToPool(float delay)
        {
            if (delay == 0) GoToPool();
            else Invoke(nameof(GoToPool), delay);
        }

        public virtual void GoToPool()
        {
            if (resetScaleAfterPool) transform.localScale = originalScale ?? Vector3.one;
            transform.SetParent(parent);
            gameObject.SetActive(false);

            IPoolable[] components = GetComponents<IPoolable>();
            foreach(IPoolable poolable in components)
            {
                poolable.OnGoToPool();
            }
        }

        public virtual void PoolSpawn()
        {
            if (originalScale == null) originalScale = transform.localScale;
            IPoolable[] components = GetComponents<IPoolable>();
            foreach (IPoolable poolable in components)
            {
                poolable.OnPoolSpawn();
            }

            if (poolAfterDelay) { Invoke(nameof(GoToPool), delay); }
        }

        public void ManuelPoolAfterDelay(float delay)
        {
            CancelInvoke();
            Invoke(nameof(GoToPool), delay);
        }
        
        #endregion
    }
}
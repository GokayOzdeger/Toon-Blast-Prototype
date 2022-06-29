using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public sealed class ObjectPooler : AutoSingleton<ObjectPooler>
    {
        #region Variables

        private PoolProfile poolProfile;
        public List<Pool> extraPools = new List<Pool>();

        [ShowOnly] public List<Pool> allPools = new List<Pool>();

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            GetPoolProfile();
            CollectPools();
            InstansiatePoolObjects(allPools);
        }
        private void GetPoolProfile()
        {
            poolProfile = Resources.Load<PoolProfile>("ScriptableObjects/Pools/PoolProfiles/" + SceneManager.GetActiveScene().name);
            if (!poolProfile)
            {
                Debug.Log("Scene doesnt have a PoolProfile Using FallbackPoolProfile");
                poolProfile = Resources.Load<PoolProfile>("ScriptableObjects/Pools/PoolProfiles/" + "FallbackPoolProfile");
            }
        }
        private void CollectPools()
        {
            List<PoolPack> poolPacks = poolProfile.poolPacks;
            foreach (PoolPack poolpack in poolPacks)
            {
                List<Pool> pools = poolpack.pools;
                foreach(Pool pool in pools)
                {
                    allPools.Add(Pool.CopyOf(pool));
                }
            }
            foreach (Pool pool in extraPools)
            {
                allPools.Add(Pool.CopyOf(pool));
            }
        }

        private void InstansiatePoolObjects(List<Pool> pools)
        {
            foreach (var pool in pools)
            {
                GameObject poolParent = new GameObject { name = pool.Prefab.name };
                poolParent.transform.parent = transform;
                pool.StartingParent = poolParent.transform;

                for (var i = 0; i < pool.StartingQuantity; i++)
                {
                    var obj = Instantiate(pool.Prefab, Vector3.zero, Quaternion.identity, poolParent.transform);
                    obj.GetComponent<PoolObject>().Parent = poolParent.transform;
                    obj.name = obj.name.Substring(0,obj.name.Length - 7) + " " + i;
                    pool.PooledObjects.Add(obj);
                    obj.SetActive(false);
                }
            }
        }

        #endregion

        #region Public Methods
        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation = new Quaternion(),
            Transform parentTransform = null)
        { 
            // Find the pool that matches the pool name:
            var pool = 0;
            for (var i = 0; i < allPools.Count; i++)
            {
                if (allPools[i].Prefab.name == poolName)
                {
                    pool = i;
                    break;
                }

                if (i == allPools.Count - 1)
                {
                    Debug.LogError("There's no pool named \"" + poolName +
                                   "\"! Check the spelling or add a new pool with that name.");
                    return null;
                }
            }


            foreach (var poolObj in allPools[pool].PooledObjects.Where(poolObj => !poolObj.activeSelf))
            {
                // Set active:
                poolObj.SetActive(true);
                poolObj.transform.localPosition = position;
                poolObj.transform.localRotation = rotation;
                // Set parent:
                if (parentTransform)
                {
                    poolObj.transform.SetParent(parentTransform, false);
                }

                poolObj.GetComponent<PoolObject>().PoolSpawn();

                return poolObj;
            }

            // If there's no game object available then expand the list by creating a new one:
            GameObject obj = Instantiate(allPools[pool].Prefab, position, rotation, allPools[pool].StartingParent);
            obj.name = obj.name.Substring(0, obj.name.Length - 7) + " " + allPools[pool].StartingParent.childCount;
            obj.GetComponent<PoolObject>().Parent = allPools[pool].StartingParent;
            allPools[pool].PooledObjects.Add(obj);

            obj.GetComponent<PoolObject>().PoolSpawn();
            return obj;
        }
        
        #endregion
    }
}
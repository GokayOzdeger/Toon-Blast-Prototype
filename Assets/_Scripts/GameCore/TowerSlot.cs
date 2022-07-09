using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class TowerSlot : MonoBehaviour, IPoolable
{
    public PoolObject PoolObject 
    { 
        get 
        {
            if (!_poolObject) _poolObject = GetComponent<PoolObject>();
            return _poolObject;
        } 
    }

    public TowerSpawnController TowerSpawnController
    {
        get
        {
            if (!_towerSpawnController) _towerSpawnController = GameManager.Instance.CurrentLevel.GetBehaviour<TowerSpawnController>();
            return _towerSpawnController;
        }
    }

    private PoolObject _poolObject;
    private TowerSpawnController _towerSpawnController;
    

    public void OnGoToPool()
    {
        TowerSpawnController.UnRegisterTowerSlot(this);
    }

    public void OnPoolSpawn()
    {
        TowerSpawnController.RegisterTowerSlot(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/TowerSpawnController")]
public class TowerSpawnController : ALevelBehaviourSO
{
    public List<BehaviourController> AllTowers { get; private set; } = new List<BehaviourController>();
    
    public List<TowerSlot> AllTowerSlots { get; private set; } = new List<TowerSlot>();

    public override void OnSetup()
    {
        Button spawnTowerButton = SceneReferences.TowerSpawnControllerReferences.CreateTowerButton;
        spawnTowerButton.onClick.RemoveAllListeners();
        spawnTowerButton.onClick.AddListener(SpawnTurretAtRandomSlot);    
    }

    public override void OnTick(float deltaTime)
    {
        //
    }

    public void RegisterTower(BehaviourController controller)
    {
        AllTowers.Add(controller);
    }
    public void UnRegisterTower(BehaviourController controller)
    {
        AllTowers.Add(controller);
    }

    public void RegisterTowerSlot(TowerSlot slot)
    {
        AllTowerSlots.Add(slot);
    }

    public void UnRegisterTowerSlot(TowerSlot slot)
    {
        AllTowerSlots.Add(slot);  
    }

    public void SpawnTurretAtRandomSlot()
    {
        if (AllTowerSlots.Count == 0) return;
        int randomTowerSlotIndex = Random.Range(0, AllTowerSlots.Count);
        TowerSlot randomSlot = AllTowerSlots[randomTowerSlotIndex];
        GameObject randomTower = Config.TowerSpawnerSettings.Towers[Random.Range(0, Config.TowerSpawnerSettings.Towers.Length)];
        ObjectPooler.Instance.Spawn(randomTower.name, randomSlot.transform.position);
        AllTowerSlots.RemoveAt(randomTowerSlotIndex);
    }

    [System.Serializable]
    public class TowerSpawnControllerSettings
    {
        public GameObject[] Towers;
    }

    [System.Serializable]
    public class TowerSpawnControllerSceneReferences
    {
        public Button CreateTowerButton;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableLevelBehaviours/ EnemySpawnOnPathBehaviour")]
public class EnemySpawnOnPathBehaviour : ALevelBehaviourSO
{
    private EnemySpawnControllerSettings Settings { get; set; }

    private float _nextSpawnSeconds;
    private float _timePassedSinceStart = 0;
    private float _totalSpawnChance;

    public override void OnSetup()
    {
        Settings = Config.EnemySpawnControllerSettings;
        CalculateSpawnChance();
    }

    public override void OnTick(float deltaTime)
    {
        _timePassedSinceStart += deltaTime;
        if (_timePassedSinceStart >= _nextSpawnSeconds)
        {
            float spawnRateRampUpPercentage = _timePassedSinceStart / Settings.spawnRateMaxOutDuration;
            float currentEnemyPerSeconds = Settings.enemySpawnPerSecondsMin + ((Settings.enemySpawnPerSecondsMax - Settings.enemySpawnPerSecondsMin) * spawnRateRampUpPercentage);
            _nextSpawnSeconds = _timePassedSinceStart + (1 / currentEnemyPerSeconds);
            SpawnRandomEnemyOnPath();
        }
    }

    private void SpawnRandomEnemyOnPath()
    {
        PathNode chosenStartNode = PathManager.Instance.GetRandomStartNode();
        GameObject enemy = ObjectPooler.Instance.Spawn(GetRandomEnemy().name, chosenStartNode.transform.position);
        PathMovementBehaviour pathMovementBehaviour = (PathMovementBehaviour) enemy.GetComponent<BehaviourController>().MovementBehaviour;
        pathMovementBehaviour.SetNode(chosenStartNode);
    }

    private void CalculateSpawnChance()
    {
        foreach (var enemyWithSpawnChance in Settings.enemies) _totalSpawnChance += enemyWithSpawnChance.spawnChance;   
    }

    private GameObject GetRandomEnemy()
    {
        float randomSpawnChance = Random.Range(0, _totalSpawnChance);
        float currentSpawnChance = 0;
        
        foreach (var enemyWithSpawnChance in Settings.enemies)
        {
            currentSpawnChance += enemyWithSpawnChance.spawnChance;
            if (randomSpawnChance <= currentSpawnChance) return enemyWithSpawnChance.enemyPrefab;
        }

        Debug.LogError("Spawn Chance Error");
        return null;
    }


    [System.Serializable]
    public class EnemySpawnControllerSettings
    {
        public EnemyWithSpawnChance[] enemies;
        public float enemySpawnPerSecondsMin;
        public float enemySpawnPerSecondsMax;
        
        // how much it takes to reach maximum spawn speed
        public float spawnRateMaxOutDuration;

        [System.Serializable]
        public struct EnemyWithSpawnChance
        {
            public GameObject enemyPrefab;
            public float spawnChance;
        }
    }
}

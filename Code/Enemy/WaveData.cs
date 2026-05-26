using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Combat/Waves/WaveData")]
public class WaveData : ScriptableObject
{

    [Range(0f, 10f)] public float spawnIntervalMin;
    [Range(0f, 10f)] public float spawnIntervalMax;

    [System.Serializable]
    public class EnemySpawnChance
    {
        public GameObject enemy;
        [Range(1, 100)]public int spawnChance;
        [Range(1, 100)]public int limit;
    }

    public EnemySpawnChance[] enemies;

    [HideInInspector]
    public int maxChanceValue;

    [HideInInspector]
    public int maxEnemiesValue;

    Dictionary<EnemySpawnChance, int> enemyLimit = new Dictionary<EnemySpawnChance, int>();

    [ContextMenu("Apply Max Value")]
    public void ApplyMax()
    {
        maxChanceValue = 0;

        foreach (var enemy in enemies)
        {
            maxEnemiesValue += enemy.limit;
            maxChanceValue += enemy.spawnChance;
            enemyLimit.Add(enemy, enemy.limit);
        }
    }

    public GameObject GetRandomEnemy()
    {
        int randomValue = Random.Range(0, maxChanceValue);
        int currentSum = 0;

        foreach (var enemy in enemies)
        {
            currentSum += enemy.spawnChance;
            if (randomValue < currentSum || enemyLimit[enemy] > 0)
            {
                enemyLimit[enemy] -= 1;
                return enemy.enemy;
            }
        }
        return enemies[enemies.Length - 1].enemy;
    }

    public float GetRandomSpawnInterval()
    {
        return (int)Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

}

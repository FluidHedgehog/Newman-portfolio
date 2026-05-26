using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawnerEvents
{
    public static System.Action onEnemyDead;

    public static void TriggerEnemyDead() => onEnemyDead?.Invoke();
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] 
    WaveData waveData;

    [SerializeField] 
    WaveMusicManager musicManager;

    [SerializeField]
    Transform[] transforms;

    List<GameObject> spawnedEnemies = new List<GameObject>();
    int spawnedEnemiesCount = 0; 

    public void StartSpawning()
    {
        if (musicManager == null)
            musicManager = FindFirstObjectByType<WaveMusicManager>();
        
        if (musicManager != null) 
            musicManager.FadeInMusic();

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        waveData.ApplyMax();

        for (int i = 0; i < waveData.maxEnemiesValue; i++)
        {
            var enemyInstance = Instantiate(waveData.GetRandomEnemy(), GetRandomPoint(), Quaternion.identity);

            yield return new WaitForSeconds(waveData.GetRandomSpawnInterval());
        }   
    }

    void OnEnable()
    {
        spawnedEnemies.Clear();
        spawnedEnemiesCount = 0;
        EnemySpawnerEvents.onEnemyDead += ClearEnemies;
    }

    void OnDisable()
    {
        EnemySpawnerEvents.onEnemyDead -= ClearEnemies;
    }

    void ClearEnemies()
    {
        spawnedEnemiesCount++;
        spawnedEnemies.RemoveAll(x => x == null);

        if (spawnedEnemiesCount == waveData.maxEnemiesValue)
        {
            if (musicManager != null) musicManager.FadeOutMusic();
            
            StateMachineEvents.TriggerStateChange(States.Platform);
        }
    }

    Vector2 GetRandomPoint()
    {
        return transforms[Random.Range(0, transforms.Length)].position;
    }
}
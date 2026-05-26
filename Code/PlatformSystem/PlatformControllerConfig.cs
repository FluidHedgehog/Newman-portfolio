using UnityEngine;

[CreateAssetMenu(fileName = "PlatformControllerConfig", menuName = "Platforms/PlatformControllerConfig")]
public class PlatformControllerConfig : ScriptableObject
{
    [System.Serializable]
    public class PlatformSpawnChance
    {
        public GameObject[] platforms;

        [Range(1, 100)] public int spawnChance;
        [Range(1, 100)] public int limit;
    }

    public PlatformSpawnChance[] platformPools;

    public GameObject GetRandomPlatform(PlatformSpawnChance platformPoolIndex)
    {
        return platformPoolIndex.platforms[Random.Range(0, platformPoolIndex.platforms.Length)];
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [System.Serializable]
    public class PlatformPool
    {
        public Queue<GameObject> platforms = new Queue<GameObject>();
        public int spawnChance;
    }

    List<PlatformPool> platformPools = new List<PlatformPool>();

    [SerializeField] GameObject[] startPoints;

    [SerializeField] int startPointsQueueSize;

    Queue<GameObject> startPointsQueue = new Queue<GameObject>();

    //Helper variables
    int[] previous;
    int maxChanceValue;

    [SerializeField] PlatformControllerConfig[] configs;
    int currentConfig = 0;

    void Start()
    {
        previous = new int[2] { 0, 1 };

        BuildRoadQueue();
        BuildPlatformPools();
        ApplyMax();
    }

    void OnStart() => CreatePlatform();

    void OnEnable()
    {
        PlatformEvents.PlatformDown += ReassignPlatform;
        PlatformEvents.CreatePlatform += CreatePlatform;
        PlatformEvents.End += EndLevel;
        PlatformEvents.Start += OnStart;
    }

    void OnDisable()
    {
        PlatformEvents.PlatformDown -= ReassignPlatform;
        PlatformEvents.CreatePlatform -= CreatePlatform;
        PlatformEvents.End -= EndLevel;
        PlatformEvents.Start -= OnStart;
    }

    void BuildRoadQueue()
    {
        for (int i = 0; i < startPointsQueueSize; i++)
        {
            int random;

            do
            {
                random = Random.Range(0, startPoints.Length);
            }
            while (previous.Contains(random));

            previous[0] = previous[1];
            previous[1] = random;

            startPointsQueue.Enqueue(startPoints[random]);
        }
    }

    void BuildPlatformPools()
    {
        for (int i = 0; i < configs[currentConfig].platformPools.Length; i++)
        {
            var currentPlatformPool = configs[currentConfig].platformPools[i];
            PlatformPool newPlatformPool = new PlatformPool();

            for (int j = 0; j < currentPlatformPool.limit; j++)
            {
                var obj = Instantiate(configs[currentConfig].GetRandomPlatform(currentPlatformPool), startPoints[0].transform);
                obj.GetComponentInChildren<PlatformInstance>().index = i;
                newPlatformPool.platforms.Enqueue(obj);
            }
            platformPools.Add(newPlatformPool);
            platformPools[i].spawnChance = currentPlatformPool.spawnChance;
        }
    }

    void ApplyMax()
    {
        maxChanceValue = 0;

        for (int i = 0; i < configs[currentConfig].platformPools.Length; i++)
            maxChanceValue += configs[currentConfig].platformPools[i].spawnChance;
        
    }

    void CreatePlatform()
    {
        var platformPool = GetPlatformPool();

        if (platformPool.platforms.Count == 0)
        {
            if (platformPools[0].platforms.Count <= 0)
            {
                Debug.LogError("<color=red> Platform System: </color> Platform creation broken! Change default platform limit size!");
                return;
            }
            Debug.LogWarning("<color=red> Platform System: </color> Platfrorm creation customized, nothing's wrong if it shows occasionally!");
            platformPool = platformPools[0];
        }

        GameObject platform = platformPool.platforms.Dequeue();

        var startPoint = startPointsQueue.Dequeue();
        startPointsQueue.Enqueue(startPoint);

        platform.transform.SetParent(null);
        platform.transform.position = startPoint.transform.position;
        platform.SetActive(true);
    }

    PlatformPool GetPlatformPool()
    {
        int randomPlatformIndex = Random.Range(0, maxChanceValue);
        int currentSum = 0;

        foreach (var platformPool in platformPools)
        {
            currentSum += platformPool.spawnChance;
            if (randomPlatformIndex < currentSum)
                return platformPool;
            
        }
        return platformPools[0];
    }

    void ReassignPlatform(int index, GameObject platform) => platformPools[index].platforms.Enqueue(platform);

    void EndLevel() => gameObject.SetActive(false);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<Wave> waves;
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    public float rate;
    public GameObject enemiesParent;

    void Start()
    {
        StartCoroutine(SpawnLevel());
    }

    IEnumerator SpawnLevel()
    {
        foreach (Wave wave in waves)
        {
            foreach (EnemyGroup group in wave.enemyGroups)
            {
                yield return StartCoroutine(SpawnGroup(group));
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnGroup(EnemyGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            Instantiate(group.enemyPrefab, spawnPoint.position, spawnPoint.rotation, enemiesParent.transform);
            yield return new WaitForSeconds(rate);
        }
    }
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
}

[System.Serializable]
public class Wave
{
    public List<EnemyGroup> enemyGroups; 
}
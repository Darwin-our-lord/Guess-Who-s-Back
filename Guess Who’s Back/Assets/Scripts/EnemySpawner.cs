
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    public int wave = 1;
    public bool waveOngoing = false;
    public int waveValueTotal = 1;

    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    public float rate = 1f;

    public GameObject nextWavebutton;
    public GameObject enemiesParent;
    public RoadMaker roadMaker;
    public StoreManager StoreManager;

    public void StartWave()
    {
        if (!waveOngoing) StartCoroutine(SpawnWave());
        nextWavebutton.SetActive(false);
    }
    IEnumerator SpawnWave()
    {
        waveOngoing = true;
        StoreManager.AddMoney(wave*10);
        StoreManager.RerollStore();

        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            roadMaker.ExtendRoad();
        }

        foreach (Enemy enemy in enemiesParent.transform.GetComponentsInChildren<Enemy>(true))
        {
            enemy.Respawn();
        }


        int waveValue = 0;
        int enemiesSpawned = 0;

        while(waveValue < waveValueTotal && enemiesSpawned < 5)
        {
            GameObject highestValueEnemy = null;
            for (int i = 0; i < 3; i++)
            {
                GameObject enemiesChosen = enemies[UnityEngine.Random.Range(0, enemies.Count)];
                if (highestValueEnemy == null)
                    highestValueEnemy = enemiesChosen;
                else if (enemiesChosen.GetComponent<Enemy>().WaveValue > highestValueEnemy.GetComponent<Enemy>().WaveValue)
                    highestValueEnemy = enemiesChosen;
            }
                

            if (highestValueEnemy.GetComponent<Enemy>().WaveValue + waveValue <= waveValueTotal)
            {
                waveValue += highestValueEnemy.GetComponent<Enemy>().WaveValue;
                enemiesSpawned++;
                Instantiate(highestValueEnemy, spawnPoint.position, spawnPoint.rotation, enemiesParent.transform);
                yield return new WaitForSeconds(rate);
            }
        }

        int waveMod = (int)math.floor(wave / 10f);
        waveValueTotal+=waveMod+1;

        StartCoroutine(CheckForEnemies());

    }

    IEnumerator CheckForEnemies()
    {
        while (waveOngoing)
        {
            if (enemiesParent.transform.Cast<Transform>().All(t => !t.gameObject.activeSelf))
            {
                waveOngoing = false;
                StoreManager.AddMoney(0);
                wave++;
                nextWavebutton.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }

}

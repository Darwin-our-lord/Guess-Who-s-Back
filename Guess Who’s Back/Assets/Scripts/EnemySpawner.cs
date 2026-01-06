
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    public int wave = 1;
    public int waveValueTotal = 1;
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    public float rate = 1f;
    public GameObject enemiesParent;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (true)//temp while loop
        {
            
            foreach (Enemy enemy in enemiesParent.transform.GetComponentsInChildren<Enemy>(true))
            {
                enemy.Respawn();
                yield return new WaitForSeconds(rate/2);
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
            wave++;

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

}

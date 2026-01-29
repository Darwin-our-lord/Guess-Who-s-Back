
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct Wave
{
    public int waveNr;
    public List<EnemyGroup> enemyGroups;
    public bool respawnEnemies;
    public int roadsToCreate;
    public float newSpawnRate;
}

[System.Serializable]
public struct EnemyGroup
{
    public GameObject enemy;
    public int amount;
}

public class EnemySpawner : MonoBehaviour
{
    public List<Wave> specialWaves = new List<Wave>();

    public List<GameObject> enemies = new List<GameObject>();
    public int wave = 1;
    public bool waveOngoing = false;
    public int waveValueTotal = 1;
    public TMP_Text currentWaveText;

    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    public float rate = 1f;

    public GameObject nextWavebutton;
    public GameObject storebutton;
    public GameObject enemiesParent;
    public GameObject towersParent;

    public RoadMaker roadMaker;
    public StoreManager StoreManager;
    public MenuManager menuManager;

    public void StartWave()
    {
        foreach (Tower tower in towersParent.transform.GetComponentsInChildren<Tower>())
        {
            tower.ResetFireRateTimer();
        }
        nextWavebutton.SetActive(false);
        storebutton.SetActive(false);

        bool special = false;
        Wave specialWave= specialWaves[0];//it doesnt acutally use the first one in the list, just needed it to stop complaining
        for (int i = 0; i < specialWaves.Count; i++)
        {
            if(wave == specialWaves[i].waveNr)
            {
                special = true;
                specialWave = specialWaves[i];
            }
        }
        if (!waveOngoing && !special) StartCoroutine(SpawnWave());
        if (!waveOngoing && special) StartCoroutine(SpawnWaveSpecial(specialWave));

        if (menuManager.storeUI.activeSelf) menuManager.StoreButton();


    }
    IEnumerator SpawnWaveSpecial(Wave specialWave)
    {
        waveOngoing = true;
        StoreManager.AddMoney((wave * 10) / 2 + 10);
        StoreManager.RerollStore();

        for (int i = 0; i < specialWave.roadsToCreate; i++)
        {
            yield return new WaitForSeconds(0.3f);
            roadMaker.ExtendRoad();
        }
        if (specialWave.respawnEnemies)
        {
            foreach (Enemy enemy in enemiesParent.transform.GetComponentsInChildren<Enemy>(true))
            {
                enemy.Respawn();
            }
        }

        foreach (EnemyGroup group in specialWave.enemyGroups)
        {
            for(int i = 0; i < group.amount; i++)
            {
                Instantiate(group.enemy, spawnPoint.position, spawnPoint.rotation, enemiesParent.transform);
                if(specialWave.newSpawnRate > 0)yield return new WaitForSeconds(specialWave.newSpawnRate);
                else yield return new WaitForSeconds(rate);
            }
        }

        int waveMod = (int)math.floor(wave / 10f);
        waveValueTotal += waveMod + 1;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckForEnemies());
    }
    IEnumerator SpawnWave()
    {
        waveOngoing = true;
        StoreManager.AddMoney((wave*10)/2+10);
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

        while(waveValue < waveValueTotal && enemiesSpawned < 30)
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
            
            if(highestValueEnemy.GetComponent<Enemy>().waveReq > wave) continue;
            
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

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckForEnemies());

    }

    IEnumerator CheckForEnemies()
    {
        while (waveOngoing)
        {
            yield return new WaitForSeconds(rate);
            if (enemiesParent.transform.Cast<Transform>().All(t => !t.gameObject.activeSelf))
            {
                waveOngoing = false;
                StoreManager.AddMoney(0);
                wave++;
                nextWavebutton.SetActive(true);
                storebutton.SetActive(true);
                currentWaveText.text = "Next wave: " + wave.ToString();
            }
            foreach (Enemy enemy in enemiesParent.transform.GetComponentsInChildren<Enemy>())
            {
                if(enemy.walkType == WalkType.flying) enemy.UpdateFlyingTarget();
            }
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }

}

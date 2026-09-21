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
    [Header("")]
    public int startPosMoveNr;
    public float newStartMoveRate;
    [Header("")]
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
    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] float rate = 1f;
    [Header("Objects")]
    [SerializeField] GameObject nextWavebutton;
    [SerializeField] GameObject storebutton;
    [SerializeField] GameObject moneyText;
    [SerializeField] GameObject yetToBeAddedMoneyText;
    [SerializeField] GameObject enemiesParent;
    [SerializeField] GameObject towersParent;
    [Header("Scripts")]
    [SerializeField] RoadMaker roadMaker;
    [SerializeField] StoreManager StoreManager;
    [SerializeField] MenuManager menuManager;

    private int waveGeneration = 0;

    public void StartWave()
    {
        foreach (Tower tower in towersParent.transform.GetComponentsInChildren<Tower>())
        {
            tower.ResetFireRateTimer();
        }
        nextWavebutton.SetActive(false);
        storebutton.SetActive(false);
        moneyText.SetActive(false);
        yetToBeAddedMoneyText.SetActive(false);

        bool special = false;
        Wave specialWave = specialWaves[0];
        for (int i = 0; i < specialWaves.Count; i++)
        {
            if (wave == specialWaves[i].waveNr)
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
        waveGeneration++;
        int myGeneration = waveGeneration;
        waveOngoing = true;
        StoreManager.RerollStore();
        StoreManager.RestockStore();

        for (int i = 0; i < specialWave.roadsToCreate; i++)
        {
            yield return new WaitForSeconds(specialWave.newSpawnRate);
            roadMaker.ExtendRoad();
        }

        yield return new WaitForSeconds(specialWave.newSpawnRate);
        roadMaker.CheckIfRoadIsOnRoadAndMaybeExtendIt();

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < specialWave.startPosMoveNr; i++)
        {
            StartCoroutine(roadMaker.MoveStartRoad(specialWave.newStartMoveRate));

            yield return new WaitForSeconds(specialWave.newStartMoveRate);
        }

        yield return new WaitForSeconds(0.5f);

        if (specialWave.respawnEnemies)
        {
            foreach (Enemy enemy in enemiesParent.transform.GetComponentsInChildren<Enemy>(true))
            {
                enemy.Respawn();
            }
        }

        foreach (EnemyGroup group in specialWave.enemyGroups)
        {
            for (int i = 0; i < group.amount; i++)
            {
                Instantiate(group.enemy, spawnPoint.position, spawnPoint.rotation, enemiesParent.transform);
                if (specialWave.newSpawnRate > 0) yield return new WaitForSeconds(specialWave.newSpawnRate);
                else yield return new WaitForSeconds(rate);
            }
        }

        int waveMod = (int)math.floor(wave / 10f);
        waveValueTotal += waveMod + 1;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckForEnemies(myGeneration));
    }
    IEnumerator SpawnWave()
    {
        waveGeneration++;
        int myGeneration = waveGeneration;
        waveOngoing = true;
        StoreManager.RerollStore();
        StoreManager.RestockStore();

        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            roadMaker.ExtendRoad();
        }

        yield return new WaitForSeconds(0.5f);
        roadMaker.CheckIfRoadIsOnRoadAndMaybeExtendIt();

        if (wave >= 5 && wave % 2 == 1)
        {

            StartCoroutine(roadMaker.MoveStartRoad(1));
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.5f);

        foreach (Enemy enemy in enemiesParent.transform.GetComponentsInChildren<Enemy>(true))
        {
            enemy.Respawn();
        }


        int waveValue = 0;
        int enemiesSpawned = 0;

        while (waveValue < waveValueTotal && enemiesSpawned < 30)
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

            if (highestValueEnemy.GetComponent<Enemy>().waveReq > wave) continue;

            if (highestValueEnemy.GetComponent<Enemy>().WaveValue + waveValue <= waveValueTotal)
            {
                waveValue += highestValueEnemy.GetComponent<Enemy>().WaveValue;
                enemiesSpawned++;
                Instantiate(highestValueEnemy, spawnPoint.position, spawnPoint.rotation, enemiesParent.transform);
                yield return new WaitForSeconds(rate);
            }
        }

        int waveMod = (int)math.floor(wave / 10f);
        waveValueTotal += waveMod + 1;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckForEnemies(myGeneration));

    }
    IEnumerator CheckForEnemies(int myGeneration)
    {
        while (waveOngoing && myGeneration == waveGeneration)
        {
            if (enemiesParent.transform.Cast<Transform>().All(t => !t.gameObject.activeSelf))
            {
                waveOngoing = false;
                wave++;
                StoreManager.AddMoney();
                StoreManager.UpdateMoneyGained();
                nextWavebutton.SetActive(true);
                storebutton.SetActive(true);
                moneyText.SetActive(true);
                yetToBeAddedMoneyText.SetActive(true);
                currentWaveText.text = "Next wave: " + wave.ToString();
                yield break;
            }
            yield return new WaitForSeconds(rate);
        }
    }

}
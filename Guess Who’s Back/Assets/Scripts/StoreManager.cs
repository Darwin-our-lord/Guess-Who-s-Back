using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TowerEntry
{
    public GameObject towerPrefab;
    public Rarities rarity;
}
public class StoreManager : MonoBehaviour
{
    public List<TowerEntry> towers = new List<TowerEntry>();

    public List<TowerEntry> towersInShop = new List<TowerEntry>();
    public List<GameObject> towerButtons = new List<GameObject>();
    public GameObject rerollButton;
    public TMP_Text moneyText;
    public TMP_Text toBeAddedMoneyText;

    public Placement placement;
    public MenuManager menuManager;

    public int money = 50;
    private int yetToBeAddedMoney = 0;

    private List<(Rarities, int, UnityEngine.Color)> allRarities = new List<(Rarities, int, Color)>
    {(Rarities.common,50,UnityEngine.Color.white),
     (Rarities.uncommon, 30, UnityEngine.Color.gray),
     (Rarities.rare,10,UnityEngine.Color.blue),
     (Rarities.epic,6,UnityEngine.Color.magenta),
     (Rarities.legendary,3,UnityEngine.Color.yellow),
     (Rarities.mytical,1,UnityEngine.Color.black)};


    private void Awake()
    {
        money = 50;
        UpdateMoneyUI();
        RerollStore();
    }

    public void RerollStore()
    {
        for (int i = 0; i < 3; i++)
        {
            Rarities rarity = Rarities.common;
            int totalWeight = 0;
            for (int j = 0; j < allRarities.Count; j++)
            {
                totalWeight += allRarities[j].Item2;
            }

            float randomValue = Random.Range(0f, totalWeight);

            float currentWeight = 0f;
            foreach (var option in allRarities)
            {
                currentWeight += option.Item2;
                if (randomValue >= currentWeight)
                {
                    rarity = option.Item1;
                }
            }
            while (true)
            {
                TowerEntry tower = towers[Random.Range(0, towers.Count)];
                if (tower.rarity == rarity)
                {
                    towersInShop[i] = tower;
                    break;
                }
            }
        }
        UpdateStoreTowerVisuals();
    }

    public void UpdateStoreTowerVisuals()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < allRarities.Count; ++j)
            {
                if (allRarities[j].Item1 == towersInShop[i].rarity)
                {
                    towerButtons[i].transform.GetChild(0).GetComponent<Image>().color = allRarities[j].Item3;
                }
            }
            towerButtons[i].transform.GetChild(1).GetComponent<Image>().sprite = towersInShop[i].towerPrefab.GetComponent<SpriteRenderer>().sprite;
            towerButtons[i].transform.GetChild(2).gameObject.SetActive(false);
            if (towersInShop[i].towerPrefab.transform.childCount != 0)
            {
                towerButtons[i].transform.GetChild(2).gameObject.SetActive(true);
                towerButtons[i].transform.GetChild(2).GetComponent<Image>().sprite = towersInShop[i].towerPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            }
            towerButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = towersInShop[i].towerPrefab.name;
            towerButtons[i].transform.GetChild(4).GetComponent<TMP_Text>().text = towersInShop[i].towerPrefab.GetComponent<Tower>().GetDescription();
        }

    }

    public void SelectTower(int buttonID)
    {
        if (money >= towersInShop[buttonID].towerPrefab.GetComponent<Tower>().Cost)
        {
            placement.TowerObjPrefab = towersInShop[buttonID].towerPrefab;
            placement.TowerObjFake.GetComponent<SpriteRenderer>().sprite = towersInShop[buttonID].towerPrefab.GetComponent<SpriteRenderer>().sprite;
            placement.TowerObjFake.transform.localScale = towersInShop[buttonID].towerPrefab.transform.localScale;
            placement.selectedTowerCost = towersInShop[buttonID].towerPrefab.GetComponent<Tower>().Cost;
            menuManager.StoreButton();
            if (rerollButton.activeSelf) rerollButton.SetActive(false);
        }
    }
    public void UpdateMoneyUI()
    {

        moneyText.text = "Money: " + money.ToString();
        toBeAddedMoneyText.text = "+" + yetToBeAddedMoney.ToString();

    }
    public void AddMoney(int amount)
    {
        EnemySpawner enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        if (enemySpawner.waveOngoing)
        {
            yetToBeAddedMoney += amount;
            UpdateMoneyUI();
        }
        else
        {
            yetToBeAddedMoney += amount;
            money += yetToBeAddedMoney;
            yetToBeAddedMoney = 0;
            UpdateMoneyUI();
        }
    }
}
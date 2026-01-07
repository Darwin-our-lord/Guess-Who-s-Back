using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class StoreManager : MonoBehaviour
{
    public List<GameObject> towers = new List<GameObject>();
    public List<GameObject> towersInShop = new List<GameObject>();
    public List<GameObject> towerButtons = new List<GameObject>();
    public TMP_Text moneyText;
    public TMP_Text toBeAddedMoneyText;

    public Placement placement;
    public MenuManager menuManager;

    public int money = 50;
    private int yetToBeAddedMoney = 0;
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
            towersInShop[i] = towers[Random.Range(0, towers.Count)];
        }
        UpdateStoreTowerVisuals();
    }

    public void UpdateStoreTowerVisuals()
    {
        for (int i = 0; i < 3; i++)
        {
            towerButtons[i].transform.GetChild(1).GetComponent<Image>().sprite = towersInShop[i].GetComponent<SpriteRenderer>().sprite;
            towerButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = towersInShop[i].name;
            towerButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = towersInShop[i].GetComponent<Tower>().GetDescription();
        }

    }

    public void SelectTower(int buttonID)
    {
        if (money >= towersInShop[buttonID].GetComponent<Tower>().Cost)
        {
            placement.TowerObjPrefab = towersInShop[buttonID];
            placement.TowerObjFake.GetComponent<SpriteRenderer>().sprite = towersInShop[buttonID].GetComponent<SpriteRenderer>().sprite;
            menuManager.StoreButton();
            money -= towersInShop[buttonID].GetComponent<Tower>().Cost;
            RerollStore();
            UpdateMoneyUI();
        }
    }
    public void UpdateMoneyUI()
    {

        moneyText.text = "Money: " + money.ToString();
        toBeAddedMoneyText.text = "+" + yetToBeAddedMoney.ToString();
        
    }
    public void AddMoney(int amount)
    {
        if (EnemySpawner.waveOngoing)
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StoreManager : MonoBehaviour
{
    public List<GameObject> towers = new List<GameObject>();
    public List<GameObject> towersInShop = new List<GameObject>();
    public List<GameObject> towerButtons = new List<GameObject>();

    public Placement placement;
    public MenuManager menuManager;
    //[Header("UI elements")]

    private void Awake()
    {
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
            towerButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = towersInShop[i].GetComponent<Tower>().description;
        }

    }

    public void SelectTower(int buttonID)
    {
        placement.TowerObjPrefab = towersInShop[buttonID];
        placement.TowerObjFake.GetComponent<SpriteRenderer>().sprite = towersInShop[buttonID].GetComponent<SpriteRenderer>().sprite;
        menuManager.StoreButton();
    }
}

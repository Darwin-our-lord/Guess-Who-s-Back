using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public List<GameObject> towers = new List<GameObject>();
    public List<GameObject> towersInShop = new List<GameObject>();

    //[Header("UI elements")]


    public void RerollStore()
    {
        for (int i = 0; i < 3; i++)
        {
            towersInShop[i] = towers[Random.Range(0, towers.Count)];
        }

    }

    public void UpdateStoreTowerVisuals()
    {
        foreach (GameObject tower in towersInShop)
        {
            
        }
    }
}

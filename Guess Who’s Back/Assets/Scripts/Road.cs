using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public List<Transform> nextTiles = new List<Transform>();
    public Transform lastTile;

    public void AddNextTile(Transform tile)
    {
        if (tile == null) return;
        if (!nextTiles.Contains(tile))
            nextTiles.Add(tile);
            lastTile = tile;
    }
}

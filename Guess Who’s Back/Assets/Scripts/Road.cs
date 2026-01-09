using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Road : MonoBehaviour
{
    public List<Transform> nextTiles = new List<Transform>();
    public Transform lastTile;

    //public List<Sprite> roadSprites = new List<Sprite>();

    public List<Sprite> roadSprites = new List<Sprite>();
    public LayerMask layerMask;

    public void AddNextTile(Transform tile)
    {
        if (tile == null) return;
        if (!nextTiles.Contains(tile))
            nextTiles.Add(tile);
            lastTile = tile;
    }
    public void UpdateSprite(Transform fromTransform)
    {
        bool stacked = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.9f, 0.9f), 0f, layerMask);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit != GetComponent<Collider2D>())
            {
                stacked = true;
                break;
            }
        }


        SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();

        Vector3 inDirRaw = (transform.position - fromTransform.position).normalized;

        Vector3Int inDir = Vector3Int.RoundToInt(inDirRaw);

        if (nextTiles.Count == 0 && !stacked)
        {
            switch (inDir)
            {
                case var move when inDir == Vector3Int.right:
                    sr.sprite = roadSprites[0];
                    break;
                case var move when inDir == Vector3Int.left:
                    sr.sprite = roadSprites[1];
                    break;
                case var move when inDir == Vector3Int.up:
                    sr.sprite = roadSprites[2];
                    break;
                case var move when inDir == Vector3Int.down:
                    sr.sprite = roadSprites[3];
                    break;
                default:
                    Debug.LogError("your sprite shit broke twin - but its the end piece");
                    break;
            }

            return;
        }
        else if (nextTiles.Count == 0 && stacked)
        {

            switch (inDir)
            {
                case var move when inDir == Vector3Int.right:
                    sr.sprite = roadSprites[10];
                    break;
                case var move when inDir == Vector3Int.left:
                    sr.sprite = roadSprites[11];
                    break;
                case var move when inDir == Vector3Int.up:
                    sr.sprite = roadSprites[12];
                    break;
                case var move when inDir == Vector3Int.down:
                    sr.sprite = roadSprites[13];
                    break;
                default:
                    Debug.LogError("your sprite shit broke twin - but its the end piece and its stacked"); 
                    break;
            }

            return;
        }

        Vector3 outDirRaw = (nextTiles[0].position - transform.position).normalized;


        Vector3Int outDir = Vector3Int.RoundToInt(outDirRaw);


        if (!stacked)
        {
            switch (inDir, outDir)
            {
                // --- STRAIGHTS ---

                // Horizontal: Moving Right OR Moving Left
                case var move when (move.inDir == Vector3Int.right && move.outDir == Vector3Int.right) ||
                               (move.inDir == Vector3Int.left && move.outDir == Vector3Int.left):
                    sr.sprite = roadSprites[4];
                    break;

                // Vertical: Moving Up OR Moving Down
                case var move when (move.inDir == Vector3Int.up && move.outDir == Vector3Int.up) ||
                               (move.inDir == Vector3Int.down && move.outDir == Vector3Int.down):
                    sr.sprite = roadSprites[5];
                    break;


                // --- CORNERS ---

                // 1. Moving Right then Up  (Enter Left, Exit Top)
                // 2. Moving Down then Left (Enter Top, Exit Left)
                case var move when (move.inDir == Vector3Int.right && move.outDir == Vector3Int.up) ||
                               (move.inDir == Vector3Int.down && move.outDir == Vector3Int.left):
                    sr.sprite = roadSprites[6];
                    break;


                // 1. Moving Left then Up    (Enter Right, Exit Top)
                // 2. Moving Down then Right (Enter Top, Exit Right)
                case var move when (move.inDir == Vector3Int.left && move.outDir == Vector3Int.up) ||
                               (move.inDir == Vector3Int.down && move.outDir == Vector3Int.right):
                    sr.sprite = roadSprites[7];
                    break;


                // 1. Moving Right then Down (Enter Left, Exit Bottom)
                // 2. Moving Up then Left    (Enter Bottom, Exit Left)
                case var move when (move.inDir == Vector3Int.right && move.outDir == Vector3Int.down) ||
                               (move.inDir == Vector3Int.up && move.outDir == Vector3Int.left):
                    sr.sprite = roadSprites[9];
                    break;


                // 1. Moving Left then Down (Enter Right, Exit Bottom)
                // 2. Moving Up then Right  (Enter Bottom, Exit Right)
                case var move when (move.inDir == Vector3Int.left && move.outDir == Vector3Int.down) ||
                               (move.inDir == Vector3Int.up && move.outDir == Vector3Int.right):
                    sr.sprite = roadSprites[8];
                    break;

                default:
                    //shii idk twin
                    Debug.LogError("your sprite shit broke twin - ordinary");
                    break;
            }
        }
        else if (stacked)
        {
            switch (inDir, outDir)
            {
                default:
                    //shii idk twin
                    sr.sprite = roadSprites[14];
                    sr.sortingOrder = 8;
                    break;
            }
        }
        
    }

}

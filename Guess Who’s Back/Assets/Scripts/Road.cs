using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class Road : MonoBehaviour
{
    public List<Transform> nextTiles = new List<Transform>();
    public Transform fromtile;
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
        fromtile = fromTransform;
        bool stacked = false;
        Vector3Int inDir2 = new Vector3Int();
        Vector3Int outDir2 = new Vector3Int();

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.9f, 0.9f), 0f, layerMask);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit != GetComponent<Collider2D>())
            {
                stacked = true;

                Vector3 inDirRaw2 = (hit.gameObject.transform.position - hit.gameObject.GetComponent<Road>().fromtile.position).normalized;

                inDir2 = Vector3Int.RoundToInt(inDirRaw2);

                Vector3 outDirRaw2 = (hit.gameObject.GetComponent<Road>().nextTiles[0].position - hit.gameObject.transform.position).normalized;


                outDir2 = Vector3Int.RoundToInt(outDirRaw2);

                break;
            }
        }


        SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();

        Vector3 inDirRaw = (transform.position - fromtile.position).normalized;

        Vector3Int inDir = Vector3Int.RoundToInt(inDirRaw);

        #region oldcode(doesnt do anything)
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
        #endregion

        Vector3 outDirRaw = (nextTiles[0].position - transform.position).normalized;


        Vector3Int outDir = Vector3Int.RoundToInt(outDirRaw);

        if(nextTiles.Count > 1)
        {

            HashSet<Vector3Int> outDirs = new HashSet<Vector3Int>();

            foreach (Transform t in nextTiles)
            {
                if (t == null) continue;
                Vector3Int d = Vector3Int.RoundToInt((t.position - transform.position).normalized);
                outDirs.Add(d);
            }

            // Combine in + out directions
            bool left = outDirs.Contains(Vector3Int.left) || inDir == Vector3Int.right;
            bool right = outDirs.Contains(Vector3Int.right) || inDir == Vector3Int.left;
            bool up = outDirs.Contains(Vector3Int.up) || inDir == Vector3Int.down;
            bool down = outDirs.Contains(Vector3Int.down) || inDir == Vector3Int.up;

            int connections =
                (left ? 1 : 0) +
                (right ? 1 : 0) +
                (up ? 1 : 0) +
                (down ? 1 : 0);

            // ----- T JUNCTIONS -----
            if (connections == 3)
            {
                if (!up) sr.sprite = roadSprites[16]; // T missing up
                else if (!down) sr.sprite = roadSprites[15]; // T missing down
                else if (!left) sr.sprite = roadSprites[18]; // T missing left
                else if (!right) sr.sprite = roadSprites[17]; // T missing right
                return;
            }

        }
        else if (!stacked)
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

            HashSet<Vector3Int> outDirs = new HashSet<Vector3Int>();
            HashSet<Vector3Int> inDirs = new HashSet<Vector3Int>();

            foreach (Transform t in nextTiles)
            {
                if (t == null) continue;
                Vector3Int d = Vector3Int.RoundToInt((t.position - transform.position).normalized);
                outDirs.Add(d);
            }
            outDirs.Add(outDir2);

            inDirs.Add(inDir);
            inDirs.Add(inDir2);

            // Combine in + out directions
            bool left = outDirs.Contains(Vector3Int.left) || inDirs.Contains(Vector3Int.right);
            bool right = outDirs.Contains(Vector3Int.right) || inDirs.Contains(Vector3Int.left);
            bool up = outDirs.Contains(Vector3Int.up) || inDirs.Contains(Vector3Int.down);
            bool down = outDirs.Contains(Vector3Int.down) || inDirs.Contains(Vector3Int.up);

            int connections =
                (left ? 1 : 0) +
                (right ? 1 : 0) +
                (up ? 1 : 0) +
                (down ? 1 : 0);

            // ----- T JUNCTIONS -----
            if (connections == 3)
            { 
                if (!up) sr.sprite = roadSprites[16]; // T missing up
                else if (!down) sr.sprite = roadSprites[15]; // T missing down
                else if (!left) sr.sprite = roadSprites[18]; // T missing left
                else if (!right) sr.sprite = roadSprites[17]; // T missing right
            }
            else
            {
                sr.sprite = roadSprites[14];
            }

            sr.sortingOrder = 80;
        }
        
    }

}

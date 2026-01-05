using System.Collections.Generic;
using UnityEngine;

public class RoadMaker : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject RoadObjPrefab;

    public LayerMask layerMask;

    public Vector2 lastRoadPos = new Vector2(0.5f,0.5f);
    public Vector2 lastlastRoadPos;
    public List<GameObject> roads = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < 5; i++) 
        {
            ExtendRoad();
        }
    }
    public void ExtendRoad()
    {
        int attempts = 0;
        while (true)
        {
            attempts++;

            Vector2 newRoadPos = lastRoadPos;

            //1=up   2=down   3=left   4=right
            int direction = Random.Range(1, 5);
            switch (direction)
            {
                case 1: newRoadPos += new Vector2(1, 0);  break;
                case 2: newRoadPos += new Vector2(-1,0);  break;
                case 3: newRoadPos += new Vector2(0,-1);  break;
                case 4: newRoadPos += new Vector2(0, 1);  break;
            }

            if (newRoadPos == lastlastRoadPos) continue;

            Collider2D hit = Physics2D.OverlapBox(newRoadPos, new Vector2(0.9f, 0.9f), 0f, layerMask);

            if (hit == null)
            {
                GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity);
                roads.Add(newRoad);
                lastlastRoadPos = lastRoadPos;
                lastRoadPos = newRoadPos;
                break;
            }
            else if (hit.gameObject.CompareTag("Tower"))
            {
                Destroy(hit.gameObject);
                GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity);
                roads.Add(newRoad);
                lastlastRoadPos = lastRoadPos;
                lastRoadPos = newRoadPos;
                break;
            }
            else if (hit.gameObject.CompareTag("Road"))
            {
                if (attempts < 7) continue;

                GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity);
                roads.Add(newRoad);
                lastlastRoadPos = lastRoadPos;
                lastRoadPos = newRoadPos;
                break;
            }
            else
            {
                continue;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMaker : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject RoadObjPrefab;
    public GameObject RoadEndObjPrefab;
    public GameObject RoadStartObjPrefab;

    public LayerMask layerMask;

    public Vector2 lastRoadPos = new Vector2(0.5f,0.5f);
    public Vector2 lastlastRoadPos;

    public GameObject roadsParent;
    public List<GameObject> roads = new List<GameObject>();


    private void Start()
    {
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {
        for (int i = 0; i < 10; i++)
        {

            ExtendRoad();
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void ExtendRoad()
    {
        int attempts = 0;
        if (roads.Count != 0)
        {
            Destroy(roads[roads.Count - 1]);
            roads.Remove(roads[roads.Count - 1]);

            GameObject oldRoad = Instantiate(RoadObjPrefab, lastRoadPos, Quaternion.identity, roadsParent.transform);
            roads.Add(oldRoad);
        }
        else
        {
            GameObject oldRoad = Instantiate(RoadStartObjPrefab, lastRoadPos, Quaternion.identity, roadsParent.transform);
            roads.Add(oldRoad);
        }

        while (true)
        {
            attempts++;

            Vector2 newRoadPos = lastRoadPos;

            //1=up   2=down   3=left   4=right
            int direction = UnityEngine.Random.Range(1, 5);
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
                GameObject newRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                roads.Add(newRoad);
                lastlastRoadPos = lastRoadPos;
                lastRoadPos = newRoadPos;
                break;
            }
            else if (hit.gameObject.CompareTag("Tower"))
            {
                Destroy(hit.gameObject);
                GameObject newRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                roads.Add(newRoad);
                lastlastRoadPos = lastRoadPos;
                lastRoadPos = newRoadPos;
                break;
            }
            else if (hit.gameObject.CompareTag("Road"))
            {
                if (hit.gameObject == roads[0]) continue;

                Vector2 roadTest = lastRoadPos;
                roadTest += new Vector2(1, 0);
                Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                roadTest = lastRoadPos;
                roadTest += new Vector2(-1, 0);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                roadTest = lastRoadPos;
                roadTest += new Vector2(0, 1);
                hit2=null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                roadTest = lastRoadPos;
                roadTest += new Vector2(0, -1);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;



                GameObject newRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                roads.Add(newRoad);
                lastlastRoadPos = lastRoadPos;
                lastRoadPos = newRoadPos;
                break;
            }
            else if (hit.gameObject.CompareTag("Wall"))
            {
                Vector2 roadTest = lastRoadPos;
                roadTest += new Vector2(1, 0);
                Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                roadTest = lastRoadPos;
                roadTest += new Vector2(-1, 0);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                roadTest = lastRoadPos;
                roadTest += new Vector2(0, 1);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                roadTest = lastRoadPos;
                roadTest += new Vector2(0, -1);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit.gameObject.CompareTag("Tower")) continue;

                Destroy(hit.gameObject);
                continue;
            }
            else
            {
                Debug.LogError("INFINITE LOOP");
                break;
            }
        }
    }
}

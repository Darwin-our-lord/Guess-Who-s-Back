using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;



public class RoadMaker : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject RoadObjPrefab;
    public GameObject RoadEndObjPrefab;
    public GameObject RoadStartObjPrefab;

    public LayerMask layerMask;

    public GameObject roadsParent;

    public float branchChance = 0.02f;
    public List<GameObject> branchFronts = new List<GameObject>();
    public List<GameObject> fakeFronts = new List<GameObject>();
    public List<GameObject> formerBranchFronts = new List<GameObject>();
    public GameObject firstRoad;

    public EnemySpawner enemySpawner;

    private void Start()
    {
        StartCoroutine(GenerateLevel());
    }

    IEnumerator GenerateLevel()
    {

        for (int i = 0; i < 10; i++)
        {
            ExtendRoad();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ExtendRoad()
    {
        if (branchFronts.Count == 0)
        {
            GameObject oldRoad = Instantiate(RoadStartObjPrefab, new Vector2(0.5f, 0.5f), Quaternion.identity, roadsParent.transform);
            GameObject newRoad = Instantiate(RoadObjPrefab, new Vector2(0.5f, 1.5f), Quaternion.identity, roadsParent.transform);
            GameObject fakeRoad = Instantiate(RoadEndObjPrefab, new Vector2(0.5f, 1.5f), Quaternion.identity, roadsParent.transform);

            fakeFronts.Add(fakeRoad);
            branchFronts.Add(newRoad);
            formerBranchFronts.Add(oldRoad);

            firstRoad = newRoad;
        }
        if (Random.value < branchChance && enemySpawner.wave >= 10)
        {
            List<int> possibleBranches = new List<int>();
            for (int i = 0; i < branchFronts.Count;i++) 
            {
                int possiblePlaces = 0;
                Vector2 roadTest = branchFronts[i].transform.position;
                roadTest += new Vector2(1, 0);
                Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit2.gameObject.CompareTag("Tower")) possiblePlaces++;

                roadTest = branchFronts[i].transform.position;
                roadTest += new Vector2(-1, 0);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit2.gameObject.CompareTag("Tower")) possiblePlaces++;

                roadTest = branchFronts[i].transform.position;
                roadTest += new Vector2(0, 1);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit2.gameObject.CompareTag("Tower")) possiblePlaces++;

                roadTest = branchFronts[i].transform.position;
                roadTest += new Vector2(0, -1);
                hit2 = null;
                hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit2 == null || hit2.gameObject.CompareTag("Tower")) possiblePlaces++;


                if (possiblePlaces >= 2) possibleBranches.Add(i);
            }


            while (true)
            {
                if (possibleBranches.Count == 0)
                {
                    break;
                }

                int branchStart = possibleBranches[UnityEngine.Random.Range(0, possibleBranches.Count)];

                Vector2 newRoadPos = branchFronts[branchStart].transform.position;

                int direction = UnityEngine.Random.Range(1, 5);
                switch (direction)
                {
                    case 1: newRoadPos += new Vector2(1, 0); break;//1=up 
                    case 2: newRoadPos += new Vector2(-1, 0); break;//2=down
                    case 3: newRoadPos += new Vector2(0, -1); break;//3=left
                    case 4: newRoadPos += new Vector2(0, 1); break;//4=right
                }

                if (newRoadPos == new Vector2(formerBranchFronts[branchStart].transform.position.x, formerBranchFronts[branchStart].transform.position.y)) continue;

                Collider2D hit = Physics2D.OverlapBox(newRoadPos, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit == null)
                {
                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[branchStart].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[branchStart].GetComponent<Road>().UpdateSprite(branchFronts[branchStart].transform);

                    formerBranchFronts.Add(branchFronts[branchStart]);
                    branchFronts.Add(newRoad);
                    fakeFronts.Add(fakeRoad);

                    break;
                }
                else if (hit.gameObject.CompareTag("Tower"))
                {
                    Destroy(hit.gameObject);

                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);


                    branchFronts[branchStart].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[branchStart].GetComponent<Road>().UpdateSprite(branchFronts[branchStart].transform);

                    formerBranchFronts.Add(branchFronts[branchStart]);
                    branchFronts.Add(newRoad);
                    fakeFronts.Add(fakeRoad);

                    break;
                }
                else if (hit.gameObject.CompareTag("Road"))
                {
                    if (hit.gameObject.name == RoadStartObjPrefab.name) continue;

                    #region checkForValidSpotElsewhere
                    Vector2 roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(1, 0);
                    Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(-1, 0);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(0, 1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(0, -1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    #endregion

                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[branchStart].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[branchStart].GetComponent<Road>().UpdateSprite(branchFronts[branchStart].transform);

                    formerBranchFronts.Add(branchFronts[branchStart]);
                    branchFronts.Add(newRoad);
                    fakeFronts.Add(fakeRoad);

                    break;
                }
                else if (hit.gameObject.CompareTag("Wall"))
                {
                    #region checkForValidSpotElsewhere
                    Vector2 roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(1, 0);
                    Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(-1, 0);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(0, 1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[branchStart].transform.position;
                    roadTest += new Vector2(0, -1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    #endregion

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
        for (int i = 0; i < branchFronts.Count; i++)
        {

            Destroy(fakeFronts[i]);

            while (true)
            {
                Vector2 newRoadPos = branchFronts[i].transform.position;

                int direction = UnityEngine.Random.Range(1, 5);
                switch (direction)
                {
                    case 1: newRoadPos += new Vector2(1, 0); break;//1=up 
                    case 2: newRoadPos += new Vector2(-1, 0); break;//2=down
                    case 3: newRoadPos += new Vector2(0, -1); break;//3=left
                    case 4: newRoadPos += new Vector2(0, 1); break;//4=right
                }

                if (newRoadPos == new Vector2(formerBranchFronts[i].transform.position.x, formerBranchFronts[i].transform.position.y)) continue;

                Collider2D hit = Physics2D.OverlapBox(newRoadPos, new Vector2(0.9f, 0.9f), 0f, layerMask);

                if (hit == null)
                {
                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[i].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[i].GetComponent<Road>().UpdateSprite(formerBranchFronts[i].transform);

                    formerBranchFronts[i] = branchFronts[i];
                    branchFronts[i] = newRoad;
                    fakeFronts[i] =fakeRoad;

                    break;
                }
                else if (hit.gameObject.CompareTag("Tower"))
                {
                    Destroy(hit.gameObject);

                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[i].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[i].GetComponent<Road>().UpdateSprite(formerBranchFronts[i].transform);

                    formerBranchFronts[i] = branchFronts[i];
                    branchFronts[i] = newRoad;
                    fakeFronts[i] = fakeRoad;

                    break;
                }
                else if (hit.gameObject.CompareTag("Road"))
                {
                    if (hit.gameObject.name == RoadStartObjPrefab.name) continue;

                    #region checkForValidSpotElsewhere
                    Vector2 roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(1, 0);
                    Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(-1, 0);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(0, 1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(0, -1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    #endregion

                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[i].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[i].GetComponent<Road>().UpdateSprite(formerBranchFronts[i].transform);

                    formerBranchFronts[i] = branchFronts[i];
                    branchFronts[i] = newRoad;
                    fakeFronts[i] = fakeRoad;

                    break;
                }
                else if (hit.gameObject.CompareTag("Wall"))
                {
                    #region checkForValidSpotElsewhere
                    Vector2 roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(1, 0);
                    Collider2D hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(-1, 0);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(0, 1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    roadTest = branchFronts[i].transform.position;
                    roadTest += new Vector2(0, -1);
                    hit2 = null;
                    hit2 = Physics2D.OverlapBox(roadTest, new Vector2(0.9f, 0.9f), 0f, layerMask);

                    if (hit2 == null || hit2.gameObject.CompareTag("Tower")) continue;

                    #endregion

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
}

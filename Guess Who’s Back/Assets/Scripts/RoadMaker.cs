using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public List<GameObject> formerBranchFronts = new List<GameObject>(); //all roads that arent the current branch front
    public GameObject firstRoad;
    public GameObject startRoad;

    [SerializeField] EnemySpawner enemySpawner;

    [Header("")]
    [SerializeField] Sprite[] roadStartSprites;

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
        StartCoroutine(CheckIfRoadIsOnRoadAndMaybeExtendIt());
        yield return new WaitForSeconds(0.1f);
    }

    public void ExtendRoad()
    {
        if (branchFronts.Count == 0)  //make the first road if there are no roads yet
        {
            int randomDir;
            Vector2 direction = (randomDir = Random.Range(1, 5)) == 1 ? new Vector2(0.5f, 1.5f) : randomDir == 2 ? new Vector2(1.5f, 0.5f) : randomDir == 3 ? new Vector2(0.5f, -0.5f) : new Vector2(-0.5f, 0.5f);

            GameObject oldRoad = Instantiate(RoadStartObjPrefab, new Vector2(0.5f, 0.5f), Quaternion.identity, roadsParent.transform);
            GameObject newRoad = Instantiate(RoadObjPrefab, direction, Quaternion.identity, roadsParent.transform);
            GameObject fakeRoad = Instantiate(RoadEndObjPrefab, new Vector2(0.5f, 1.5f), Quaternion.identity, roadsParent.transform);
            
            GameObject startSpriteRoad = Instantiate(RoadObjPrefab, new Vector2(0.5f, 0.5f), Quaternion.identity, roadsParent.transform);
            startSpriteRoad.GetComponent<SpriteRenderer>().sprite = roadStartSprites[randomDir-1];

            fakeFronts.Add(fakeRoad);
            branchFronts.Add(newRoad);
            formerBranchFronts.Add(oldRoad);

            startRoad = oldRoad;
            firstRoad = newRoad;
        }
        if (Random.value < branchChance && enemySpawner.wave >= 10) //check if a branch should be made
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
                    if (hit.gameObject.name == RoadStartObjPrefab.name+"(clone)") continue;

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

                Collider2D[] hit = Physics2D.OverlapBoxAll(newRoadPos, new Vector2(0.9f, 0.9f), 0f, layerMask);

                bool HitTower = false;
                bool HitRoad = false;
                bool HitStart = false;
                bool HitWall = false;
                if (hit != null)
                {
                    foreach (Collider2D collider in hit)
                    {
                        if (collider.CompareTag("Tower"))
                        {
                            HitTower = true;
                        }
                        if (collider.CompareTag("Road"))
                        {
                            HitRoad = true;
                        }
                        if(collider.gameObject == startRoad)
                        {
                            HitStart = true;
                        }
                        if (collider.gameObject.CompareTag("Wall"))
                        {
                            HitWall = true;
                        }
                    }
                }

                if (HitStart) continue;

                if (hit.Length == 0)
                {
                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[i].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[i].GetComponent<Road>().UpdateSprite(formerBranchFronts[i].transform);

                    formerBranchFronts[i] = branchFronts[i];
                    branchFronts[i] = newRoad;
                    fakeFronts[i] = fakeRoad;

                    break;
                }
                else if (HitTower && !HitRoad)
                {
                    foreach (Collider2D collider in hit)
                    {
                        if (collider.CompareTag("Tower"))
                        {
                            Destroy(collider.gameObject);
                        }
                    }

                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[i].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[i].GetComponent<Road>().UpdateSprite(formerBranchFronts[i].transform);

                    formerBranchFronts[i] = branchFronts[i];
                    branchFronts[i] = newRoad;
                    fakeFronts[i] = fakeRoad;

                    break;
                }
                else if (HitRoad)
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

                    GameObject fakeRoad = Instantiate(RoadEndObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);
                    GameObject newRoad = Instantiate(RoadObjPrefab, newRoadPos, Quaternion.identity, roadsParent.transform);

                    branchFronts[i].GetComponent<Road>().AddNextTile(newRoad.transform);
                    branchFronts[i].GetComponent<Road>().UpdateSprite(formerBranchFronts[i].transform);

                    formerBranchFronts[i] = branchFronts[i];
                    branchFronts[i] = newRoad;
                    fakeFronts[i] = fakeRoad;

                    break;
                }
                else if (HitWall)
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

                    foreach (Collider2D collider in hit)
                    {
                        if (collider.CompareTag("Wall"))
                        {
                            Destroy(collider.gameObject);
                        }
                    }
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
    public IEnumerator CheckIfRoadIsOnRoadAndMaybeExtendIt()
    {
        for (int i = 0; i < branchFronts.Count; i++)
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(branchFronts[i].transform.position, new Vector2(0.9f, 0.9f), 0f, layerMask);
            for (int j = 0; j < hit.Length; j++)
            {
                if (hit[j].gameObject == branchFronts[i])continue;

                if (hit[j].gameObject == fakeFronts[i])continue;

                if (hit[j].gameObject.CompareTag("Road"))
                {
                    ExtendRoad();
                    yield return new WaitForSeconds(0.25f);
                    StartCoroutine(CheckIfRoadIsOnRoadAndMaybeExtendIt());
                    break;
                }
            }
        }
        enemySpawner.RoadCheckComplete = true;
    }

    public IEnumerator MoveStartRoad(float duration)
    {
        Vector3 startPosition = startRoad.transform.position;


        Vector3 targetPosition = firstRoad.transform.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            startRoad.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        startRoad.transform.position = targetPosition;

        //GameObject ripMrRoad = firstRoad; //store the road so that it can be destroyed after the firstRoad variable is updated to the next road

        if (firstRoad.GetComponent<Road>().nextTiles.Count > 1)
        {
            int randomDir = Random.Range(0, firstRoad.GetComponent<Road>().nextTiles.Count - 1);

            /*for (int i = 0; i < firstRoad.GetComponent<Road>().nextTiles.Count; i++)
            {
                if (i != randomDir)
                {
                    if(firstRoad.GetComponent<Road>().nextTiles[i].gameObject.GetComponent<Road>().nextTiles.Count < 0)
                    {

                    }
                    Destroy(firstRoad.GetComponent<Road>().nextTiles[i].gameObject);
                }
            }*/

            enemySpawner.spawnPoint.position = firstRoad.transform.position;
            firstRoad = firstRoad.GetComponent<Road>().nextTiles[randomDir].gameObject;
        }
        else
        {
            enemySpawner.spawnPoint.position = firstRoad.transform.position;
            firstRoad = firstRoad.GetComponent<Road>().nextTiles[0].gameObject;
        }

        //Destroy(ripMrRoad);

    }

    /*void DestroyRoadBranch(GameObject roadObject)  //could be used to destroy a branch of roads, but currently not used cuz we lwk gotta talk about this type shit
    {
        Road road = roadObject.GetComponent<Road>();

        foreach (Transform nextTile in road.nextTiles)
        {
            if (nextTile != null)
            {
                DestroyRoadBranch(nextTile.gameObject);
            }
        }

        Destroy(roadObject);
    }*/

}

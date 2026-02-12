using UnityEngine;
using UnityEngine.EventSystems;
public class Placement : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject TowerObjFake;
    public GameObject RangeCicle;
    public GameObject TowerObjPrefab;
    public RoadMaker roadMaker;
    public Grid grid;
    public LayerMask layerMask;
    public GameObject towersParent;

    private int currentRotation = 0;
    public int selectedTowerCost = 0;

    private StoreManager storeManager;

    void Start()
    {
        storeManager = GameObject.Find("StoreManager").GetComponent<StoreManager>();
    }

    void Update()
    {
        if (TowerObjPrefab != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentRotation = (currentRotation + 1) % 4;
                UpdateFakeObjectRotation();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentRotation = (currentRotation - 1 + 4) % 4;
                UpdateFakeObjectRotation();
            }

            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = grid.WorldToCell(mousePos);
            Vector3 worldCenterPos = grid.GetCellCenterWorld(cellPos);
            if (TowerObjPrefab.transform.localScale.x % 2 == 0 || TowerObjPrefab.transform.localScale.y % 2 == 0) worldCenterPos += new Vector3(0.5f, 0.5f, 0);

            TowerObjFake.transform.position = worldCenterPos;
            RangeCicle.transform.localScale
                = new Vector3(TowerObjPrefab.GetComponent<Tower>().Range * 2, TowerObjPrefab.GetComponent<Tower>().Range * 2, 1);
            RangeCicle.transform.position = worldCenterPos;
            //places object---
            if (Input.GetMouseButton(0) && TowerObjPrefab != null && !EventSystem.current.IsPointerOverGameObject() || Input.GetMouseButton(0) && TowerObjPrefab != null && TowerObjPrefab.GetComponent<Tower>().isTrap)
            {
                Collider2D hit = Physics2D.OverlapBox(worldCenterPos, new Vector2(TowerObjFake.transform.localScale.x * 0.9f, TowerObjFake.transform.localScale.y * 0.9f), 0f, layerMask);
                if (hit == null && !TowerObjPrefab.GetComponent<Tower>().isTrap)
                {
                    GameObject clone = Instantiate(TowerObjPrefab, worldCenterPos, Quaternion.identity, towersParent.transform);
                    clone.name = TowerObjPrefab.name;

                    Tower towerScript = clone.GetComponent<Tower>();
                    towerScript.SetRotation(currentRotation);

                    storeManager.money -= selectedTowerCost;
                    storeManager.RerollStore();
                    storeManager.UpdateMoneyUI();

                    TowerObjPrefab = null;
                    TowerObjFake.GetComponent<SpriteRenderer>().sprite = null;
                    RangeCicle.transform.localScale = new Vector3(0, 0, 1);
                    currentRotation = 0;
                    selectedTowerCost = 0;
                    //tower placesssed do!!! 
                }
                else if (hit.CompareTag("Road") && TowerObjPrefab.GetComponent<Tower>().isTrap)
                {
                    GameObject clone = Instantiate(TowerObjPrefab, worldCenterPos, Quaternion.identity, towersParent.transform);
                    clone.name = TowerObjPrefab.name;

                    Tower towerScript = clone.GetComponent<Tower>();
                    towerScript.SetGridPosition(cellPos);

                    storeManager.money -= selectedTowerCost;
                    storeManager.RerollStore();
                    storeManager.UpdateMoneyUI();

                    TowerObjPrefab = null;
                    TowerObjFake.GetComponent<SpriteRenderer>().sprite = null;
                    RangeCicle.transform.localScale = new Vector3(0, 0, 1);
                    currentRotation = 0;
                    selectedTowerCost = 0;
                    //tower placesssed do!!! 
                }
            }
        }
    }

    private void UpdateFakeObjectRotation()
    {
        float angle = currentRotation * 90f;
        TowerObjFake.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void CancelPlacement()
    {
        TowerObjPrefab = null;
        TowerObjFake.GetComponent<SpriteRenderer>().sprite = null;
        RangeCicle.transform.localScale = new Vector3(0, 0, 1);
        currentRotation = 0;
        selectedTowerCost = 0;
    }
}
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

    void Update()
    {
        if (TowerObjPrefab != null)
        {
            //finds what pos the mouse is hovering---
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = grid.WorldToCell(mousePos);

            Vector3 worldCenterPos = grid.GetCellCenterWorld(cellPos);
            if (TowerObjPrefab.transform.localScale.x % 2 == 0 || TowerObjPrefab.transform.localScale.y % 2 == 0) worldCenterPos += new Vector3(0.5f,0.5f,0);
            
            TowerObjFake.transform.position = worldCenterPos;
            RangeCicle.transform.localScale
                = new Vector3(TowerObjPrefab.GetComponent<Tower>().Range*2, TowerObjPrefab.GetComponent<Tower>().Range*2, 1);
            RangeCicle.transform.position = worldCenterPos;

            //places object---
            if (Input.GetMouseButton(0) && TowerObjPrefab != null && !EventSystem.current.IsPointerOverGameObject())
            {
                Collider2D hit = Physics2D.OverlapBox(worldCenterPos, new Vector2(TowerObjFake.transform.localScale.x * 0.9f, TowerObjFake.transform.localScale.y * 0.9f), 0f, layerMask);

                if (hit == null)
                {
                    GameObject clone = Instantiate(TowerObjPrefab, worldCenterPos, Quaternion.identity, towersParent.transform);
                    clone.name = TowerObjPrefab.name;
                    TowerObjPrefab = null;
                    TowerObjFake.GetComponent<SpriteRenderer>().sprite = null;
                    RangeCicle.transform.localScale = new Vector3(0,0,1);

                    //tower placesssed do!!! 
                }
            }
        }
    }
    
}
using UnityEngine;
using UnityEngine.EventSystems;

public class Placement : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject TowerObjFake;
    public GameObject TowerObjPrefab;
    public RoadMaker roadMaker;
    public Grid grid;
    public LayerMask layerMask;
    public GameObject towersParent;

    void Update()
    {
        //finds what pos the mouse is hovering---
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = grid.WorldToCell(mousePos);

        Vector3 worldCenterPos = grid.GetCellCenterWorld(cellPos);

        TowerObjFake.transform.position = worldCenterPos;

        //places object---
        if (Input.GetMouseButton(0) && TowerObjPrefab != null && !EventSystem.current.IsPointerOverGameObject())
        {
            Collider2D hit = Physics2D.OverlapBox(worldCenterPos, new Vector2(TowerObjFake.transform.localScale.x * 0.9f, TowerObjFake.transform.localScale.y * 0.9f), 0f, layerMask);

            if (hit == null) 
            {
                Instantiate(TowerObjPrefab, worldCenterPos, Quaternion.identity, towersParent.transform);
                TowerObjPrefab = null;
                TowerObjFake.GetComponent<SpriteRenderer>().sprite = null;

                //tower placesssed do!!! 
            }
        }
    }
    
}
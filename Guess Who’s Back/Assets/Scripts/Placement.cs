using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class Placement : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject TowerObjFake;
    public GameObject TowerObjPrefab;

    public Grid grid;
    public LayerMask layerMask;

    void Update()
    {
        //finds what pos the mouse is hovering---
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = grid.WorldToCell(mousePos);

        Vector3 worldCenterPos = grid.GetCellCenterWorld(cellPos);

        TowerObjFake.transform.position = worldCenterPos;

        //places object---
        if (Input.GetMouseButton(0) && TowerObjPrefab != null)
        {
            Collider2D hit = Physics2D.OverlapBox(worldCenterPos, new Vector2(0.9f, 0.9f), 0f, layerMask);

            if (hit == null) 
            {
                Instantiate(TowerObjPrefab, worldCenterPos, Quaternion.identity);
                //TowerObjPrefab = null;


                //tower placesssed do!!! 
            }
        }
    }
    
}
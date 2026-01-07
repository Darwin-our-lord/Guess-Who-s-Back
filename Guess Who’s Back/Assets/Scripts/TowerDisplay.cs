using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDisplay : MonoBehaviour
{
    public Camera mainCamera;

    public LayerMask layerMask;
    public GameObject towerUI;
    public GameObject rangeCircle;
    public Grid grid;

    void Update()
    {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int cellPos = grid.WorldToCell(mousePos);

            Vector3 worldCenterPos = grid.GetCellCenterWorld(cellPos);

            Collider2D hit = Physics2D.OverlapBox(worldCenterPos, new Vector2(0.9f, 0.9f), 0f, layerMask);

            if (hit != null)
            {
                if (hit.gameObject.CompareTag("Tower") || hit.gameObject.CompareTag("Wall"))
                {
                    towerUI.SetActive(true);
                    towerUI.transform.position = mousePos + new Vector2(1.5f, -2);

                    towerUI.transform.GetChild(1).GetComponent<TMP_Text>().text = hit.gameObject.name;
                    towerUI.transform.GetChild(2).GetComponent<TMP_Text>().text = hit.gameObject.GetComponent<Tower>().GetDescription();

                    rangeCircle.SetActive(true);
                    rangeCircle.transform.localScale
                        = new Vector3(hit.gameObject.GetComponent<Tower>().Range * 2, hit.gameObject.GetComponent<Tower>().Range * 2, 1);


                }
            }
            else
            {
                towerUI.SetActive(false);
                rangeCircle.transform.localScale = new Vector3(0,0,1);
            }
        


    }
}

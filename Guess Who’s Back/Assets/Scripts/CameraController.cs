using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed;
    public float panBorderThickness;

    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if (Input.mousePosition.y >= Screen.height - panBorderThickness) { ver =  1; }//top
        if (Input.mousePosition.y <= panBorderThickness)                 { ver = -1; }//bot
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)  { hor =  1; }//right
        if (Input.mousePosition.x <= panBorderThickness)                 { hor = -1; }//left

        transform.position += new Vector3(hor, ver, 0).normalized * cameraSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.T)) transform.position = new Vector3 (0, 0,-10);
        
    }
}

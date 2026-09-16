using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraSpeed; //is set using the settings script at the start of the game
    [SerializeField] float cameraSprintSpeed; //is set using the settings script at the start of the game
    [SerializeField] float panBorderThickness;

    [Header("")]
    [SerializeField] GameObject monoChrome;
    [SerializeField] GameObject reverseColor;
    void Awake()
    {
        if (Settings.monoChrome && monoChrome != null) monoChrome.SetActive(true);
        else if(!Settings.monoChrome && monoChrome != null) monoChrome.SetActive(false);
        if (Settings.reverseColor && reverseColor != null) reverseColor.SetActive(true);
        else if (!Settings.reverseColor && reverseColor != null) reverseColor.SetActive(false);

        cameraSpeed = Settings.cameraPanSpeed;
        cameraSprintSpeed = Settings.cameraSprintSpeed;
    }
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        if (Settings.cameraPan)
        {
            if (Input.mousePosition.y >= Screen.height - panBorderThickness) { ver = 1; }//top
            if (Input.mousePosition.y <= panBorderThickness) { ver = -1; }//bot
            if (Input.mousePosition.x >= Screen.width - panBorderThickness) { hor = 1; }//right
            if (Input.mousePosition.x <= panBorderThickness) { hor = -1; }//left
        }

        if (Input.GetKey(KeyCode.LeftShift)) transform.position += new Vector3(hor, ver, 0).normalized * cameraSprintSpeed * Time.deltaTime; 
        else transform.position += new Vector3(hor, ver, 0).normalized * cameraSpeed * Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.T)) transform.position = new Vector3 (0, 0,-10);


        Camera cam = GetComponent<Camera>();
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            Vector3 mouseWorldBefore = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));

            cam.orthographicSize -= scroll * Settings.cameraZoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 5f, 9f);

            Vector3 mouseWorldAfter = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));

            if (Settings.ZoomInOnMouse)
            {
                transform.position += mouseWorldBefore - mouseWorldAfter;
            }
        }
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed;

    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(hor, ver, 0).normalized * cameraSpeed * Time.deltaTime;

    }
}

using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool cameraPan = false;
    void Awake()
    {
        cameraPan = false;    
    }
    public static void ChangeCameraPan()
    {
        cameraPan = !cameraPan;
    }


}

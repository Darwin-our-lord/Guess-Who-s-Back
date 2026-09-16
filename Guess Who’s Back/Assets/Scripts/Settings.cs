using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("---GamePlay---")]

    [Header("Camera")]
    public static bool cameraPan = false;
    public static float cameraPanSpeed = 7f;
    public static float cameraSprintSpeed = 15f;
    public static bool ZoomInOnMouse = true;
    public static float cameraZoomSpeed = 2f;

    [Header("---Visual---")]

    [Header("")]
    public static bool ShowBullets = true;

    [Header("VisualEffects")]
    public static bool monoChrome = false;
    public static bool reverseColor = false;

    [Header("audio")]
    public static bool BackgroundMusic = true; //does nothing yet

    /*void Awake()
    {
        //gameplay---
        cameraPan = false;

        //visual---
        ShowBullets = true;
        monoChrome = false;
        reverseColor = false;
    }*/

    #region gameplay

    #region Camera
    public static void ChangeCameraPan()
    {
        cameraPan = !cameraPan;
    }
    public static void ChangeZoomInOnMouse()
    {
        ZoomInOnMouse = !ZoomInOnMouse;
    }
    public void ChangeCameraZoomSpeed(float speed)
    {
        cameraZoomSpeed = speed;
    }
    public void ChangeCameraSpeed(float speed)
    {
        cameraPanSpeed = speed;
    }
    public void ChangeCameraSprintSpeed(float speed)
    {
        cameraSprintSpeed = speed;
    }
    #endregion

    #endregion

    #region visual

    public static void ChangeShowBullets()
    {
        ShowBullets = !ShowBullets;
    }

    #region VisualEffects
    public static void ChangeMonoChrome()
    {
        monoChrome = !monoChrome;
    }
    public static void ChangeReverseColor()
    {
        reverseColor = !reverseColor;
    }
    #endregion

    #endregion

}

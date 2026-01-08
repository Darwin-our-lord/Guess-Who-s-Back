using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("GamePlay")]
    public static bool cameraPan = false;

    [Header("Visual")]
    public static bool ShowBullets = true;
    public static bool monoChrome = false;
    public static bool reverseColor = false;

    //[Header("audio")]
    void Awake()
    {
        //gameplay---
        cameraPan = false;

        //visual---
        ShowBullets = true;
        monoChrome = false;
        reverseColor = false;
    }
    #region gameplay

    public static void ChangeCameraPan()
    {
        cameraPan = !cameraPan;
    }

    #endregion

    #region visual

    public static void ChangeShowBullets()
    {
        ShowBullets = !ShowBullets;
    }
    public static void ChangeMonoChrome()
    {
        monoChrome = !monoChrome;
    }
    public static void ChangeReverseColor()
    {
        reverseColor = !reverseColor;
    }

    #endregion

}

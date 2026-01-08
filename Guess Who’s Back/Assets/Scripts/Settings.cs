using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("GamePlay")]
    public static bool cameraPan = false;

    [Header("Visual")]
    public static bool ShowBullets = true;
    public static bool monoChrome = false;

    //[Header("audio")]
    void Awake()
    {
        //gameplay---
        cameraPan = false;

        //visual---
        ShowBullets = true;
        monoChrome = false;
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

    #endregion

}

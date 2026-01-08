using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("GamePlay")]
    public static bool cameraPan = false;

    [Header("Visual")]
    public static bool ShowBullets = true;

    //[Header("audio")]
    void Awake()
    {
        //gameplay---
        cameraPan = false;

        //visual---
        ShowBullets = true;
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

    #endregion

}

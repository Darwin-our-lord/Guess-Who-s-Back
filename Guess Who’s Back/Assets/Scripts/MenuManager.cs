using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainUI;
    public GameObject settingsUI;
    public GameObject storeUI;

    private bool inStore = false;
    //button functions
    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    public void SettingsMenuButton()
    {
        mainUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(true);
    }
    public void BackButton()
    {
        mainUI.gameObject.SetActive(true);
        settingsUI.gameObject.SetActive(false);
    }
    public void StoreButton()
    {
        if (inStore)  storeUI.SetActive(false);  
        else if (!inStore) storeUI.SetActive(true);
        inStore = !inStore;
    }

}

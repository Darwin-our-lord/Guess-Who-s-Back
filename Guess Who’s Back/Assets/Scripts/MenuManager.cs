using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("MainMenu")]
    public GameObject mainUI;
    public GameObject settingsUI;

    public GameObject GameplaySettingsUI;
    public GameObject VisualSettingsUI;
    public GameObject VisualEffectsSettingsUI;
    public GameObject OtherVisualSettingsUI;
    public GameObject AudioSettingsUI;

    [Header("During Game")]
    public GameObject storeUI;
    public GameObject loseUI;

    public StoreManager storeManager;
    public Placement placement;

    private bool inStore = false;
    //button functions
    public void StartButton()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void CreditsButton()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1f;
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
    public void GameplaySettingsMenuButton()
    {
        AudioSettingsUI.gameObject.SetActive(false);
        VisualSettingsUI.gameObject.SetActive(false);

        GameplaySettingsUI.gameObject.SetActive(true);
    }
    public void AudioSettingsMenuButton()
    {
        AudioSettingsUI.gameObject.SetActive(true);

        VisualSettingsUI.gameObject.SetActive(false);
        GameplaySettingsUI.gameObject.SetActive(false);
    }
    public void VisualSettingsMenuButton()
    {
        VisualSettingsUI.gameObject.SetActive(true);

        AudioSettingsUI.gameObject.SetActive(false);
        GameplaySettingsUI.gameObject.SetActive(false);
    }
    public void VisualEffectsMenuButton()
    {
        VisualEffectsSettingsUI.gameObject.SetActive(true);
        OtherVisualSettingsUI.gameObject.SetActive(false);
    }
    public void OtherVisualMenuButton()
    {
        VisualEffectsSettingsUI.gameObject.SetActive(false);
        OtherVisualSettingsUI.gameObject.SetActive(true);
    }
    public void BackButton()
    {
        mainUI.gameObject.SetActive(true);
        settingsUI.gameObject.SetActive(false);
    }
    public void StoreButton()
    {

        if (placement.TowerObjPrefab != null || inStore) storeUI.SetActive(false);
        else if (!inStore) storeUI.SetActive(true);
        inStore = !inStore;
        storeManager.UpdateMoneyUI();
        
    }

}

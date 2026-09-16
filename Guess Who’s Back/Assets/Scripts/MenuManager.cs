using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] GameObject mainUI;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject GameplaySettingsUI;
    [SerializeField] GameObject VisualSettingsUI;
    [SerializeField] GameObject VisualEffectsSettingsUI;
    [SerializeField] GameObject OtherVisualSettingsUI;
    [SerializeField] GameObject AudioSettingsUI;
    [SerializeField] GameObject DiscordLinkUI;

    [Header("During Game")]
    public GameObject storeUI;
    public GameObject loseUI;
    [SerializeField] GameObject pauseUI;
    [Header("")]
    [SerializeField] StoreManager storeManager;
    [SerializeField] Placement placement;

    #region pausemenu

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && storeUI != null && pauseUI != null)
        {
            if (!storeUI.activeSelf)
            {
                if (Time.timeScale == 1f)
                {
                    Time.timeScale = 0f;
                    pauseUI.SetActive(true);
                }
                else if (Time.timeScale == 0f)
                {
                    Time.timeScale = 1f;
                    pauseUI.SetActive(false);

                }
            }
            else
            {
                storeUI.SetActive(false);
            }
        }
    }

    #endregion
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
        DiscordLinkUI.gameObject.SetActive(false);
        GameplaySettingsUI.gameObject.SetActive(true);
    }
    public void AudioSettingsMenuButton()
    {
        AudioSettingsUI.gameObject.SetActive(true);
        VisualSettingsUI.gameObject.SetActive(false);
        GameplaySettingsUI.gameObject.SetActive(false);
        DiscordLinkUI.gameObject.SetActive(false);
    }
    public void VisualSettingsMenuButton()
    {
        VisualSettingsUI.gameObject.SetActive(true);
        AudioSettingsUI.gameObject.SetActive(false);
        GameplaySettingsUI.gameObject.SetActive(false);
        DiscordLinkUI.gameObject.SetActive(false);
    }
    public void DiscordSettingsMenuButton()
    {
        DiscordLinkUI.gameObject.SetActive(true);
        AudioSettingsUI.gameObject.SetActive(false);
        VisualSettingsUI.gameObject.SetActive(false);
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


    #region Only in game

    public void ResumeButton()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
    }
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    public void SurenderButton()
    {
        EnemySpawner enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();

        if (LeaderboardClient.Instance != null)
        {
            LeaderboardClient.Instance.SubmitScore(enemySpawner.wave, "Surrender");
        }

        Time.timeScale = 0f;
        loseUI.SetActive(true);
        loseUI.transform.GetChild(1).GetComponent<TMP_Text>().text = "you made it to wave: " + enemySpawner.wave;
        pauseUI.SetActive(false);
    }
    public void StoreButton()
    {
        if (placement.TowerObjPrefab != null)
        {
            placement.CancelPlacement();
            storeUI.SetActive(true);
        }

        else if (storeUI.activeSelf) storeUI.SetActive(false);
        else if (!storeUI.activeSelf) storeUI.SetActive(true);

        storeManager.UpdateMoneyUI();
    }


    #endregion
}
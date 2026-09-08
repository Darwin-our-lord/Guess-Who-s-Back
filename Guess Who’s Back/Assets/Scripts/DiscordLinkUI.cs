using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscordLinkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button linkButton;
    [SerializeField] private TMP_Text statusText;

    private void Awake()
    {
        linkButton.onClick.AddListener(OnLinkButtonPressed);
    }

    private void OnLinkButtonPressed()
    {
        var code = codeInput.text.Trim();
        if (string.IsNullOrEmpty(code))
        {
            statusText.text = "Enter the code from /linkgame first.";
            return;
        }

        linkButton.interactable = false;
        statusText.text = "Linking...";

        LeaderboardClient.Instance.LinkDiscord(code, (success, message) =>
        {
            statusText.text = success ? message : $"Failed: {message}";
            linkButton.interactable = true;
        });
    }
}
using UnityEngine;

public static class PlayerIdentity
{
    private const string DiscordIdKey = "LinkedDiscordId";
    private const string NameKey = "PlayerName";

    public static string LinkedDiscordId
    {
        get => PlayerPrefs.GetString(DiscordIdKey, "");
        set
        {
            PlayerPrefs.SetString(DiscordIdKey, value ?? "");
            PlayerPrefs.Save();
        }
    }

    public static bool IsLinked => !string.IsNullOrEmpty(LinkedDiscordId);

    public static string DisplayName
    {
        get => PlayerPrefs.GetString(NameKey, "Player");
        set
        {
            PlayerPrefs.SetString(NameKey, value);
            PlayerPrefs.Save();
        }
    }

    public static void ClearLink()
    {
        PlayerPrefs.DeleteKey(DiscordIdKey);
        PlayerPrefs.Save();
    }
}
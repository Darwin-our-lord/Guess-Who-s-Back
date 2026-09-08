using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class LeaderboardClient : MonoBehaviour
{
    public static LeaderboardClient Instance { get; private set; }

    [Header("Backend")]
    [Tooltip("e.g. https://your-app.up.railway.app")]
    [SerializeField] private string backendBaseUrl = "https://your-backend-url.example.com";
    [SerializeField] private string apiKey = "change-this-to-something-random";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SubmitScore(int waveReached, string killedBy = null)
    {
        var discordIdJson = PlayerIdentity.IsLinked ? $"\"{Escape(PlayerIdentity.LinkedDiscordId)}\"" : "null";
        var killedByJson = string.IsNullOrEmpty(killedBy) ? "null" : $"\"{Escape(killedBy)}\"";
        var body = "{\"discordId\":" + discordIdJson + "," +
                    "\"wave\":" + waveReached + "," +
                    "\"name\":\"" + Escape(PlayerIdentity.DisplayName) + "\"," +
                    "\"killedBy\":" + killedByJson + "}";

        StartCoroutine(PostJson(
            "/api/score",
            body,
            onSuccess: _ => Debug.Log($"[Leaderboard] Score submitted: wave {waveReached}"),
            onError: err => Debug.LogWarning($"[Leaderboard] Score submit failed: {err}")
        ));
    }
    public void LinkDiscord(string code, Action<bool, string> onComplete)
    {
        StartCoroutine(PostJson(
            "/api/link",
            $"{{\"code\":\"{Escape(code)}\"}}",
            onSuccess: body =>
            {
                try
                {
                    var parsed = JsonUtility.FromJson<LinkResponse>(body);
                    PlayerIdentity.LinkedDiscordId = parsed.discordId;
                    onComplete?.Invoke(true, $"Linked as {parsed.discordUsername}!");
                }
                catch
                {
                    onComplete?.Invoke(false, "Linked, but couldn't read the response.");
                }
            },
            onError: err => onComplete?.Invoke(false, err)
        ));
    }

    [Serializable]
    private class LinkResponse
    {
        public bool ok;
        public string discordId;
        public string discordUsername;
    }

    private System.Collections.IEnumerator PostJson(
        string path, string jsonBody, Action<string> onSuccess, Action<string> onError)
    {
        var url = backendBaseUrl.TrimEnd('/') + path;
        var bodyBytes = Encoding.UTF8.GetBytes(jsonBody);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            var errorText = string.IsNullOrEmpty(request.downloadHandler.text)
                ? request.error
                : request.downloadHandler.text;
            onError?.Invoke(errorText);
        }
        else
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
    }

    private static string Escape(string s) => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
}
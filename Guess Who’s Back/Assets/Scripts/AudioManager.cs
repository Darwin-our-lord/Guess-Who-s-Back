using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private List<AudioClip> playlist = new List<AudioClip>();

    [SerializeField] private bool shufflePlaylist = false;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;

    private List<int> playOrder = new List<int>();
    private int playOrderIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = false;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
    }

    private void Start()
    {
        if (playlist.Count > 0)
        {
            BuildPlayOrder();
            PlayTrackAtOrderIndex(0);
        }
    }

    private void Update()
    {
        if (playlist.Count > 0 && musicSource.clip != null && !musicSource.isPlaying)
        {
            PlayNext();
        }
    }

    private void BuildPlayOrder()
    {
        playOrder.Clear();
        for (int i = 0; i < playlist.Count; i++) playOrder.Add(i);

        if (shufflePlaylist)
        {
            for (int i = playOrder.Count - 1; i > 0; i--)
            {
                int swap = Random.Range(0, i + 1);
                (playOrder[i], playOrder[swap]) = (playOrder[swap], playOrder[i]);
            }
        }

        playOrderIndex = 0;
    }

    private void PlayTrackAtOrderIndex(int index)
    {
        if (playlist.Count == 0) return;

        playOrderIndex = index;
        int trackIndex = playOrder[playOrderIndex];
        musicSource.clip = playlist[trackIndex];
        musicSource.Play();
    }

    public void PlayNext()
    {
        if (playlist.Count == 0) return;

        int nextOrderIndex = playOrderIndex + 1;

        if (nextOrderIndex >= playOrder.Count)
        {
            if (shufflePlaylist) BuildPlayOrder();
            nextOrderIndex = 0;
        }

        PlayTrackAtOrderIndex(nextOrderIndex);
    }

    public void PlayPrevious()
    {
        if (playlist.Count == 0) return;

        int prevOrderIndex = playOrderIndex - 1;
        if (prevOrderIndex < 0) prevOrderIndex = playOrder.Count - 1;

        PlayTrackAtOrderIndex(prevOrderIndex);
    }
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.loop = true;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void ResumePlaylist()
    {
        musicSource.loop = false;
        if (playlist.Count > 0) PlayTrackAtOrderIndex(playOrderIndex);
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public float GetVolume() => musicVolume;

    public void ToggleMute()
    {
        musicSource.mute = !musicSource.mute;
    }

    public bool IsMuted => musicSource.mute;
}
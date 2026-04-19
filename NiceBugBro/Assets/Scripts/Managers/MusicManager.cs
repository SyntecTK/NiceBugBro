using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Header("Loop Settings")]
    [Tooltip("If true, the music will loop back to the Loop Point Time instead of the beginning.")]
    [SerializeField] private bool useLoopPoint = false;
    [SerializeField] private float loopPointTime = 0f;

    private AudioSource musicSource;
    private float lastPlayTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f; 
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }


    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        backgroundMusic = clip;
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
        lastPlayTime = 0f;
    }
    private void Update()
    {
        if (musicSource != null && musicSource.isPlaying && useLoopPoint)
        {
            float currentTime = musicSource.time;

            if (currentTime < lastPlayTime)
            {
                musicSource.time = loopPointTime + currentTime;
            }

            lastPlayTime = musicSource.time;
        }
    }


    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }


    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    private void OnValidate()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
}

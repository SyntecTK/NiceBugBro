using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundGroup
{
    public SoundType soundType;
    public AudioClip[] clips;
    
    [Range(0f, 1f)] 
    public float volume = 1f;

    [Header("Random Pitch Variation")]
    [Range(0.1f, 2f)] public float minPitch = 0.9f;
    [Range(0.1f, 2f)] public float maxPitch = 1.1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private SoundGroup[] soundGroups;
    [SerializeField] private AudioSource globalAudioSource;

    [Header("3D Sound Settings")]
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
    [SerializeField] private float minDistance3D = 2f;
    [SerializeField] private float maxDistance3D = 20f;

    private Dictionary<SoundType, SoundGroup> soundDictionary;

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

        soundDictionary = new Dictionary<SoundType, SoundGroup>();
        foreach (var group in soundGroups)
        {
            soundDictionary[group.soundType] = group;
        }
    }

    /// <summary>
    /// Play a 2D sound that isn't attached to a world position (e.g. Powerup, Game Over, Player Damage).
    /// </summary>
    public void Play2DSound(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out SoundGroup group))
        {
            if (group.clips == null || group.clips.Length == 0) return;

            AudioClip clip = group.clips[Random.Range(0, group.clips.Length)];
            
            globalAudioSource.pitch = Random.Range(group.minPitch, group.maxPitch);
            globalAudioSource.PlayOneShot(clip, group.volume);
            Debug.Log(clip.name);
        }
        else
        {
            Debug.LogWarning($"Sound {type} not found!");
        }
    }

    /// <summary>
    /// Play a 3D sound at a specific position (e.g. Enemy shots, Footsteps).
    /// </summary>
    public void Play3DSound(SoundType type, Vector3 position)
    {
        if (soundDictionary.TryGetValue(type, out SoundGroup group))
        {
            if (group.clips == null || group.clips.Length == 0) return;

            AudioClip clip = group.clips[Random.Range(0, group.clips.Length)];

            GameObject tempAudioHost = new GameObject("TempAudio_" + type);
            tempAudioHost.transform.position = position;
            
            AudioSource source = tempAudioHost.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = group.volume;
            source.pitch = Random.Range(group.minPitch, group.maxPitch);
            
            source.spatialBlend = 1f; 
            source.rolloffMode = rolloffMode;
            source.minDistance = minDistance3D;
            source.maxDistance = maxDistance3D;

            source.Play();

            Destroy(tempAudioHost, clip.length + 0.1f);
        }
        else
        {
            Debug.LogWarning($"Sound {type} not found!");
        }
    }
}

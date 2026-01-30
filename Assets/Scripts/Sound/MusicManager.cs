using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource _audioSource;
    private Dictionary<string, AudioClip> _clipByName;
    [SerializeField] private List<AudioClip> _musicList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        _clipByName = new Dictionary<string, AudioClip>();

        foreach (var clip in _musicList)
        {
            if (clip == null)
                continue;

            if (_clipByName.ContainsKey(clip.name))
            {
                Debug.LogWarning($"Duplicate AudioClip name: {clip.name}");
                continue;
            }

            _clipByName.Add(clip.name, clip);
        }
    }

    public void PlayMusic(string musicName)
    {
        if (_clipByName.TryGetValue(musicName, out var clip))
        {
            if (_audioSource.clip == clip && _audioSource.isPlaying)
                return;

            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music '{musicName}' not found");
        }
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void PauseMusic()
    {
        _audioSource.Pause();
    }

    public void ResumeMusic()
    {
        _audioSource.UnPause();
    }
}

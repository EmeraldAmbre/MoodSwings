using UnityEngine;

public class MasterVolumeManager : MonoBehaviour
{
    public static MasterVolumeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolume();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public float GetMasterVolume()
    {
        return AudioListener.volume;
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = volume;
    }
}

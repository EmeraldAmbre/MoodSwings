using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;

    private void Start()
    {
        _volumeSlider.value = MasterVolumeManager.Instance.GetMasterVolume();
    }

    public void OnVolumeChanged(float value)
    {
        MasterVolumeManager.Instance.SetMasterVolume(value);
    }
}

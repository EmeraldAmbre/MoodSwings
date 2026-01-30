using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private string _musicName;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(_musicName);
    }
}

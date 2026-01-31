using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionsPanel;

    private void Start()
    {
        _optionsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void OpenOptions()
    {
        _optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        _optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

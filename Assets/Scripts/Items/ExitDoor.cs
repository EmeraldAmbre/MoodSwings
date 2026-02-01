using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [Header("Level Transition")]
    [SerializeField] private int _nextSceneIndex;
    [SerializeField] private float _delayBeforeLoad = 0.3f;

    private bool _isTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        _isTriggered = true;
        StartCoroutine(LoadNextLevel());
    }

    private System.Collections.IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(_delayBeforeLoad);

        SceneManager.LoadScene(_nextSceneIndex);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private float _blinkDuration = 0.5f;

    private SpriteRenderer _spriteRenderer;

    public bool IsPlayerDead;

    public event System.Action OnPlayerDied;

    private void Awake()
    {
        IsPlayerDead = false;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Instance = this;
    }

    private void Update()
    {
        if (IsPlayerDead)
            return;
    }

    private System.Collections.IEnumerator Blink()
    {
        float duration = _blinkDuration;
        float interval = 0.1f;
        float t = 0f;

        while (t < duration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(interval);
            t += interval;
        }

        _spriteRenderer.enabled = true;
    }

    private void Die()
    {
        IsPlayerDead = true;
        Time.timeScale = 0f;
        OnPlayerDied?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

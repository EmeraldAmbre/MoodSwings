using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private float _blinkDuration = 1.5f;
    [SerializeField] private float _blinkInterval = 0.1f;
    [SerializeField] private Transform _spawnPoint;


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

        _spriteRenderer = GetComponent<SpriteRenderer>();

        Instance = this;
    }

    private void Start()
    {
        if (_spawnPoint != null)
        {
            transform.position = _spawnPoint.position;
        }
    }


    private void Update()
    {
        if (IsPlayerDead)
            return;
    }

    private System.Collections.IEnumerator Blink()
    {
        float t = 0f;

        while (t < _blinkDuration)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(_blinkInterval);
            t += _blinkInterval;
        }

        _spriteRenderer.enabled = true;
    }

    public void StartDeathSequence()
    {
        if (IsPlayerDead)
            return;

        StartCoroutine(DeathRoutine());
    }

    private System.Collections.IEnumerator DeathRoutine()
    {
        IsPlayerDead = true;

        // Blink during _blinkDuration time
        yield return StartCoroutine(Blink());
        Respawn();
    }

    private void Respawn()
    {
        transform.position = _spawnPoint.position;
        _spriteRenderer.enabled = true;
        IsPlayerDead = false;
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

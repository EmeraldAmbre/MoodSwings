using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    [SerializeField] private Transform _spawnPoint;

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

    public void StartDeathSequence()
    {
        if (IsPlayerDead)
            return;
        
        Death();
    }

    private void Death()
    {
        IsPlayerDead = true;
        Respawn();
    }

    private void Respawn()
    {
        transform.position = _spawnPoint.position;
        IsPlayerDead = false;
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

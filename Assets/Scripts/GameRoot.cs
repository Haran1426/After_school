using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }

    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PoolManager poolManager;

    public GameManager Game => gameManager;
    public AudioManager Audio => audioManager;
    public PoolManager Pool => poolManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    void InitializeManagers()
    {
        gameManager.Initialize();
        audioManager.Initialize();
        poolManager.Initialize();
    }
}
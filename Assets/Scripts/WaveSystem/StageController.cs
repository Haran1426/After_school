using UnityEngine;

public sealed class StageController : MonoBehaviour
{
    [SerializeField] private StageMap stageMap;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WaveManager waveManager;

    public StageMap CurrentStage => stageMap;

    private void Awake()
    {
        ApplyStage();
    }

    private void Start()
    {
        ApplyStage();
    }

    public void SetStage(StageMap nextStageMap)
    {
        stageMap = nextStageMap;
        ApplyStage();
    }

    private void ApplyStage()
    {
        if (stageMap == null)
            return;

        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<PlayerMovement>();

        if (waveManager == null)
            waveManager = FindAnyObjectByType<WaveManager>();

        if (playerMovement != null)
            playerMovement.ConfigureMovementBounds(stageMap.UsesMovementBounds, stageMap.BoundsMin, stageMap.BoundsMax);

        if (waveManager != null)
            waveManager.Configure(stageMap);
    }
}

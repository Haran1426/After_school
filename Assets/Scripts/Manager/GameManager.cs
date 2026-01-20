using UnityEngine;

public class GameManager : ManagerBase<GameManager>, IManagerInitialize
{
    public bool IsGameOver { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;
        Time.timeScale = 0f;
    }
}

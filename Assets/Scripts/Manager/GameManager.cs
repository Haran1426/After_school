using UnityEngine;

public class GameManager : ManagerBase
{
    public bool IsGameOver { get; private set; }

    protected override void OnInitialize()
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
using System;
using UnityEngine;

public class GameManager : ManagerBase
{
    public bool IsGameOver { get; private set; }
    public int KillCount { get; private set; }
    public float SurvivedTime { get; private set; }

    public event Action OnGameOver;

    protected override void OnInitialize()
    {
        IsGameOver = false;
        KillCount = 0;
        SurvivedTime = 0f;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!IsGameOver)
            SurvivedTime += Time.deltaTime;
    }

    public void RegisterKill()
    {
        KillCount++;
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;
        Time.timeScale = 0f;

        OnGameOver?.Invoke();
    }
}
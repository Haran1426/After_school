using System;
using UnityEngine;

public class GameManager : ManagerBase
{
    public bool IsGameOver { get; private set; }
    public bool IsStageCleared { get; private set; }
    public int KillCount { get; private set; }
    public float SurvivedTime { get; private set; }

    public event Action OnGameOver;
    public event Action OnStageCleared;

    protected override void OnInitialize()
    {
        IsGameOver = false;
        IsStageCleared = false;
        KillCount = 0;
        SurvivedTime = 0f;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!IsGameOver && !IsStageCleared)
            SurvivedTime += Time.deltaTime;
    }

    public void RegisterKill()
    {
        KillCount++;
    }

    public void GameOver()
    {
        if (IsGameOver || IsStageCleared)
            return;

        IsGameOver = true;
        Time.timeScale = 0f;

        GameRoot.Instance.Audio.PlaySfx(AudioCue.GameOver);
        OnGameOver?.Invoke();
    }

    public void StageClear()
    {
        if (IsGameOver || IsStageCleared)
            return;

        IsStageCleared = true;
        Time.timeScale = 0f;

        GameRoot.Instance.Audio.PlaySfx(AudioCue.LevelUp);
        OnStageCleared?.Invoke();
    }
}

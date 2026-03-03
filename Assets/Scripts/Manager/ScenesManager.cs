using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneId
{
    Title,
    Lobby,
    Game,
    Result
}


[Serializable]
public class SceneEntry
{
    public SceneId id;
    public string sceneName;
}


public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    [SerializeField] private List<SceneEntry> scenes = new();

    private Dictionary<SceneId, string> sceneMap;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sceneMap = new Dictionary<SceneId, string>();

        for (int i = 0; i < scenes.Count; i++)
            sceneMap.Add(scenes[i].id, scenes[i].sceneName);
    }

    public void Load(SceneId id)
    {
        if (!sceneMap.TryGetValue(id, out var name))
        {
            Debug.LogError("씬이 없습니다: " + id);
            return;
        }

        SceneManager.LoadScene(name);
    }

    public void ReloadCurrent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
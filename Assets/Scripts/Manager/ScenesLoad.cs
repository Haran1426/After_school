using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesLoad : MonoBehaviour
{
    public void LoadRoom()
    {
        SceneManager.LoadScene("RoomScene");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void LoadCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelectScene");
    }
    public void LoadCoice()
    {
        SceneManager.LoadScene("CoiceScene");
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

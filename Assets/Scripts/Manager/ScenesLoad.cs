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
    public void LoadChoice()
    {
        SceneManager.LoadScene("ChoiceScene");
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}

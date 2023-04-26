using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LevelManagerMain : MonoBehaviour
{
    public TMP_Text PlayerName;
    public TMP_Text PlayerNameError;
    public string sceneToLoad = "GameLobby";

    public void Start()
    {
        PlayerNameError.enabled = false;
    }


    public void LoadGameLobby()
    {
        if (PlayerName.text.Length <= 1)
        {
            PlayerNameError.enabled = true;
        }
        else
        {
            PlayerPrefs.SetString("NAME", PlayerName.text);
            PlayerNameError.enabled = false;
            SceneManager.LoadScene(sceneToLoad);

        }
    }
}

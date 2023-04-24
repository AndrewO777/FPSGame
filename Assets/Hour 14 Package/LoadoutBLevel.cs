using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutBLevel : MonoBehaviour
{
    public string sceneToLoad = "GameLobby";


    public void ChangeLoadout()
    {
        PlayerPrefs.SetString("LOADOUT", "SHOTGUN");

        SceneManager.LoadScene(sceneToLoad);
    }
}

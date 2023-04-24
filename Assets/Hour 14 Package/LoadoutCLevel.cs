using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadoutCLevel : MonoBehaviour
{
    public string sceneToLoad = "GameLobby";


    public void ChangeLoadout()
    {
        PlayerPrefs.SetString("LOADOUT", "ASSAULT");

        SceneManager.LoadScene(sceneToLoad);
    }
}

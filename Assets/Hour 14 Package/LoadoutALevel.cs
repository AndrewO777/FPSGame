using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadoutALevel : MonoBehaviour
{
    public string sceneToLoad = "GameLobby";


    public void ChangeLoadout()
    {
        PlayerPrefs.SetString("LOADOUT", "PISTOL");

        SceneManager.LoadScene(sceneToLoad);
    }
}

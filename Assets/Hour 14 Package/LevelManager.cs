using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LevelManager : MonoBehaviour
{
    public string sceneToLoad = "GameLobby";
 
    public void LoadGame ()
	{
		SceneManager.LoadScene(sceneToLoad);
	}


}

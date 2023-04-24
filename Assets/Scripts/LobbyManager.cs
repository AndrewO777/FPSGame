using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LobbyManager : MonoBehaviour
{
    public TMP_Text Player1Name;
    public TMP_Text Player2Name;
    public TMP_Text Player3Name;
    public TMP_Text Player4Name;

    // Start is called before the first frame update
    void Start()
    {
        Player1Name.text = PlayerPrefs.GetString("NAME");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

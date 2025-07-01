using System.Collections;
using System.Collections.Generic;
using BlacXGames.PersistSuite.PlayerPrefs;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefsAPI.Player.FullName = "TAIMOOR";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

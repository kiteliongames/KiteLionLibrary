using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainOnline : MonoBehaviour
{
    PhotonArenaManager _PM;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("CBUG_ON", 1);

        _PM = PhotonArenaManager.Instance;
        
        _PM.ConnectAndJoinRoom("Player", null);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

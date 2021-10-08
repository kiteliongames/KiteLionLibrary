using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiteLion.Common;
public class RunGameOnline : MonoBehaviour
{
    PhotonArenaManager _PM;
    public GameObject PlayerSpawn;
    public GameObject FooblesSpawn;
    private bool isSpawned;

    private GameObject player;

    bool _isFooblesSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        _PM = PhotonArenaManager.Instance;
        isSpawned = false;
        //Forces.G = GravityForce; //todo improve "forces" layout
    }

    // Update is called once per frame
    void Update() {
        if (_PM.CurrentServerUserDepth == PhotonArenaManager.ServerDepthLevel.InRoom && isSpawned == false) {

            player = _PM.SpawnPlayer(PlayerSpawn.transform.position, PlayerSpawn.transform.rotation, "Character 3D Online");
            isSpawned = true;
        }

        if (_PM.CurrentServerUserDepth == PhotonArenaManager.ServerDepthLevel.InRoom && _isFooblesSpawned == false) {
            if(_PM.GetData("_IsFooblesSpawned") == null || ((bool)_PM.GetData("_IsFooblesSpawned") == false)) {

                _PM.SpawnObject("FoobleOnline", FooblesSpawn.transform.position, FooblesSpawn.transform.rotation);
                _PM.SaveData("_IsFooblesSpawned", true);
                _isFooblesSpawned = true;
            }
        }

//        if (Photon.Pun.PunRP)
//      {

            //    }
    }
}

using KiteLionGames.KiteLionLibrary.Networking.PUN2.Scripts.PhotonArena.Scripts;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Networking.PUN2.Scripts.OnlineMultiplayer.Scripts
{
    public class MainPAM : MonoBehaviour
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
}

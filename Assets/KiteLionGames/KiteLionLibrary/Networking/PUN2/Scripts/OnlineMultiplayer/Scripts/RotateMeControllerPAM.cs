using KiteLionGames.KiteLionLibrary.Networking.PUN2.Scripts.PhotonArena.Scripts;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using Photon.Pun;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Networking.PUN2.Scripts.OnlineMultiplayer.Scripts
{
    public class RotateMeControllerPAM : MonoBehaviour
    {

        public float RotateSpeed = 0.5f;
        public float ForwardSpeed = 3;

        float _sideInput = 0;
        float _forwardInput = 0;

        PhotonArenaManager _PM;

        // Use this for initialization
        void Start()
        {
            CBUG.Do("Online Character Spawned!");
            _PM = PhotonArenaManager.Instance;

            if(GetComponent<PhotonView>().IsMine == false)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<PhotonView>().IsMine == false) {
                return;
            }

            _sideInput = Input.GetAxis("Horizontal");
            _forwardInput = Input.GetAxis("Vertical");
        }

        private void FixedUpdate()
        {
            if (GetComponent<PhotonView>().IsMine == false) {
                return;
            }

            GetComponent<Rigidbody>().isKinematic = true;
            Vector3 previousRotation = new(0, GetComponent<Rigidbody>().rotation.eulerAngles.y);
            Vector3 newRotation = new(0, previousRotation.y + _sideInput * RotateSpeed);

            GetComponent<Rigidbody>().rotation = Quaternion.Euler(newRotation);
            GetComponent<Rigidbody>().velocity = transform.forward * ForwardSpeed * _forwardInput;
            GetComponent<Rigidbody>().isKinematic = false;

        }
    }
}

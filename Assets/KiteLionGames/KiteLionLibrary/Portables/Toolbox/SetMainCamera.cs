using UnityEngine;

namespace KiteLionGames
{
    namespace Common
    {
        public class SetMainCamera : MonoBehaviour
        {

            public Camera MainCamera;

            // Update is called once per frame
            void Update()
            {
                if (MainCamera == null)
                {
                    MainCamera = Camera.main;
                    GetComponent<Canvas>().worldCamera = MainCamera;
                }
            }
        }
    }
}
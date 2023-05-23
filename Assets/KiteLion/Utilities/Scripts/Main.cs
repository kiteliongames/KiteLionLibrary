using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KiteLionGames
{
    public class Main : MonoBehaviour
    {

        public enum Scenes
        {
            Boot,
            Splash,
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Game ... start!");
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }   
        }
    }
}

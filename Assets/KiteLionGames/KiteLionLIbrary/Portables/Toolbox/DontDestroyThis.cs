using System.Collections.Generic;
using UnityEngine;

namespace KiteLionGames.Utilities
{
    public class DontDestroyThis : MonoBehaviour
    {
        public static List<GameObject> List = new();
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

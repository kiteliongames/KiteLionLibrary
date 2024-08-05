using UnityEngine;
using UnityEngine.UI;

namespace KiteLionGames.Utilities
{
    public class Version : MonoBehaviour
    {

        public Text[] AppendTo;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var text in AppendTo)
            {
                text.text = text.text + " " + Application.version;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

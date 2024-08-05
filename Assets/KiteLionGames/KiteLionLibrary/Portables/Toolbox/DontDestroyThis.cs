using System.Collections.Generic;
using UnityEngine;
using System.Dynamic;

namespace KiteLionGames.Utilities
{
    public class DontDestroyThis : MonoBehaviour
    {
        public static dynamic MyEnums {get {return _myEnums;} }
        private static dynamic _myEnums = new ExpandoObject();

        public static List<GameObject> List = new();
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void UseExpandoObject()
        {
            dynamic person = new ExpandoObject();
            person.Name = "David";
            person.Age = 40;
            person.Address = "789 Maple St";

            Debug.Log($"Name: {person.Name}, Age: {person.Age}, Address: {person.Address}");
        }
    }
}

#region FileHeader

// test
// Project: Assembly-CSharp
// File:    DontDestroyThis.cs
// Author:  Eliot CS
// Created: 2024.09.18.02.09.56
// Edited: 2024.09.19.01.09.09
//
// Copyright (c) 2024 SomeGameDevs, LLC. All rights reserved.
//
// This source code is the property of SomeGameDevs, LLC and may not be
// copied, distributed, modified, or used in any way without prior written
// permission from SomeGameDevs, LLC.
//
// Description:
// [Provide a brief description of what this file/class does.]
//
// Previous Header (if any):
//
// License:
// This code is provided "as is," without warranty of any kind, express or
// implied, including but not limited to the warranties of merchantability,
// fitness for a particular purpose, and noninfringement. In no event shall
// the authors or copyright holders be liable for any claim, damages, or
// other liability, whether in an action of contract, tort, or otherwise,
// arising from, out of, or in connection with the software or the use or
// other dealings in the software.

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    public class DontDestroyThis : MonoBehaviour
    {
        //todo remember what i was testing w this random code in DontDestroyThis.cs ...
        // public static dynamic MyEnums {get {return _myEnums;} }
        // private static dynamic _myEnums = new ExpandoObject();

        /// <summary>
        ///     todo: manage accessing of this list
        /// </summary>
        public static List<GameObject> List = new List<GameObject>();
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // public void UseExpandoObject()
        // {
        //     dynamic person = new ExpandoObject();
        //     person.Name = "David";
        //     person.Age = 40;
        //     person.Address = "789 Maple St";
        //
        //     Debug.Log($"Name: {person.Name}, Age: {person.Age}, Address: {person.Address}");
        // }
    }
}

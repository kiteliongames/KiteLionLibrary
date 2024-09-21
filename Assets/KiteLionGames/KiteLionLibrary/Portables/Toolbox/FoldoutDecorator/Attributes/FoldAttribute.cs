#region FileHeader

// test
// Project: Assembly-CSharp
// File:    FoldAttribute.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.58
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

//  Project : UNITY FOLDOUT
// Contacts : Pix - ask@pixeye.games


using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox.FoldoutDecorator.Attributes
{
    public class FoldoutAttribute : PropertyAttribute
    {
        public bool foldEverything;
        public string name;

        /// <summary>Adds the property to the specified foldout group.</summary>
        /// <param name="name">Name of the foldout group.</param>
        /// <param name="foldEverything">Toggle to put all properties to the specified group</param>
        public FoldoutAttribute(string name, bool foldEverything = false)
        {
            this.foldEverything = foldEverything;
            this.name = name;
        }
    }
}

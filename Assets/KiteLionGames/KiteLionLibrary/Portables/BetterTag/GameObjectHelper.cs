#region FileHeader

// test
// Project: Assembly-CSharp
// File:    GameObjectHelper.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.19
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

using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.BetterTag
{
    public static class GameObjectHelper
    {
        public static GameObject FindGameObjectWithTag(Tag.label label)
        {
            return Tag.FindGameObjectWithBetterTag(label);
        }

        public static GameObject[] FindGameObjectsWithTag(Tag.label label)
        {
            return Tag.FindGameObjectsWithBetterTag(label);
        }
    }
}

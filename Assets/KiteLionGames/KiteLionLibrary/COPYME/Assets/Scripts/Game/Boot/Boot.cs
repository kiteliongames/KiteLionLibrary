#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Boot.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
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

using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using KiteLionGames.KiteLionLibrary.Portables.Toolbox;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.COPYME.Assets.Scripts.Game.Boot
{
    namespace Toolbox
    {
        public class Boot : MonoBehaviour
        {
            public static bool IsBooted;
            public Main.Scenes FirstScene;

            protected void Start()
            {
                CBUG.LogToFile("Booting ...");
                CBUG.LogToFile("Success!");

                IsBooted = true;
                // load first scene delayed

                new SceneLoader(FirstScene, false, false, 0.1f).DoLoad();
            }
        }
    }
}

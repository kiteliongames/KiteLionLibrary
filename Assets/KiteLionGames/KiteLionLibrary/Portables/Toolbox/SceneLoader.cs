#region FileHeader

// test
// Project: Assembly-CSharp
// File:    LoadScene.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.55
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

using System;
using KiteLionGames.KiteLionLibrary.COPYME.Assets.Scripts.Game;
using KiteLionGames.KiteLionLibrary.Portables.Utilities;
using UnityEngine.SceneManagement;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    /// <summary>
    ///     Easy simple scene loading class.
    /// </summary>
    public class SceneLoader
    {
        public bool IfExistsThenReload;
        public bool LoadAdditive;
        public float LoadDelay;
        public Main.Scenes SceneToLoad;

        public SceneLoader(Main.Scenes sceneToLoad, bool reloadIfExists = false, bool loadAdditive = false, float loadDelay = 0f)
        {
            SceneToLoad = sceneToLoad;
            IfExistsThenReload = reloadIfExists;
            LoadAdditive = loadAdditive;
            LoadDelay = loadDelay;
        }

        public void DoLoad()
        {
            Tools.DelayFunction(LoadHelper, LoadDelay);
        }

        private void LoadHelper()
        {
            var sceneLoaded = SceneManager.GetSceneByName(SceneToLoad.ToString()).isLoaded;
            var doSceneLoad = sceneLoaded && IfExistsThenReload || sceneLoaded == false;

            if (doSceneLoad)
            {
                SceneManager.LoadScene(SceneToLoad.ToString(), LoadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            }
            else
            {
                throw new Exception("Scene " + SceneToLoad + " is already loaded.");
            }
        }
    }
}

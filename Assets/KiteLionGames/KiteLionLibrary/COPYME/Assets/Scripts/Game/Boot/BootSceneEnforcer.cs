#region FileHeader

// test
// Project: Assembly-CSharp
// File:    BootSceneEnforcer.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.21
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
using UnityEngine.SceneManagement;

namespace KiteLionGames.KiteLionLibrary.COPYME.Assets.Scripts.Game.Boot
{
    /// <summary>
    ///     Enforces a scene to load when you hit play on active scene. Boot Scene Enforcer will enforce the other scene.
    ///     Disable via BootSceneEnforcer.CanEnforce = false;
    /// </summary>
    public static class BootSceneEnforcer
    {
        public static bool CanEnforce { get; } = true;
        /// <summary>
        ///     Scene to load when you hit play on active scene. Boot Scene Enforcer will enforce the other scene.
        /// </summary>
        public static Dictionary<string, string> ActiveSceneToBootScene { get; } = new Dictionary<string, string>
        {
            { 
                Main.Scenes.Splash.ToString(), Main.Scenes.Boot.ToString() 
            },
            { 
                Main.Scenes.Main.ToString(), Main.Scenes.Boot.ToString()
            },
        };

        /// <summary>
        ///     This is the Unity 'hack' to run a method before the scene loads. This will enforce the scene to load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnBeforeSceneLoad()
        {
            var activeScene = SceneManager.GetActiveScene();
            var enforceableScene = ActiveSceneToBootScene.TryGetValue(activeScene.name, out var sceneToLoad);
            if (enforceableScene)
            {
                if (Toolbox.Boot.IsBooted == false && CanEnforce)
                {
                    Main.LoadSceneBuildAgnostic(sceneToLoad, LoadSceneMode.Single);
                }
            }
        }
    }
}


/*
 *     public class BootSceneEnforcer : MonoBehaviour
    {
        [HideInInspector]
        public bool forceBootScene;
        [ReadOnly, HideInInspector]
        public string[] BootScenes;
        //[HideInInspector]
        public int targetBootSceneIndexx = 3;
        public static string TargetBootSceneName;
        public static readonly string SavedDataKey = "BootEnforcerTargetScene";

        protected void Start()
        {
            if (Boot.IsBooted)
            {
                Destroy(this);
                return;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected static void OnBeforeSceneLoad()
        {
            GameSaveLoad.Initialize();
            GameSaveLoad.LoadGameDataFromDisk();
            var byteArray = GameSaveLoad.GetData(SavedDataKey);
            var charArray = new char[byteArray.Length];
            for (int i = 0; i < byteArray.Length; i++)
            {
                charArray[i] = (char)byteArray[i];
            }
            TargetBootSceneName = new string(charArray);

            if(TargetBootSceneName == null || TargetBootSceneName.Length == 0)
            {
                return;
            }
            else
            {
                if (Boot.IsBooted == false)
                    SceneManager.LoadScene(TargetBootSceneName, LoadSceneMode.Single);
            }
        }

#if UNITY_EDITOR
        public void OnValidoot()
        {
            Debug.Log("Target Boot Scene: fromONVALID" + targetBootSceneIndexx);
            if (BootScenes == null)
                return;

            if (targetBootSceneIndexx >= BootScenes.Length)
                targetBootSceneIndexx = 0;

            TargetBootSceneName = BootScenes[targetBootSceneIndexx];

            GameSaveLoad.Initialize();
            //convert int to byte array
            var intByteArray = BitConverter.GetBytes(targetBootSceneIndexx);
            GameSaveLoad.Save(SavedDataKey, intByteArray);
            GameSaveLoad.SaveGameDataToDisk();
        }
#endif
    }
*/

#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Main.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.56
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
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace KiteLionGames.KiteLionLibrary.COPYME.Assets.Scripts.Game
{
    /// <summary>
    ///     
    /// </summary>
    public class Main : MonoBehaviour
    {
        /// <summary>
        ///     These scenes must match the scenes in the build settings, letter by letter.
        ///     !! DO NOT REARRANGE!!
        /// </summary>
        public enum Scenes
        {
            //DO NOT REARRANGE BELOW!!
            Boot,
            Splash,
            // DO NOT REARRANGE ABOVE!!
            Main,
            //ADD YOUR OWN SCENES BELOW
        }

        /// <summary>
        /// Scenes that will NOT be in production, like maybe an in-editor designer scene.
        /// </summary>
        public static readonly List<string> EditorOnlyScenes = new List<string>
            {
            // Scenes.RandomSceneNotInBuildSettings.ToString(), ...
        };

        // Start is called before the first frame update
        protected void Start()
        {
            Debug.Log("Game ... start!");
        }

        // Update is called once per frame
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Gets scene path, editor-only functonality.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns>relative path, empty string if not found.</returns>
        public static string FindScenePath(string sceneName)
        {
            // Find all scenes in the project
            var guids = AssetDatabase.FindAssets("t:Scene");

            foreach (var guid in guids)
            {
                // Get the path of the scene
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // Get the name of the scene
                var name = Path.GetFileNameWithoutExtension(path);

                // Check if the name matches the one you're looking for
                if (name == sceneName)
                {
                    return path;
                }
            }

            return "";
        }

#endif
        /// <summary>
        ///     Doesn't care if scene is in build settings or not.
        /// </summary>
        /// <param name="sceneName">any scene name in your project (Asset Database)</param>
        /// <param name="loadSceneMode"></param>
        public static void LoadSceneBuildAgnostic(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (EditorOnlyScenes.Contains(sceneName))
            {
#if UNITY_EDITOR
                EditorSceneManager.LoadSceneInPlayMode(FindScenePath(sceneName), new LoadSceneParameters(loadSceneMode));
#endif
            }
            else
            {
                SceneManager.LoadScene(sceneName, loadSceneMode);
            }
        }

        /// <summary>
        ///     Doesn't care if scene is in build settings or not.
        /// </summary>
        /// <param name="sceneName">any scene name in your project (Asset Database)</param>
        /// <param name="loadSceneMode"></param>
        public static AsyncOperation LoadSceneBuildAgnosticAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (EditorOnlyScenes.Contains(sceneName))
            {
#if UNITY_EDITOR
                return EditorSceneManager.LoadSceneAsyncInPlayMode(FindScenePath(sceneName), new LoadSceneParameters(loadSceneMode));
#else
                throw new System.Exception($"Scene {sceneName} should not exist in build settings, it is an editor-only scene!.");
#endif
            }
            return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        }
    }
}

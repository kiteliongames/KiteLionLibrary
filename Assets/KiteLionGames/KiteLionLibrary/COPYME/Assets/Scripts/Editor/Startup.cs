#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Startup.cs
// Author:  Eliot CS
// Created: 2024.09.18.22.09.31
// Edited: 2024.09.19.01.09.12
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


using KiteLionGames.KiteLionLibrary.Portables.Utilities.Scripts;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;

namespace KiteLionGames.KiteLionLibrary.COPYME.Assets.Scripts.Editor
{
#if UNITY_EDITOR
    /// <summary>
    ///     Setup some editor preferences when the editor loads.
    ///     All Code under the "CopyToNewProjects" namespace end w an underscore.
    ///     Remove the "_" underscore when implementing in your project.
    ///     todo: make sure all "CopyMe" code ends w an underscore.
    ///     todo: whats the underscore here symbolize?
    /// </summary>
    [InitializeOnLoad]
    public static class Startup
    {
        static Startup()
        {
            EditorSceneManager.sceneClosing += SceneClosingCallback;
            EditorSceneManager.sceneOpened += SceneOpenedCallback;
            EditorSceneManager.sceneSaving += SceneSavingCallback;
            EditorApplication.quitting += QuittingCallbackHelper;
        }

        private static void QuittingCallbackHelper()
        {
            Logging.WriteOnce("Quitting", "", "startuplog");
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                //todo this doesn't seem to actually do anything as of v.2021
                //todo do ANYTHING if the scene is closing? Now that the designer scene is an independent instance scene.
                var scene = SceneManager.GetSceneAt(i);
                //SceneClosingCallback(scene, false);
            }
        }

        private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
        {
            //Logging.WriteOnce("open", "", "startuplog");
            //StackableDesigner.IsQuitingStackDesignerScene = false;
            // search the scene for the gameobjects named "__StackablesDesignSavePrefab__" and "__donottouch" and instantiate them if not found
            //if (scene.name == StackableDesigner.SceneName)
            //{
            //todo do ANYTHING if the scene is closing? Now that the designer scene is an independent instance scene.
            //StackableDesigner.CleanupDesigner();
            //StackableDesigner.SetupDesigner();
            //}
        }

        private static void SceneClosingCallback(Scene scene, bool removingScene)
        {
            //Logging.WriteOnce("closing", "", "startuplog");
            // search the scene for the gameobjects named "__StackablesDesignSavePrefab__" and "__donottouch" and instantiate them if not found
            //if (scene.name == StackableDesigner.SceneName)
            //{
            //todo do ANYTHING if the scene is closing? Now that the designer scene is an independent instance scene.
            //StackableDesigner.CleanupDesigner();
            //}
        }

        private static void SceneSavingCallback(Scene scene, string path)
        {
            //if (scene.name == StackableDesigner.SceneName)
            //{
            //    EditorUtility.DisplayDialog("!! Stack Designer Warning !!", "You're about to save changes to your Designer scene instance. \r Use the Designer Window and click 'Save' at the bottom if you mean to create a stack design prefab for the Card+ Engine.", "OK");
            //}
        }
    }
#endif
}

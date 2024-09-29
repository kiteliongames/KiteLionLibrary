#region FileHeader

// test
// Project: Assembly-CSharp-Editor
// File:    HotReloadFix.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.04
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

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox.Editor
{
    #if UNITY_EDITOR
    /// <summary>
    ///     Found Courtesy of: https://forum.unity.com/threads/refresh-assets-when-entering-exiting-play-mode.717636/
    ///     https://forum.unity.com/threads/c-hot-reload-with-vs-hidden-feature.365706/#post-2368194
    /// </summary>
    [InitializeOnLoad]
    public static class OnSceneLoadScript
    {
        static OnSceneLoadScript()
        {
            EditorApplication.playModeStateChanged += ChangePlaymodeCallback;
        }

        private static void ChangePlaymodeCallback(PlayModeStateChange newState)
        {
            //This is only kicks off when you have exited play mode.
            //if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            if (newState == PlayModeStateChange.EnteredEditMode)
            {
                EditorPrefs.SetBool("kAutoRefresh", true);
                AssetDatabase.Refresh();
            }

            //Called just after play mode is entered
            //if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
            if (newState == PlayModeStateChange.EnteredPlayMode)
            {
                EditorPrefs.SetBool("kAutoRefresh", false);
            }
        }
    }
#endif
}

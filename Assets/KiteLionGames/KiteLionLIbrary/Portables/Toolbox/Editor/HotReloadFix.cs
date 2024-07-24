using UnityEditor;

namespace KiteLionGames.Utilities
{

    /// <summary>
    /// Found Courtesy of: https://forum.unity.com/threads/refresh-assets-when-entering-exiting-play-mode.717636/
    /// https://forum.unity.com/threads/c-hot-reload-with-vs-hidden-feature.365706/#post-2368194
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
}

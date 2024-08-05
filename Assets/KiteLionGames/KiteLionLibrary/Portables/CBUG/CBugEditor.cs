using KiteLionGames.BetterDebug;

public class CBugEditor
{
#if UNITY_EDITOR
    // ensure class initializer is called whenever scripts recompile
    [UnityEditor.InitializeOnLoad]
    public static class PlayModeStateChangedExample
    {
        // register an event handler when the class is initialized
        static PlayModeStateChangedExample()
        {
            UnityEditor.EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                OnEditorPlayModeChanged(false);
            }
        }
    }
    //editor method to set isSceneLoaded to false when the editor play mode is stopped
    public static void OnEditorPlayModeChanged(bool isPlaying)
    {
        if (isPlaying == false)
            CBUG.isSceneLoaded = false;
    }
#endif
}

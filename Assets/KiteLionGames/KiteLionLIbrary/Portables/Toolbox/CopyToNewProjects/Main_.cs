using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KiteLionGames.Toolbox.CopyToNewProjects
{
    /// <summary>
    /// Remove underscore from class name and file after copying to new project.
    /// </summary>
    public class Main_ : MonoBehaviour
    {
        /// <summary>
        /// These scenes must match the scenes in the build settings, letter by letter.
        /// !! DO NOT REARRANGE!!
        /// </summary>
        public enum Scenes
        {
            //DO NOT REARRANGE BELOW!!
            BootSample,
            SplashSample,
            MenuSample,
            // DO NOT REARRANGE ABOVE!!
            //ADD YOUR OWN SCENES BELOW
        }

        public static readonly List<string> EditorOnlyScenes = new() {
            Scenes.BootSample.ToString(),
            Scenes.SplashSample.ToString(),
            Scenes.MenuSample.ToString(),
        };


#if UNITY_EDITOR
        /// <summary>
        /// Gets scene path, editor-only functonality.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns>relative path, empty string if not found.</returns>
        public static string FindScenePath(string sceneName)
        {
            // Find all scenes in the project
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Scene");

            foreach (string guid in guids)
            {
                // Get the path of the scene
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

                // Get the name of the scene
                string name = System.IO.Path.GetFileNameWithoutExtension(path);

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
        /// Doesn't care if scene is in build settings or not.
        /// </summary>
        /// <param name="sceneName">any scene name in your project (Asset Database)</param>
        /// <param name="loadSceneMode"></param>
        public static void LoadSceneBuildAgnostic(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (EditorOnlyScenes.Contains(sceneName) == true)
            {
#if UNITY_EDITOR
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(FindScenePath(sceneName), new LoadSceneParameters(loadSceneMode));
#endif
            }
            else
            {
                SceneManager.LoadScene(sceneName, loadSceneMode);
            }
        }

        /// <summary>
        /// Doesn't care if scene is in build settings or not.
        /// </summary>
        /// <param name="sceneName">any scene name in your project (Asset Database)</param>
        /// <param name="loadSceneMode"></param>
        public static AsyncOperation LoadSceneBuildAgnosticAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (EditorOnlyScenes.Contains(sceneName) == true)
            {
#if UNITY_EDITOR
                return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(FindScenePath(sceneName), new LoadSceneParameters(loadSceneMode));
#else
                throw new System.Exception($"Scene {sceneName} should not exist in build settings, it is an editor-only scene!.");
#endif
            }
            else
            {
                return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            }
        }

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
    }
}

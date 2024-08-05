using KiteLionGames.Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace KiteLionGames.Toolbox.CopyToNewProjects   
{

    /// <summary>
    /// Setup some editor preferences when the editor loads.
    /// </summary>
    [InitializeOnLoad]
    public static class Startup_
    {

        static Startup_()
        {
            EditorSceneManager.sceneClosing += SceneClosingCallback;
            EditorSceneManager.sceneOpened += SceneOpenedCallback;
            EditorSceneManager.sceneSaving += SceneSavingCallback;
            EditorApplication.quitting += QuittingCallback;
        }

        private static void QuittingCallback()
        {
            Logging.WriteOnce("qutting", "", "startuplog");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                //todo this doesn't seem to actually do anything as of v.2021
                //todo do ANYTHING if the scene is closing? Now that the designer scene is an independent instance scene.
                Scene scene = SceneManager.GetSceneAt(i);
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

        private static void SceneSavingCallback(UnityEngine.SceneManagement.Scene scene, string path)
        {
            //if (scene.name == StackableDesigner.SceneName)
            //{
            //    EditorUtility.DisplayDialog("!! Stack Designer Warning !!", "You're about to save changes to your Designer scene instance. \r Use the Designer Window and click 'Save' at the bottom if you mean to create a stack design prefab for the Card+ Engine.", "OK");
            //}
        }
    }
}

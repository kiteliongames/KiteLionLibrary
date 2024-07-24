using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KiteLionGames.Toolbox
{
    /// <summary>
    /// Enforces a scene to load when you hit play on active scene. Boot Scene Enforcer will enforce the other scene.
    /// Disable via BootSceneEnforcer.CanEnforce = false;
    /// </summary>
    public static class BootSceneEnforcer 
    {
        public static bool CanEnforce { get; private set; } = true;
        /// <summary>
        /// Scene to load when you hit play on active scene. Boot Scene Enforcer will enforce the other scene.
        /// </summary>
        public static Dictionary<string, string> ActiveSceneToBootScene {get; private set; } = new() {
            // { Main.Scenes.Boot.ToString(), Main.Scenes.Other.ToString()},
        };

        /// <summary>
        /// This is the Unity 'hack' to run a method before the scene loads. This will enforce the scene to load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnBeforeSceneLoad()
        {
            var activeScene = SceneManager.GetActiveScene();
            bool enforceableScene = ActiveSceneToBootScene.TryGetValue(activeScene.name, out string sceneToLoad);
            if (enforceableScene)
            {
                if (Boot.IsBooted == false && CanEnforce)
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
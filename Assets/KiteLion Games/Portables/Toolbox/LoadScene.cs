using UnityEngine.SceneManagement;

namespace KiteLionGames.Utilities
{
    /// <summary>
    /// Easy simple scene loading class.
    /// </summary>
    public class SceneLoader
    {
        public bool IfExistsThenReload = false;
        public bool LoadAdditive = false;
        public Main.Scenes SceneToLoad;
        public float LoadDelay = 0f;

        public SceneLoader(Main.Scenes sceneToLoad, bool reloadIfExists = false, bool loadAdditive = false, float loadDelay = 0f)
        {
            SceneToLoad = sceneToLoad;
            IfExistsThenReload = reloadIfExists;
            LoadAdditive = loadAdditive;
            LoadDelay = loadDelay;
        }

        public void DoLoad()
        {
            Common.Tools.DelayFunction(LoadHelper, LoadDelay);
        }

        private void LoadHelper()
        {
            bool sceneLoaded = SceneManager.GetSceneByName(SceneToLoad.ToString()).isLoaded;
            bool doSceneLoad = (sceneLoaded && IfExistsThenReload) || sceneLoaded == false;

            if (doSceneLoad)
            {
                SceneManager.LoadScene(SceneToLoad.ToString(), LoadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            } else
            {
                throw new System.Exception("Scene " + SceneToLoad.ToString() + " is already loaded.");
            }
        }
    }
}

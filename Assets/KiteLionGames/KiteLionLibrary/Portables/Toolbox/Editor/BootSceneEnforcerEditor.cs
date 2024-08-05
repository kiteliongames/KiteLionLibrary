using UnityEditor;
using UnityEngine.SceneManagement;

//todo put in retirement bin
//[CustomEditor(typeof(BootSceneEnforcer), true)]
public class BootSceneEnforcerEditor : Editor
{
    public void OnEnable()
    {
        //SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, Scene arg1)
    {
        //foreach (var item in arg0.GetRootGameObjects())
        //{
        //    var bootSceneEnforcerScript = item.GetComponent<BootSceneEnforcer>();
        //    if(bootSceneEnforcerScript != null)
        //    {
        //        bootSceneEnforcerScript.OnValidoot();
        //        break;
        //    }
        //    bootSceneEnforcerScript = item.GetComponentInChildren<BootSceneEnforcer>();
        //    if (bootSceneEnforcerScript != null)
        //    {
        //        bootSceneEnforcerScript.OnValidoot();
        //        break;
        //    }
        //}
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        //BootSceneEnforcer baseClassTarget = (BootSceneEnforcer)target;

        ////dynamically get all scenes with 'boot' in the name
        //List<string> bootScenesList = new ();
        //foreach (var eScene in EditorBuildSettings.scenes)
        //{
        //    string sceneName = Path.GetFileNameWithoutExtension(eScene.path);
        //    string sceneNameLower = sceneName.ToLower();
        //    if (sceneNameLower.Contains("boot"))
        //        bootScenesList.Add(sceneName);
        //}
        //if (bootScenesList.Count == 0)
        //{
        //    Debug.Log("You're missing a boot scene. Please include at least one scene with 'boot' in its name in your build settings.");
        //    return;
        //}

        //if (baseClassTarget.targetBootSceneIndexx >= bootScenesList.Count)
        //    baseClassTarget.targetBootSceneIndexx = 0;

        //baseClassTarget.BootScenes = bootScenesList.ToArray();


        ////expose scene options to editor inspector 
        //var _newBootSceneIndex = baseClassTarget.targetBootSceneIndexx;
        //_newBootSceneIndex = EditorGUILayout.Popup("Boot Scene", baseClassTarget.targetBootSceneIndexx, bootScenesList.ToArray());
        //if(_newBootSceneIndex != baseClassTarget.targetBootSceneIndexx)
        //{
        //    baseClassTarget.targetBootSceneIndexx = _newBootSceneIndex;
        //    baseClassTarget.OnValidoot();
        //}

        ////Debug.Log("Boot Scene Index: " + baseClassTarget.targetBootSceneIndexx);
        //EditorGUILayout.HelpBox("This will force the boot scene to be " + baseClassTarget.BootScenes[baseClassTarget.targetBootSceneIndexx] + " when the game starts.", MessageType.Info);
    }
}

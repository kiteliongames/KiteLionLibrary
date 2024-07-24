using UnityEditor;
using UnityEngine;

public class ForceReserialization : MonoBehaviour
{
    [MenuItem("Asset Database/Force Reserialize")]
    static void Do()
    {
        //Confirmation popup window, if yes, then continue
        if (EditorUtility.DisplayDialog("Force Reserialize", "Are you sure you want to force reserialize all assets?", "Yes", "No"))
            AssetDatabase.ForceReserializeAssets();
    }
}
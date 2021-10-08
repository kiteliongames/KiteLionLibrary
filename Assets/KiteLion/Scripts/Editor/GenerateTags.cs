using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateTags : AssetPostprocessor
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static int maxTags = 10000;
    //private static int maxLayers = 31; //unused

    public static bool AddTag(string tagName) {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // Tags Property
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        if (tagsProp.arraySize >= maxTags) {
            Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
            return false;
        }
        // if not found, add it
        //if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName)) {
        //    int index = tagsProp.arraySize;
        //    // Insert new array element
        //    tagsProp.InsertArrayElementAtIndex(index);
        //    SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
        //    // Set array element to tagName
        //    sp.stringValue = tagName;
        //    Debug.Log("Tag: " + tagName + " has been added");
        //    // Save settings
        //    tagManager.ApplyModifiedProperties();
        //    return true;
        //} else {
        //    //Debug.Log ("Tag: " + tagName + " already exists");
        //}
        return false;
    }


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        foreach (string str in importedAssets) {
            Debug.Log("Reimported Asset: " + str);
        }
        foreach (string str in deletedAssets) {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++) {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }
}
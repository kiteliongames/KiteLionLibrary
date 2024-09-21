#region FileHeader

// test
// Project: Assembly-CSharp-Editor
// File:    GenerateTags.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.00
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


using UnityEditor;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Editor
{
    #if UNITY_EDITOR
    public class GenerateTags : AssetPostprocessor
    {
        private static readonly int maxTags = 10000;


        /// <summary>
        /// todo this function needs to be in its own class or something ...
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var str in importedAssets)
            {
                Debug.Log("Reimported Asset: " + str);
            }
            foreach (var str in deletedAssets)
            {
                Debug.Log("Deleted Asset: " + str);
            }

            for (var i = 0; i < movedAssets.Length; i++)
            {
                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        //private static int maxLayers = 31; //unused

        public static bool AddTag(string tagName)
        {
            // Open tag manager
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Tags Property
            var tagsProp = tagManager.FindProperty("tags");
            if (tagsProp.arraySize >= maxTags)
            {
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
            //    // WriteOnce settings
            //    tagManager.ApplyModifiedProperties();
            //    return true;
            //} else {
            //    //Debug.Log ("Tag: " + tagName + " already exists");
            //}
            return false;
        }
    }
#endif
}

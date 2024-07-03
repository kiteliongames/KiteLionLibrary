using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PhotonArenaBuilder : MonoBehaviour {
    [MenuItem("PhotonArena/Create")]
    static void Create() {
        string manName = "PhotonArenaManager";

        GameObject prefab;

        // Loading in the reference to your prefab
        prefab = Resources.Load(manName, typeof(GameObject)) as GameObject;

        // Loading in and instantiating an instance of your prefab
        prefab = Instantiate(Resources.Load(manName, typeof(GameObject))) as GameObject;

        TagHelper.AddTag(manName);

        prefab.tag = manName;
        prefab.gameObject.name = manName;
    }
}
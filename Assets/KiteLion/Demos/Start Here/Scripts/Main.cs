using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("CBUG_ON", 0);
        CBUG.Do("Sanity check Start");
    }

    // Update is called once per frame
    void Update()
    {
        CBUG.Do("Sanity check Update");

    }
}

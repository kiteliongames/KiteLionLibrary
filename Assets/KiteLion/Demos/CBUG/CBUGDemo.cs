using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBUGDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CBUG.Do("START");
    }

    // Update is called once per frame
    void Update()
    {
        CBUG.Do("UPDATE");
    }
}

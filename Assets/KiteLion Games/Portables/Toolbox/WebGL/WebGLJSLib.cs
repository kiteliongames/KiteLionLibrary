using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLJSLib : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        string[] urlArray = new string[2] { 
            "https://www.kiteliongames.com/images/klgvidsupported.webm",
            "https://www.kiteliongames.com/images/klgvidsupported.webm" };

        string urls = string.Join(" ", urlArray);

        int length = urls.Length;

        //DoClipBuildJsLib(urls, length);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    [DllImport("__Internal")]
    private static extern void DoClipBuildJsLib(string urls, int length);

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLink : MonoBehaviour
{
    public string Link;

    public void OpenURL() {
        Application.OpenURL("https://drive.google.com/drive/folders/1OY9vMYWH_XN6TP494TH0BzuKMFKSNOY0?usp=sharing");
    }
}

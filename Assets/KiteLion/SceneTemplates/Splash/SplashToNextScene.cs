using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SplashToNextScene : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.time < 1f) return; // wait for video to load

        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying)
            {
                return;
            }
            else
            {
                LoadNextScene();
            }
        }

        if(Input.anyKeyDown == true) {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
    }
}

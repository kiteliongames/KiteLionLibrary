using KiteLionGames.BetterDebug;
using KiteLionGames.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace KiteLionGames.Utilities
{

    public class SplashToNextScene : MonoBehaviour
    {
        public AudioListener SplashAudioListener;
        public VideoPlayer SplashScreenVideoPlayer;
        public MeshRenderer SplashScreenImageRenderer;

        public Main.Scenes[] NextScenes;

        public UnityEngine.Camera[] CamerasToDisable;

        [Serializable]
        public class SplashData
        {
            public enum SplashType
            {
                Video,
                Image,
                URL
            }

            public SplashType Type;
            public Sprite Image;
            public VideoClip Video;
            public string URL;
            public float Duration;
        }

        [Min(0.01f)]
        public float SkipCooldownAfter = 0.5f;

        [SerializeField]
        private SplashData[] _splashData;
        private AsyncOperation[] NextSceneLoadOperations;
        private bool _canSkip = false;
        private int _currentSplashIndex = 0;
        private bool _isPlayingPaused = true;
        private float _startTime = 0f;
        private float _currentSplashStartTime = 0f;
        private bool _doSkip;
        private bool _loadingNextScene;
        private float _skipCooldownTimer;


        // Start is called before the first frame update
        protected void Start()
        {
            //save data if first time loading game
            if (PlayerPrefs.GetInt("FirstTime") == 0)
            {
                PlayerPrefs.SetInt("FirstTime", 1);
                PlayerPrefs.Save();
            }
            else
            {
                _canSkip = true;
            }
            _isPlayingPaused = true;

        }

        // Update is called once per frame
        protected void LateUpdate()
        {
            if (Time.time < 1f) return; // wait for videos to load
            if (_loadingNextScene == true) return;

            if (_startTime == 0f)
            {
                _startTime = Time.time;
            }

            if (_doSkip || (_isPlayingPaused == false && Time.time > _currentSplashStartTime + _splashData[_currentSplashIndex].Duration))
            {
                CBUG.Do("" + (_currentSplashStartTime + _splashData[_currentSplashIndex].Duration));
                CBUG.Do("T" + Time.time);

                _doSkip = false;
                _isPlayingPaused = true;
                _currentSplashIndex++;

                if (_currentSplashIndex >= _splashData.Length)
                {
                    for (int i = 0; i < CamerasToDisable.Length; i++)
                    {
                        CamerasToDisable[i].enabled = false;
                    }
                    _loadingNextScene = true;
                    Tools.DelayFunction(LoadNextScenes, 0.01f);
                    return;
                }
            }

            if (_isPlayingPaused == true)
            {
                _isPlayingPaused = false;
                _currentSplashStartTime = Time.time;
                AssignSplashByIndexHelper();
            }


            bool anyMouseKeyDown = false;
            foreach (var control in Mouse.current.allControls)
            {
                ButtonControl button = control as ButtonControl;
                if (button is not null && control.IsPressed())
                {
                    anyMouseKeyDown = true;
                }
            }
            if ((Keyboard.current.anyKey.isPressed == true || anyMouseKeyDown) && _canSkip)
            {
                if (_skipCooldownTimer == 0f)
                {
                    _skipCooldownTimer = Time.time;
                }
                if (Time.time > _skipCooldownTimer + SkipCooldownAfter)
                {
                    _doSkip = true;
                    _skipCooldownTimer = Time.time;
                    CBUG.Do("SKIP");
                }
            }
        }

        private IEnumerator SceneLoadingProgress()
        {
            while (true)
            {
                float progress = 0f;
                for (int i = 0; i < NextSceneLoadOperations.Length; i++)
                {
                    progress += NextSceneLoadOperations[i].progress;
                }
                progress /= NextSceneLoadOperations.Length;
                CBUG.Do("Loading progress: " + progress);
                yield return null;
                if (progress == 1)
                {
                    SceneManager.UnloadSceneAsync(gameObject.scene);
                    break;
                }
            }

        }

        void LoadNextScenes()
        {
            SplashAudioListener.enabled = false;
            NextSceneLoadOperations = new AsyncOperation[NextScenes.Length];
            //int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            for (int i = 0; i < NextScenes.Length; i++)
            {
                Main.Scenes scene = NextScenes[i];
                string sceneName = scene.ToString();
                AsyncOperation _ = Main.LoadSceneBuildAgnosticAsync(sceneName, LoadSceneMode.Additive);
                NextSceneLoadOperations[i] = _;
            }
            StartCoroutine(SceneLoadingProgress());
        }

        private void AssignSplashByIndexHelper()
        {
            if (_splashData[_currentSplashIndex].Type == SplashData.SplashType.Video)
            {
                SplashScreenImageRenderer.enabled = false;
                SplashScreenVideoPlayer.enabled = true;
                SplashScreenVideoPlayer.source = VideoSource.VideoClip;
                SplashScreenVideoPlayer.clip = _splashData[_currentSplashIndex].Video;
                SplashScreenVideoPlayer.Stop();
                SplashScreenVideoPlayer.Play();
            }
            else if (_splashData[_currentSplashIndex].Type == SplashData.SplashType.Image)
            {
                SplashScreenVideoPlayer.enabled = false;
                SplashScreenImageRenderer.enabled = true;
                SplashScreenImageRenderer.material.mainTexture = _splashData[_currentSplashIndex].Image.texture;
            }
            else if (_splashData[_currentSplashIndex].Type == SplashData.SplashType.URL)
            {
                SplashScreenImageRenderer.enabled = false;
                SplashScreenVideoPlayer.enabled = true;
                if (SplashScreenVideoPlayer.isPlaying) SplashScreenVideoPlayer.Stop();
                SplashScreenVideoPlayer.source = VideoSource.Url;
                SplashScreenVideoPlayer.prepareCompleted += VideoFinishedCallbackHelper;
                SplashScreenVideoPlayer.errorReceived += VideoErrorCallbackHelper;
                SplashScreenVideoPlayer.url = _splashData[_currentSplashIndex].URL;
                SplashScreenVideoPlayer.Prepare();
            }

        }

        private void VideoFinishedCallbackHelper(VideoPlayer eventHandler)
        {
            if (eventHandler != null && eventHandler != null)
            {
                CBUG.Do("vid ready");
                eventHandler.Play();
            }
        }

        private void VideoErrorCallbackHelper(VideoPlayer eventHandler, string message)
        {
            if (eventHandler != null && eventHandler != null)
            {
                CBUG.Do("vid error: " + message);
            }
        }
    }
}

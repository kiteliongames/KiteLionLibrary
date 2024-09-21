#region FileHeader

// test
// Project: Assembly-CSharp
// File:    SplashToNextScene.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.14
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

using System;
using System.Collections;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using KiteLionGames.KiteLionLibrary.Portables.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace KiteLionGames.KiteLionLibrary.COPYME.Assets.Scripts.Game
{
    public class SplashToNextScene : MonoBehaviour
    {
        public AudioListener SplashAudioListener;
        public VideoPlayer SplashScreenVideoPlayer;
        public MeshRenderer SplashScreenImageRenderer;

        public Main.Scenes[] NextScenes;

        public UnityEngine.Camera[] CamerasToDisable;

        [Min(0.01f)]
        public float SkipCooldownAfter = 0.5f;

        [SerializeField]
        private SplashData[] _splashData;
        private bool _canSkip;
        private int _currentSplashIndex;
        private float _currentSplashStartTime;
        private bool _doSkip;
        private bool _isPlayingPaused = true;
        private bool _loadingNextScene;
        private float _skipCooldownTimer;
        private float _startTime;
        private AsyncOperation[] NextSceneLoadOperations;


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
            if (Time.time < 1f) return;// wait for videos to load
            if (_loadingNextScene) return;

            if (_startTime == 0f)
            {
                _startTime = Time.time;
            }

            if (_doSkip || _isPlayingPaused == false && Time.time > _currentSplashStartTime + _splashData[_currentSplashIndex].Duration)
            {
                // CBUG.Do("" + (_currentSplashStartTime + _splashData[_currentSplashIndex].Duration));
                // CBUG.Do("T" + Time.time);

                _doSkip = false;
                _isPlayingPaused = true;
                _currentSplashIndex++;

                if (_currentSplashIndex >= _splashData.Length)
                {
                    for (var i = 0; i < CamerasToDisable.Length; i++)
                    {
                        CamerasToDisable[i].enabled = false;
                    }
                    _loadingNextScene = true;
                    Tools.DelayFunction(LoadNextScenes, 0.01f);
                    return;
                }
            }

            if (_isPlayingPaused)
            {
                _isPlayingPaused = false;
                _currentSplashStartTime = Time.time;
                AssignSplashByIndexHelper();
            }
            
            var anyMouseKeyDown = false;
            foreach (var control in Mouse.current.allControls)
            {
                var button = control as ButtonControl;
                if (button is not null && control.IsPressed())
                {
                    anyMouseKeyDown = true;
                }
            }
            if ((Keyboard.current.anyKey.isPressed || anyMouseKeyDown) && _canSkip)
            {
                if (_skipCooldownTimer == 0f)
                {
                    _skipCooldownTimer = Time.time;
                }
                if (Time.time > _skipCooldownTimer + SkipCooldownAfter)
                {
                    _doSkip = true;
                    _skipCooldownTimer = Time.time;
                    // CBUG.Do("SKIP");
                }
            }
        }

        private IEnumerator SceneLoadingProgress()
        {
            while (true)
            {
                var progress = 0f;
                for (var i = 0; i < NextSceneLoadOperations.Length; i++)
                {
                    progress += NextSceneLoadOperations[i].progress;
                }
                progress /= NextSceneLoadOperations.Length;
                // CBUG.Do("Loading progress: " + progress);
                yield return null;
                if (progress == 1)
                {
                    SceneManager.UnloadSceneAsync(this.gameObject.scene);
                    break;
                }
            }
        }

        private void LoadNextScenes()
        {
            SplashAudioListener.enabled = false;
            NextSceneLoadOperations = new AsyncOperation[NextScenes.Length];
            //int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            for (var i = 0; i < NextScenes.Length; i++)
            {
                var scene = NextScenes[i];
                var sceneName = scene.ToString();
                var _ = Main.LoadSceneBuildAgnosticAsync(sceneName, LoadSceneMode.Additive);
                NextSceneLoadOperations[i] = _;
            }
            this.StartCoroutine(SceneLoadingProgress());
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
                SplashScreenVideoPlayer.prepareCompleted += VideoFinishedCallbackHelper;
                SplashScreenVideoPlayer.errorReceived += VideoErrorCallbackHelper;
                
                SplashScreenVideoPlayer.source = VideoSource.Url;
                SplashScreenVideoPlayer.url = _splashData[_currentSplashIndex].URL;
                SplashScreenVideoPlayer.Prepare();
            }
        }

        private void VideoFinishedCallbackHelper(VideoPlayer eventHandler)
        {
            if (eventHandler != null && eventHandler != null)
            {
                // CBUG.Do("vid ready");
                eventHandler.Play();
            }
        }

        private void VideoErrorCallbackHelper(VideoPlayer eventHandler, string message)
        {
            if (eventHandler != null && eventHandler != null)
            {
                CBUG.LogError("SplashScreen VideoPlayer Error: " + message);
            }
        }

        [Serializable]
        public class SplashData
        {
            public enum SplashType
            {
                Video,
                Image,
                URL,
            }

            public SplashType Type;
            public Sprite Image;
            public VideoClip Video;
            public string URL;
            public float Duration;
        }
    }
}

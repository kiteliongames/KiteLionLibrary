#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Tools.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.02
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

/* Copyright (C) KiteLion Games, LLC - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 *
 * Written by Eliot Carney-Seim <support@kiteliongames.com>, January 2023
 */

using System;
using System.Collections;
using System.Linq;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using KiteLionGames.KiteLionLibrary.Portables.BetterTag;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.

/// <summary>
/// A set of easily callable functions built to be used through a singleton in Unity.
/// Current Functionalities:
///  - Delay an animation
///  - Delay a function
///  - Fade a Text object.
///  - Remove whitespace. (super trim)
///
/// Contributors:
///  - Eliot Leo Carney-Seim
///
/// Credit:
/// https://benjaminbeagley.wordpress.com/2013/10/14/calling-a-static-ienumerator-in-unity/
///
/// Further Reading:
/// http://wiki.unity3d.com/index.php/Singleton
/// http://wiki.unity3d.com/index.php/Toolbox
/// </summary>
namespace KiteLionGames.KiteLionLibrary.Portables.Utilities
{
    public class Tools : MonoBehaviour
    {
        public delegate void VanillaFunction();

        protected Tools()
        {
            //No instantiation!
        }

        #region Getters and Setters

        public int Priority
        {
            set => priority = value;
        }

        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            Tag.GetTagComponent(this.gameObject, true, Tag.label.Tools);

            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        ///     Finds a component in all scenes. Useful for finding a component that is not in the current scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T FindComponentInAllScenes<T>() where T : Component
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)// Ensure the scene is loaded
                {
                    foreach (var rootGameObject in scene.GetRootGameObjects())
                    {
                        T component;
                        component = rootGameObject.GetComponent<T>();
                        if (component == null)
                        {
                            //if not found, check children
                            component = rootGameObject.GetComponentInChildren<T>();
                            if (component == null)
                            {
                                //if not found, check next rootGameObject
                                continue;
                            }
                            return component;
                        }
                        return component;
                    }
                }
            }
            return null;
        }

        #region Helper Functions

        private void _fadeText(Text textToFade, float from, float to, float speed)
        {
        }

        #endregion

        #region Singleton Control

        private int priority;

        public static Tools instance
        {
            get
            {
                var currentInstance = FindObjectOfType(typeof(Tools)) as Tools;
                if (currentInstance == null)
                {
                    currentInstance = new GameObject("Tools").AddComponent<Tools>();
                }
                return currentInstance;
            }

            set => instance = value;
        }

        #endregion

        #region Public Static Hooks

        public static string RemoveWhitespace(string inString)
        {
            var inputLen = inString.Length;
            var outString = inString;

            var charPos = -1;
            foreach (var ch in inString)
            {
                charPos++;
                if (char.IsWhiteSpace(ch))
                {
                    outString = outString.Replace(inString.Substring(charPos, 1), "");
                }
            }

            return outString;
        }

        /// <summary>
        ///     Extensions for arrays (currently just for Byte arrays)
        ///     A byte pool is used for the data storage of the SaveGameManager. a 1D byte pool is used to store all the data for
        ///     the game.
        ///     ex: PlayerName = byte [0], PlayerLevel = byte[10], PlayerHealth = byte[20], etc.
        ///     You must manage on your own what data is stored where. The SaveGameManager only stores the data and it's key.
        /// </summary>
        public static class ByteArrayExtensions
        {
            [Serializable]
            public class ByteArrayPool
            {
                public string[] Keys = new string[0];
                public int TotalItems;
                public byte[][] Data = new byte[0][];

                /// <summary>
                ///     Adds item to pool. Resizes pool.
                /// </summary>
                /// <param name="pool">your byte array wrapper</param>
                /// <param name="item">NEW item to add. WILL FAIL if item not new.</param>
                /// <returns>True if successful, false if not.</returns>
                public bool AddItem(ref ByteArrayItem item)
                {
                    if (Keys.Contains(item.Key))
                    {
                        CBUG.Do("Item already exists in pool. Use UpdateItem instead.");
                        return false;
                    }
                    var foundSpot = -1;
                    for (var i = 0; i < Keys.Length; i++)
                    {
                        if (Keys[i] is null)
                        {
                            Keys[i] = item.Key;
                            Data[i] = item.Data;
                            TotalItems++;
                            foundSpot = i;
                            break;
                        }
                    }

                    if (foundSpot == -1)
                    {
                        Array.Resize(ref Data, Data.Length + 1);
                        Array.Resize(ref Keys, Keys.Length + 1);
                        Data[Data.Length - 1] = item.Data;
                        Keys[Keys.Length - 1] = item.Key;
                        TotalItems++;
                    }
                    return true;
                }

                /// <summary>
                ///     Adds item to pool. Resizes pool.
                /// </summary>
                /// <param name="pool">your byte array wrapper</param>
                /// <param name="item">NEW item to add. WILL FAIL if item not new.</param>
                /// <returns>True if successful, false if not.</returns>
                public bool AddItem(string itemKey, byte[] itemData)
                {
                    var i = new ByteArrayItem
                    {
                        Data = itemData, Key = itemKey,
                    };
                    return AddItem(ref i);
                }

                public bool UpdateItem(ref ByteArrayItem item)
                {
                    if (Keys.Contains(item.Key))
                    {
                        var foundSpot = -1;
                        for (var i = 0; i < Keys.Length; i++)
                        {
                            if (Keys[i] == item.Key)
                            {
                                Data[i] = item.Data;
                                foundSpot = i;
                                break;
                            }
                        }

                        if (foundSpot == -1)
                        {
                            CBUG.Do("Failed to find item in pool.");
                            return false;
                        }
                        return true;
                    }
                    CBUG.Do("Item does not exist in pool. Use AddItem instead.");
                    return false;
                }

                public bool UpdateItem(string itemKey, byte[] itemData)
                {
                    var i = new ByteArrayItem
                    {
                        Data = itemData, Key = itemKey,
                    };
                    return UpdateItem(ref i);
                }

                /// <summary>
                ///     Removes item. Does not resize pool.
                /// </summary>
                /// <param name="pool">Array to remove item from.</param>
                /// <param name="item">Item to remove (needs KEY (the UID) filled).</param>
                /// <returns>True if successful, false if not.</returns>
                /// <exception cref="ArgumentNullException"> You passed a declared but not assigned pool.</exception>
                public bool RemoveItem(ref ByteArrayItem item)
                {
                    if (Keys.Contains(item.Key) == false)
                    {
                        CBUG.Do($"Missing Key: {item.Key}");
                        return false;
                    }

                    var index = Array.IndexOf(Keys, item.Key);
                    Keys[index] = null;
                    Data[index] = null;
                    TotalItems--;
                    return true;
                }

                public bool RemoveItem(string itemKey)
                {
                    var i = new ByteArrayItem
                    {
                        Key = itemKey,
                    };
                    return RemoveItem(ref i);
                }

                public ByteArrayItem GetItem(ByteArrayItem item)
                {
                    if (Keys.Contains(item.Key) == false)
                    {
                        CBUG.Do($"Missing Key: {item.Key}");
                        item.Key = null;
                        item.Data = null;
                        return item;
                    }
                    var key = item.Key;
                    if (Keys.Contains(key))
                    {
                        var index = Array.IndexOf(Keys, key);
                        item.Key = key;
                        item.Data = Data[index];
                    }
                    return item;
                }

                public ByteArrayItem GetItem(string itemKey)
                {
                    var i = new ByteArrayItem
                    {
                        Key = itemKey,
                    };
                    return GetItem(i);
                }
            }

            /// <summary>
            ///     Key is a UID for the data. Use this to add it to the pool.
            /// </summary>
            public struct ByteArrayItem
            {
                public byte[] Data;
                public string Key;
            }
        }

        /// <summary>
        ///     For delaying a function call by a single frame.
        /// </summary>
        /// <param name="Call">Parameter-less function</param>
        /// <param name="Time">How long to delay by seconds. To delay by 1 frame, Time == 0.</param>
        /// <returns></returns>
        public static void DelayFunction(VanillaFunction Call, float Time)
        {
            instance.StartCoroutine(instance._delayFunction(Call, Time));
        }

        /// <summary>
        ///     For delaying a function call by a single frame.
        /// </summary>
        /// <param name="Call">Parameter-less function</param>
        /// <param name="Time">How long to delay by seconds. To delay by 1 frame, Time == 0.</param>
        /// <returns></returns>
        public static void DelayFunction<T>(Action<T> Call, T param, float Time)
        {
            instance.StartCoroutine(instance._delayFunction(Call, param, Time));
        }

        /// <summary>
        ///     For delaying an animation call by a single frame.
        /// </summary>
        /// <param name="Call">Animation object</param>
        /// <param name="Time">How long to delay by. To delay by 1 frame, Time == 0.</param>
        /// <returns></returns>
        public static void DelayAnim(Animator anim, float time,
            string clipName = "",
            string trigger = "",
            float floatParam = -1f,
            int intParam = -1,
            bool boolParam = false,
            bool hasBoolParam = false)
        {
            instance.StartCoroutine(instance._delayAnim(anim, time, clipName,
                trigger,
                floatParam,
                intParam,
                boolParam,
                hasBoolParam
            ));
        }

        /// <summary>
        ///     Returns Tools class. GetComponent<Tools>() supplies the same functionality.
        /// </summary>
        /// <returns></returns>
        public static Tools GetSelf()
        {
            return GameObject.FindGameObjectWithTag("Tools").GetComponent<Tools>()._getSelf();
        }

        #endregion

        #region Private Core Functionality

        private IEnumerator _delayAnim(Animator anim, float time,
            string clipName = "",
            string trigger = "",
            float floatParam = -1f,
            int intParam = -1,
            bool boolParam = false,
            bool hasBoolParam = false)
        {
            if (time <= 0f)
                yield return 0f;
            else
                yield return new WaitForSeconds(time);

            if (clipName != "" && trigger == "")
                CBUG.SeriousError("clipName Required!");

            if (hasBoolParam)
                anim.SetBool(clipName, boolParam);
            else if (trigger != "")
                anim.SetTrigger(trigger);
            else if (floatParam != -1f)
                anim.SetFloat(clipName, floatParam);
            else if (intParam != -1)
                anim.SetFloat(clipName, intParam);
        }

        private IEnumerator _delayFunction(VanillaFunction call, float time)
        {
            if (time <= 0f)
                yield return 0f;
            else
                yield return new WaitForSeconds(time);
            // CBUG.Do("Delaying function call by " + time + " seconds.");
            call();
        }

        private IEnumerator _delayFunction<T>(Action<T> call, T param, float time)
        {
            if (time <= 0f)
                yield return 0f;
            else
                yield return new WaitForSeconds(time);
            // CBUG.Do("Delaying function call by " + time + " seconds.");
            call?.Invoke(param);
        }

        private Tools _getSelf()
        {
            return this;
        }

        #endregion
    }
}
//NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.

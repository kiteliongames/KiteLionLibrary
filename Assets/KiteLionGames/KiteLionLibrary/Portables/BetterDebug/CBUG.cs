#region FileHeader

// test
// Project: Assembly-CSharp
// File:    CBUG.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.53
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
using System.Collections.Generic;
using JetBrains.Annotations;
using KiteLionGames.KiteLionLibrary.Portables.Legal.Legal;
using KiteLionGames.KiteLionLibrary.Portables.Toolbox;
using KiteLionGames.KiteLionLibrary.Portables.Utilities.Scripts;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace KiteLionGames.KiteLionLibrary.Portables.BetterDebug
{
    /// <summary>
    ///     A statically available debugger for on-screen data visualization.
    ///     Focus on ease-of-use and not optimized.
    ///     todo - add support for editor-time calls
    ///     - Eliot Carney-Seim
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class CBUG : MonoBehaviour, ILegal
    {
        /// <summary>
        ///     THIS IS DANGEROUS USAGE! USE ONLY FOR DEBUGGING!
        ///     REMOVE ALL REFERENCES TO THIS BEFORE RELEASE!
        /// </summary>
        [ItemNotNull]
        public static object[] INSPECT_OBJECTS = new object[10];
        //NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBILITY AND SUPPORT OF THIS SOFTWARE BY KITELIONGAMES LLC AND IT'S PARTNERS.

        // Use this for initialization ...                                                                                                                                                                                                    *whispers* "Ganbare"
        private void Awake()
        {
            _Lines = new LinkedList<string>();
            _Cocurrences = new LinkedList<int>();

            _LogText = this.GetComponent<Text>();
            if (Application.isEditor && Options.isEnabledForEditor == false)
            {
                Options.isEnabledOnScreen = false;
                Do("In-Editor, CBUG Disabled!");
                _LogText.color = new Color(0, 0, 0, 0);
            }
            if (Application.isEditor == false && Debug.isDebugBuild && Options.isEnabledForDevelopmentBuild == false)
            {
                Options.isEnabledOnScreen = false;
                Do("CBUG Disabled for this Development build.");
                _LogText.color = new Color(0, 0, 0, 0);
            }
            if (Application.isEditor == false && Debug.isDebugBuild == false && Options.isEnabledForReleaseBuild == false)
            {
                Options.isEnabledOnScreen = false;
                Do("CBUG Disabled, development turned off.");
                _LogText.color = new Color(0, 0, 0, 0);
            }


            if (Options.clearLineTimeInSeconds == 0)
                _neverClear = true;

            this.transform.tag = "CBUG";
            _previousClear = Time.time;

            Application.logMessageReceived += HandleUnityLogHelper;

            _maxLines = 33;//Tested, based on 24pt Min.
            DontDestroyOnLoad(this.transform.parent);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= HandleUnityLogHelper;
        }

        private void Start() { Copyright.RecordUsage(this); }

        // Update is called once per frame
        private void Update()
        {
            if (!Options.isEnabledOnScreen)
                return;

            if (Options.clearNow)
            {
                Options.clearNow = false;
                ClearLinesHelper(-1);
            }

            //todo - add support for editor-time calls
            //todo CanvasGroup name finding??? ewwww - Eliot
            if (!_isParented && GameObject.Find("CanvasGroup") is not null)
            {
                _isParented = true;
                GameObject.Find("CanvasGroup").transform.SetParent(this.transform, true);
            }

            _LogText.text = "";
            _TempLinesIter = _Lines.First;
            _TempOccurIter = _Cocurrences.First;
            for (var x = 0; x < _Lines.Count; x++)
            {
                if (_TempLinesIter != null)
                {
                    if (_TempOccurIter != null)
                        _LogText.text += _TempLinesIter.Value + " || " + _TempOccurIter.Value + "\n";
                    _TempLinesIter = _TempLinesIter.Next;
                }
                _TempOccurIter = _TempOccurIter?.Next;
            }

            if (_Lines.Count > _maxLines)
            {
                for (var x = 0; x < _Lines.Count - _maxLines; x++)
                {
                    _Lines.RemoveFirst();
                    _Cocurrences.RemoveFirst();
                }
            }

            if (!_neverClear && Time.time - _previousClear > Options.clearLineTimeInSeconds)
            {
                Options.clearNow = true;
                _previousClear = Time.time;
            }
        }

        //NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBILITY AND SUPPORT OF THIS SOFTWARE BY KITELIONGAMES LLC AND IT'S PARTNERS.
        public string KiteLionGamesSoftwareName { get => "CBUG"; }

        /// <summary>
        /// Consumes a Unity Console Message and sends to CBUG.
        /// </summary>
        /// <param name="LogString"></param>
        /// <param name="StackTrace"></param>
        /// <param name="type"></param>
        private static void HandleUnityLogHelper(string LogString, string StackTrace, LogType type)
        {
            var CBUGRef = GetRef();
            var prevSet = CBUGRef.Options.isEnabledForUnityLog;
            // Log methods print to Unity Console. Set this to false temporarily to prevent inf. recurs.
            CBUGRef.Options.isEnabledForUnityLog = false;
            
            var line = "UnityConsole::" + type + ": " + LogString;
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    LogError(line + "/n" + StackTrace);
                    break;
                case LogType.Assert:
                case LogType.Warning:
                case LogType.Log:
                default:
                    Log(line);
                    break;
            }
            GetRef().Options.isEnabledForUnityLog = prevSet;
        }

        #region Public Unity-Assigned Vars

        public static bool isSceneLoaded;
        public static int CCounter = 0;
        public CBUGOptions Options;

        #endregion

        #region Private Vars

        private Text _LogText;
        private LinkedList<string> _Lines;
        private LinkedList<int> _Cocurrences;
        private LinkedListNode<string> _TempLinesIter;
        private LinkedListNode<int> _TempOccurIter;
        private bool _isParented;
        private float _previousClear;
        private bool _neverClear;
        private int _maxLines;
        private bool _isClearingPaused;
        private static readonly LinkedList<string> _preSceneLoadedLines = new LinkedList<string>();
        private enum _printType
        {
            Log,
            Error,
            Warning,
            Exception,
            SeriousError,
            LogToFile,
        }

        #endregion

        #region Debug Aliases

        public static void Log(object line, bool trimLinesForBrevity = true)
        {
            SafeToStringHelper(line, out var temp);
            if (trimLinesForBrevity && temp.Length > GetRef().Options.maxCharactersPerLine)
                temp = temp[..GetRef().Options.maxCharactersPerLine] + "...";
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.PrintHelper(temp, _printType.Log);
            else
                UnityPrintHelper(temp, _printType.Log);
        }

        /// <summary>
        ///     Is CBUG.Log. Sometimes after a million times of typing CBUG.Log, you just want to type CBUG.Do.
        /// </summary>
        /// <param name="line"></param>
        public static void Do(object line)
        {
            Log(line);
        }

        #endregion

        #region Public Static Functions

        /// <summary>
        ///     -1 for All lines.
        /// </summary>
        /// <param name="amount"></param>
        public static void ClearLines(int amount)
        {
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.ClearLinesHelper(amount);
        }

        /// <summary>
        ///     Alias for CBUG.Log;
        /// </summary>
        /// <param name="line"></param>
        public static void Print(string line)
        {
            Do(line);
        }

        /// <summary>
        ///     Red-ify the Debug text. But don't throw an Exception.
        /// </summary>
        /// <param name="line"></param>
        public static void Error(string line)
        {
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.ErrorHelper(line);
            else
                UnityPrintHelper(line, _printType.Error);
        }

        /// <summary>
        ///     Like Error, but also actually throws an Exception given the Line.
        /// </summary>
        /// <param name="line"></param>
        public static void SeriousError(string line)
        {
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.SeriousErrorHelper(line);
            else
                UnityPrintHelper(line, _printType.SeriousError);
        }

        /// <summary>
        ///     Wrapper for Logging library Logging.WriteOnce().
        ///     <see cref="Logging.WriteOnce" />
        /// </summary>
        /// <param name="line"></param>
        public static void LogToFile(string line)
        {
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.LogToFileHelper(line);
        }

        /// <summary>
        ///     Lazy implementation of Debug.LogWarning. Prints to CBUG.Do
        /// </summary>
        /// <param name="line"></param>
        public static void LogWarning(string line)
        {
            Do(line);
        }

        /// <summary>
        ///     Lazy implementation of Debug.LogError. Prints to CBUG.Error
        /// </summary>
        /// <param name="line"></param>
        public static void LogError(string line)
        {
            Error(line);
        }

        public static bool? DEBUG_ON
        {
            get => GetRef() == null ? null : GetRef().Options.isEnabledOnScreen;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnSceneLoaded()
        {
            isSceneLoaded = true;
            foreach (var item in _preSceneLoadedLines)
            {
                Do(item);
            }
            _preSceneLoadedLines.Clear();
        }

        #endregion

        #region Helper Functions

        private void ClearLinesHelper(int amount)
        {
            if (_Lines.Count == 0)
                return;

            if (amount == -1)
            {
                _Lines.Clear();
                _Cocurrences.Clear();
            }
            else
            {
                amount = amount > _Lines.Count ? _Lines.Count : amount;
                for (var x = 0; x < amount; x++)
                {
                    _Lines.RemoveFirst();
                    _Cocurrences.RemoveFirst();
                }
            }
        }

        private static void SafeToStringHelper(object line, out string temp)
        {
            temp = line == null ? "null" : line.ToString();
            if (temp.Length == 0)
                temp = "Empty String";
        }

        private static CBUG GetRef()
        {
            if (isSceneLoaded == false)
            {
                _preSceneLoadedLines.AddLast("CBUG Created");
                return null;
            }

            var myCBUG = GameObject.FindGameObjectWithTag(nameof(CBUG));
            if (myCBUG is not null)
                return myCBUG.GetComponent<CBUG>();
            myCBUG = Instantiate(Resources.Load("CBUG_Canvas") as GameObject);
            //GameObject cbugCam = Instantiate(Resources.Load("CBUG_Camera") as GameObject);
            //cbugCam.GetComponent<Camera>().
            //myCBUG.GetComponent<Canvas>().worldCamera = cbugCam.GetComponent<Camera>();
            //DontDestroyOnLoad(cbugCam);
            DontDestroyOnLoad(myCBUG);
            DontDestroyThis.List.Add(myCBUG);
            return myCBUG.GetComponentInChildren<CBUG>();
        }

        private static void UnityPrintHelper(string line, _printType printType)
        {
            if (printType == _printType.Log)
                Debug.Log(line);
            else if (printType == _printType.Error)
                Debug.LogError(line);
            else if (printType == _printType.Warning)
                Debug.LogWarning(line);
            else if (printType == _printType.Exception)
                Debug.LogException(new Exception(line));
            else if (printType == _printType.SeriousError)
                Debug.LogError(line);
            else if (printType == _printType.LogToFile)
                Debug.Log(line);
            else
                Debug.Log(line);
        }

        //todo code re-use! this method shares logic
        private void PrintHelper(string line, _printType printType)
        {
            if (Options.isEnabledOnConsole == false)
                return;

            if (Options.isEnabledForUnityLog)
            {
                UnityPrintHelper(line, printType);
            }

            if (isSceneLoaded == false)
            {
                _preSceneLoadedLines.AddLast(line);
                return;
            }

            if (_Lines.Find(line) != null)
            {
                _TempLinesIter = _Lines.First;
                _TempOccurIter = _Cocurrences.First;
                for (var x = 0; x < GetRef()._Lines.Count; x++)
                {
                    if (_TempLinesIter != null && _TempLinesIter.Value == line)
                    {
                        if (_TempOccurIter != null)
                            _TempOccurIter.Value++;
                        break;
                    }
                    _TempLinesIter = _TempLinesIter?.Next;
                    _TempOccurIter = _TempOccurIter?.Next;
                }
            }
            else
            {
                _Lines.AddLast(line);
                _Cocurrences.AddLast(1);
            }
        }

        private void ErrorHelper(string line)
        {
            PrintHelper("ERROR <~> " + line, _printType.Error);
        }

        private void SeriousErrorHelper(string line)
        {
            PrintHelper("ERROR <~> " + line, _printType.SeriousError);
            throw new Exception("ERROR <~> " + line);
        }

        private void LogToFileHelper(string line)
        {
            //StreamWriter writer = new StreamWriter(Application.persistentDataPath + 'MyLogFile.txt');
            //writer.WriteLine("CardPlusEngineFile created using StreamWriter class.");
            //writer.Close();
            CBUG.Do("Writing to log: \"" + line + "\"");
            Logging.WriteOnce(line, "Logs", "Log");
        }

        #endregion
    }
}

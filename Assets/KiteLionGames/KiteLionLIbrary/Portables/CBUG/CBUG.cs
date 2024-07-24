using KiteLionGames.Common;
using KiteLionGames.Legal;
using KiteLionGames.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KiteLionGames.BetterDebug
{
    /// <summary>
    /// A statically available debugger for on-screen data visualization.
    /// Focus on ease-of-use and not optimized.
    /// todo - add support for editor-time calls
    ///  - Eliot Carney-Seim
    /// </summary>
    ///
    [RequireComponent(typeof(Text))]
    public class CBUG : MonoBehaviour, ILegal
    {

        #region Public Unity-Assigned Vars
        public static bool isSceneLoaded = false;
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
        private static LinkedList<string> _preSceneLoadedLines = new();
        private enum _printType { Log, Error, Warning, Exception, SeriousError, LogToFile };
        #endregion

        public static GameObject self;
        /// <summary>
        /// THIS IS DANGEROUS USAGE! USE ONLY FOR DEBUGGING!
        /// REMOVE ALL REFERENCES TO THIS BEFORE RELEASE!
        /// </summary>
        public static object[] INSPECT_OBJECTS = new Object[10];

        //NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELIONGAMES LLC AND IT'S PARTNERS.
        public string KiteLionGamesSoftwareName { get => "CBUG"; }
        void Start() { Copyright.RecordUsage(this); }
        //NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELIONGAMES LLC AND IT'S PARTNERS.

        // Use this for initialization ...                                                                                                                                                                                                    *whispers* "Ganbare"
        void Awake()
        {
            _Lines = new LinkedList<string>();
            _Cocurrences = new LinkedList<int>();

            _LogText = GetComponent<Text>();
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

            transform.tag = "CBUG";
            _previousClear = Time.time;

            Application.logMessageReceived += HandleUnityLog;

            _maxLines = 33; //Tested, based on 24pt Min.
            DontDestroyOnLoad(transform.parent);
        }

        // Update is called once per frame
        void Update()
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
            if (!_isParented && GameObject.Find("CanvasGroup") != null)
            {
                _isParented = true;
                GameObject.Find("CanvasGroup").transform.SetParent(transform, true);
            }

            _LogText.text = "";
            _TempLinesIter = _Lines.First;
            _TempOccurIter = _Cocurrences.First;
            for (int x = 0; x < _Lines.Count; x++)
            {
                _LogText.text += _TempLinesIter.Value + " || " + _TempOccurIter.Value + "\n";
                _TempLinesIter = _TempLinesIter.Next;
                _TempOccurIter = _TempOccurIter.Next;
            }

            if (_Lines.Count > _maxLines)
            {
                for (int x = 0; x < _Lines.Count - _maxLines; x++)
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

        public void HandleUnityLog(string LogString, string StackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    Error(type.ToString() + LogString + "/n" + StackTrace);
                    break;
                case LogType.Exception:
                    Error(type.ToString() + LogString + "/n" + StackTrace);
                    break;
                default:
                    break;
            }
        }

        #region Debug Aliases
        public static void Log(object line, bool trimLinesForBrevity = true)
        {
            SafeToStringHelper(line, out string temp);
            if (trimLinesForBrevity && temp.Length > GetRef().Options.maxCharactersPerLine)
                temp = temp[..GetRef().Options.maxCharactersPerLine] + "...";
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.PrintHelper(temp, _printType.Log);
            else
                UnityPrintHelper(temp, _printType.Log);
        }

        /// <summary>
        /// Is CBUG.Log. Sometimes after a million times of typing CBUG.Log, you just want to type CBUG.Do.
        /// </summary>
        /// <param name="line"></param>
        public static void Do(object line)
        {
            Log(line);
        }
        #endregion

        #region Public Static Functions
        /// <summary>
        /// -1 for All lines.
        /// </summary>
        /// <param name="amount"></param>
        public static void ClearLines(int amount)
        {
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.ClearLinesHelper(amount);
        }

        /// <summary>
        /// Alias for CBUG.Log;
        /// </summary>
        /// <param name="line"></param>
        public static void Print(string line)
        {
            Do(line);
        }

        /// <summary>
        /// Red-ify the Debug text. But don't throw an Exception.
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
        /// Like Error, but also actually throws an Exception given the Line.
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
        /// Wrapper for Logging library Logging.WriteOnce().
        /// <see cref="Logging.WriteOnce(string, string, string)"/>
        /// </summary>
        /// <param name="line"></param>
        public static void LogToFile(string line)
        {
            var CBUG = GetRef();
            if (CBUG != null)
                CBUG.LogToFileHelper(line);
        }

        /// <summary>
        /// Lazy implementation of Debug.LogWarning. Prints to CBUG.Do
        /// </summary>
        /// <param name="line"></param>
        public static void LogWarning(string line)
        {
            Do(line);
        }

        /// <summary>
        /// Lazy implementation of Debug.LogError. Prints to CBUG.Error
        /// </summary>
        /// <param name="line"></param>
        public static void LogError(string line)
        {
            Error(line);
        }


        public static bool? DEBUG_ON
        {
            get
            {
                return GetRef() == null ? null : GetRef().Options.isEnabledOnScreen;
            }
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
                for (int x = 0; x < amount; x++)
                {
                    _Lines.RemoveFirst();
                    _Cocurrences.RemoveFirst();
                }
            }
        }
        private static void SafeToStringHelper(object line, out string temp)
        {
            if (line == null)
                temp = "null";
            else
                temp = line.ToString();
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

            GameObject myCBUG = GameObject.FindGameObjectWithTag("CBUG");
            if (myCBUG == null)
            {
                myCBUG = Instantiate(Resources.Load("CBUG_Canvas") as GameObject);
                //GameObject cbugCam = Instantiate(Resources.Load("CBUG_Camera") as GameObject);
                //cbugCam.GetComponent<Camera>().
                //myCBUG.GetComponent<Canvas>().worldCamera = cbugCam.GetComponent<Camera>();
                self = myCBUG;
                //DontDestroyOnLoad(cbugCam);
                DontDestroyOnLoad(myCBUG);
                DontDestroyThis.List.Add(myCBUG);
                return myCBUG.GetComponentInChildren<CBUG>();
            }
            else
            {
                return myCBUG.GetComponent<CBUG>();
            }
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
                Debug.LogException(new System.Exception(line));
            else if (printType == _printType.SeriousError)
                Debug.LogError(line);
            else if (printType == _printType.LogToFile)
                Debug.Log(line);
            else
                Debug.Log(line);
        }
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
                for (int x = 0; x < GetRef()._Lines.Count; x++)
                {
                    if (_TempLinesIter.Value == line)
                    {
                        _TempOccurIter.Value++;
                        break;
                    }
                    _TempLinesIter = _TempLinesIter.Next;
                    _TempOccurIter = _TempOccurIter.Next;
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
            throw new System.Exception("ERROR <~> " + line);
        }
        private void LogToFileHelper(string line)
        {
            //StreamWriter writer = new StreamWriter(Application.persistentDataPath + 'MyLogFile.txt');
            //writer.WriteLine("CardPlusEngineFile created using StreamWriter class.");
            //writer.Close();
            Logging.WriteOnce(line, "Logs", "Log");
        }
        #endregion
    }
}
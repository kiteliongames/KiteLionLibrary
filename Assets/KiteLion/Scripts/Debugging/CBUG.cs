using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace KiteLion {

    namespace Debugging {
    
        /// <summary>
        /// A statically available debugger for on-screen data visualization.
        /// Focus on ease-of-use and not optimized.
        ///  - Eliot Carney-Seim
        /// </summary>
        ///
        [RequireComponent (typeof(Text))]
        public class CBUG : MonoBehaviour {

            #region Public Unity-Assigned Vars
            public bool Enabled;
            public bool EnabledOnConsole;
            public bool EnabledOnScreen;
            public float ClearTime;
            public int ClearLinesAmount;
            public bool ClearScreenButton;
            #endregion

            #region Private Vars
            private bool forceShow;
            private Text logText;
            private LinkedList<string> lines;
            private LinkedList<int> occurrences;
            private LinkedListNode<string> tempLinesIter;
            private LinkedListNode<int> tempOccurIter;
            private bool isParented;
            private float previousClear;
            private bool neverClear;
            private int maxLines; 
            private int tapsUntilEnable;
            private int currentTaps;
            private bool isTemp;
            #endregion

            public static GameObject self;

            // Use this for initialization ...                                                                                                                                                                                                    *whispers* "Kandare"
            void Awake()
            {
                forceShow = false;
                Enabled = true;
                EnabledOnScreen = true;
                EnabledOnConsole = true;
                logText = GetComponent<Text>();
                lines = new LinkedList<string>();
                occurrences = new LinkedList<int>();
                if (!EnabledOnScreen)
                    logText.color = new Color(0, 0, 0, 0);
                if (ClearTime == 0)
                    neverClear = true;

                transform.tag = "CBUG";
                previousClear = Time.time;
                isTemp = false;

                Application.logMessageReceived += HandleUnityLog;

                maxLines = 33; //Tested, based on 24pt Min.
                tapsUntilEnable = 10;
                currentTaps = 0;
                DontDestroyOnLoad(transform.parent);
            }

            private CBUG( bool isTemp)
            {
                if (isTemp)
                    this.isTemp = true;
            }

            void Start()
            {
                forceShow = (PlayerPrefs.GetInt("CBUG_ON", 0) == 1);
                if (forceShow)
                    CBUG.Do("CBUG ENABLED MANUALLY!");
            }

            // Update is called once per frame
            void Update()
            {

                if (!Enabled && !forceShow)
                    return;

                if (!Application.isEditor && !forceShow)
                {
                    Enabled = false;
                    Do("In-Build, CBUG Disabled!");
                    return;
                }

                if (ClearScreenButton)
                {
                    ClearScreenButton = false;
                    _ClearLines(-1);
                }

                if (!isParented && GameObject.Find("CanvasGroup") != null)
                {
                    isParented = true;
                    GameObject.Find("CanvasGroup").transform.SetParent(transform, true);
                }

                logText.text = "";
                tempLinesIter = lines.First;
                tempOccurIter = occurrences.First;
                for (int x = 0; x < lines.Count; x++)
                {
                    logText.text += tempLinesIter.Value + " || " + tempOccurIter.Value + "\n";
                    tempLinesIter = tempLinesIter.Next;
                    tempOccurIter = tempOccurIter.Next;
                }

                if (lines.Count > maxLines)
                {
                    for (int x = 0; x < lines.Count - maxLines; x++)
                    {
                        lines.RemoveFirst();
                        occurrences.RemoveFirst();
                    }
                }

                if (!neverClear && Time.time - previousClear > ClearTime)
                {
                    ClearScreenButton = true;
                    previousClear = Time.time;
                }
            }

            public void HandleUnityLog(string LogString, string StackTrace, LogType type)
            {
                switch (type)
                {
                    case LogType.Error:
                        CBUG.Error(type.ToString() + LogString + "/n" + StackTrace);
                        break;
                    case LogType.Exception:
                        CBUG.Error(type.ToString() + LogString + "/n" + StackTrace);
                        break;
                    default:
                        break;
                }
            }

            public void EnableCBUG ()
            {
                currentTaps++;
                if (currentTaps >= tapsUntilEnable)
                {
                    if (forceShow)
                        PlayerPrefs.SetInt("CBUG_ON", 0);
                    else
                        PlayerPrefs.SetInt("CBUG_ON", 1);

                    PlayerPrefs.Save();
                    Application.Quit();
                }
            }

            #region Debug Aliases
            public static void Log(string line)
            {
                GetRef()._Print(line);
            }

            public static void Do(string line)
            {
                GetRef()._Print(line);
            }

            public static void Log(string line, bool debugOn)
            {
                GetRef()._Print(line, debugOn);
            }

            public static void Do(string line, bool debugOn)
            {
                GetRef()._Print(line, debugOn);
            }
            #endregion

            #region Helper Functions
            private void _ClearLines(int amount)
            {
                if (lines.Count == 0)
                    return;

                if(amount == -1) {
                    lines.Clear();
                    occurrences.Clear();
                }
                else 
                {
                    amount = amount > lines.Count ? lines.Count : amount;
                    for(int x = 0; x < amount; x++) {
                        lines.RemoveFirst();
                        occurrences.RemoveFirst();
                    }
                }
            }

            private static CBUG GetRef()
            {
                GameObject myCBUG = GameObject.FindGameObjectWithTag("CBUG");
                if(myCBUG == null)
                {
                    myCBUG = Instantiate(Resources.Load("CBUG_Canvas") as GameObject);
                    //GameObject cbugCam = Instantiate(Resources.Load("CBUG_Camera") as GameObject);
                    //cbugCam.GetComponent<Camera>().
                    //myCBUG.GetComponent<Canvas>().worldCamera = cbugCam.GetComponent<Camera>();
                    self = myCBUG;
                    //DontDestroyOnLoad(cbugCam);
                    DontDestroyOnLoad(myCBUG);
                    return myCBUG.GetComponentInChildren<CBUG>();
                }
                else
                {
                    return myCBUG.GetComponent<CBUG>();
                }

            }

            private void _Print(string line)
            {

                if (isTemp)
                {
                    Debug.Log(line);
                    return;
                }

                if (!Enabled)
                    return;

                if(EnabledOnConsole)
                    Debug.Log(line);

                if (lines.Find(line) != null) {
                    tempLinesIter = lines.First;
                    tempOccurIter = occurrences.First;
                    for (int x = 0; x < GetRef().lines.Count; x++) {
                        if (tempLinesIter.Value == line) {
                            tempOccurIter.Value++;
                            break;
                        }
                        tempLinesIter = tempLinesIter.Next;
                        tempOccurIter = tempOccurIter.Next;
                    }
                } else {
                    lines.AddLast(line);
                    occurrences.AddLast(1);
                }
            }

            private void _Print(string line, bool debugOn)
            {
                if (Enabled && debugOn) {
                    if (line == null)
                        _Print("Null @ " + System.Environment.StackTrace);
                    else
                        _Print(line);
                }
            }

            private void _Error(string line)
            {
                _Print("ERROR <~> " + line);
                Debug.Log("ERROR <~> " + line);
            }

            private void _SrsError(string line)
            {
                _Error(line);
                //if (logText != null) {
                //    logText.color = Color.red;
                //}
                throw new System.Exception("ERROR <~> " + line);
            }
            #endregion

            #region Public Static Functions
            /// <summary>
            /// -1 for All lines.
            /// </summary>
            /// <param name="amount"></param>
            public static void ClearLines(int amount)
            {
                GetRef()._ClearLines(amount);
            }

            public static void Print(string line)
            {
                GetRef()._Print(line);
            }

            public static void Print(string line, bool debugOn)
            {
                GetRef()._Print(line, debugOn);
            }

            /// <summary>
            /// Red-ify the Debug text. But don't throw an Exception.
            /// </summary>
            /// <param name="line"></param>
            public static void Error(string line)
            {
                GetRef()._Error(line);
            }

            /// <summary>
            /// Like Error, but also actually throws an Exception given the Line.
            /// </summary>
            /// <param name="line"></param>
            public static void SrsError(string line)
            {
                GetRef()._SrsError(line);
            }

            public static bool DEBUG_ON
            {
                get {
                    return GetRef().Enabled;
                }
            }
            #endregion
        }
    }
}
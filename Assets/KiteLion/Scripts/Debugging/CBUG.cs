using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using KiteLion.Legal;

/// <summary>
/// A statically available debugger for on-screen data visualization.
/// Focus on ease-of-use and not optimized.
///  - Eliot Carney-Seim
/// </summary>
///
[RequireComponent(typeof(Text))]
public class CBUG : MonoBehaviour, ILegal {

    #region Public Unity-Assigned Vars
    public bool    _enabled                    = true;
    public bool    _enabledForUnityLog         = true;
    public bool    _enabledForEditor           = true;
    //private bool    _enabledForDevelopmentBuild = true; //unused
    //private bool    _enabledForReleaseBuild     = true; //unused
    public float   _clearLineTimeInSeconds     = 10;
    public bool    _clearNow                   = false;
    #endregion

    #region Private Vars
    private Text                    _LogText;
    private LinkedList<string>      _Lines;
    private LinkedList<int>         _Cccurrences;
    private LinkedListNode<string>  _TempLinesIter;
    private LinkedListNode<int>     _TempOccurIter;
    private bool                    _isParented;
    private float                   _previousClear;
    private bool                    _neverClear;
    private int                     _maxLines;
    private int                     _tapsUntilEnable;
    private int                     _currentTaps;
    private bool                    _isTemp;
    private bool _isClearingPaused;
    #endregion

    public static GameObject self;

    //NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELIONGAMES LLC AND IT'S PARTNERS.
    public string KiteLionGamesSoftwareName { get => "CBUG"; }
    void Start() { Copyright.RecordUsage(this); }
    //NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELIONGAMES LLC AND IT'S PARTNERS.

    // Use this for initialization ...                                                                                                                                                                                                    *whispers* "Ganbare"
    void Awake()
    {
        _Lines = new LinkedList<string>();
        _Cccurrences = new LinkedList<int>();

        _LogText = GetComponent<Text>();
        if (_enabledForEditor == false)
            _LogText.color = new Color(0, 0, 0, 0);
        if (_clearLineTimeInSeconds == 0)
            _neverClear = true;

        transform.tag = "CBUG";
        _previousClear = Time.time;
        _isTemp = false;

        Application.logMessageReceived += HandleUnityLog;

        _maxLines = 33; //Tested, based on 24pt Min.
        _tapsUntilEnable = 10;
        _currentTaps = 0;
        DontDestroyOnLoad(transform.parent);
    }

    private CBUG( bool isTemp)
    {
        if (isTemp)
            this._isTemp = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (!_enabled)
            return;

        if (!Application.isEditor)
        {
            _enabled = false;
            Do("In-Build, CBUG Disabled!");
            return;
        }

        if (_clearNow)
        {
            _clearNow = false;
            _ClearLines(-1);
        }

        if (!_isParented && GameObject.Find("CanvasGroup") != null)
        {
            _isParented = true;
            GameObject.Find("CanvasGroup").transform.SetParent(transform, true);
        }

        _LogText.text = "";
        _TempLinesIter = _Lines.First;
        _TempOccurIter = _Cccurrences.First;
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
                _Cccurrences.RemoveFirst();
            }
        }

        if (!_neverClear && Time.time - _previousClear > _clearLineTimeInSeconds)
        {
            _clearNow = true;
            _previousClear = Time.time;
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
        _currentTaps++;
        if (_currentTaps >= _tapsUntilEnable)
        {
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
        if (_Lines.Count == 0)
            return;

        if(amount == -1) {
            _Lines.Clear();
            _Cccurrences.Clear();
        }
        else 
        {
            amount = amount > _Lines.Count ? _Lines.Count : amount;
            for(int x = 0; x < amount; x++) {
                _Lines.RemoveFirst();
                _Cccurrences.RemoveFirst();
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
            DontDestroyThis.List.Add(myCBUG);
            return myCBUG.GetComponentInChildren<CBUG>();
        }
        else
        {
            return myCBUG.GetComponent<CBUG>();
        }

    }

    private void _Print(string line)
    {

        if (_isTemp)
        {
            Debug.Log(line);
            return;
        }

        if (!_enabled)
            return;

        if(_enabledForUnityLog)
            Debug.Log(line);

        if (_Lines.Find(line) != null) {
            _TempLinesIter = _Lines.First;
            _TempOccurIter = _Cccurrences.First;
            for (int x = 0; x < GetRef()._Lines.Count; x++) {
                if (_TempLinesIter.Value == line) {
                    _TempOccurIter.Value++;
                    break;
                }
                _TempLinesIter = _TempLinesIter.Next;
                _TempOccurIter = _TempOccurIter.Next;
            }
        } else {
            _Lines.AddLast(line);
            _Cccurrences.AddLast(1);
        }
    }

    private void _Print(string line, bool debugOn)
    {
        if (_enabled && debugOn) {
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
            return GetRef()._enabled;
        }
    }

    #endregion
}

/// Programmer: Eliot Carney-Seim
/// Date: April 3rd, 2017
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileHelpers;

/// <summary>
/// --NEEDS UPDATING WWW DEPRECATED---
/// Handles all parsing of the CSV for access by other classes.
/// </summary>
public class ParseManager : MonoBehaviour {

    #region Publicly Assigned
    [Tooltip("Total states, provinces, and military bases. ")]
    public int TotalStates; //Note/ToDo: Minor Outlying Islands not supported!
    [Tooltip("Filename for CSV containing codes for states, provinces, and military bases.")]
    public string StateCSV;
    [Tooltip("Filename for CSV containing cities sorted alphabetically to their name, state and ID.")]
    public string CityCSV;
    #endregion

    #region FileHelpers CSV Classes
    [DelimitedRecord(",")]
    private class CityData
    {
        public int CriteriaID;
        public string City;
        public string State;
    }

    [DelimitedRecord(",")]
    private class StateData
    {
        public int ID;
        public string State;
        public string StateAbbreviation;
    }
    #endregion

    #region Privates
    private string rootPath;
    private string cityCSVPath;
    private string stateCSVPath;
    private FileHelperEngine<CityData> engine_CityData;
    private FileHelperEngine<StateData> engine_StateData;
    private CityData[] _cityData;
    private StateData[] _stateData;
    private string cityCSVWeb;
    private string stateCSVWeb;
    private bool dataLoaded;
    private int currentState;
    private int currentCity;
    private Dictionary<string, int> StateNameToID;
    private Dictionary<string, int> StateAbbrToID;
    private List<City>[] cityDataByState;
    #endregion

    #region Statics
    public static bool DoneInstantiating = false;
    #endregion

    #region Subclasses
    [System.Serializable]
    private class City
    {
        public int CityID;
        public int StateID;
        public string CityName;
        public string StateName;      
        public string StateAbbreviation;

        public City (int CityID, int StateID, string CityName, string StateName, string StateAbbreviation)
        {
            this.CityID = CityID;
            this.StateID = StateID;
            this.CityName = CityName;
            this.StateName = StateName;
            this.StateAbbreviation = StateAbbreviation;
        }
    }
    #endregion

    // Use this for initialization
    void Start () {
        if (GameObject.FindGameObjectsWithTag("ParseManager").Length > 1)
            CBUG.SrsError("There must be only one ParseManager per scene!");

        rootPath = Application.streamingAssetsPath;
        cityCSVPath = System.IO.Path.Combine(rootPath, CityCSV);
        CBUG.Do("CityCSVPath: " + cityCSVPath);
        stateCSVPath = System.IO.Path.Combine(rootPath, StateCSV);
        //for testing:
        //Application.ExternalEval("window.open('" + stateCSVPath + "');");
        CBUG.Do("StateCSVPath: " + stateCSVPath);
        cityDataByState = new List<City>[TotalStates];
        fillArrayWithClass<List<City>>(ref cityDataByState);
        engine_CityData = new FileHelperEngine<CityData>();
        engine_StateData = new FileHelperEngine<StateData>();
        StateNameToID = new Dictionary<string, int>();
        StateAbbrToID = new Dictionary<string, int>();
        dataLoaded = false;
        currentCity = -1;
        currentState = 0;
        if (Application.isEditor)
        {
            _cityData = engine_CityData.ReadFile(cityCSVPath);
            _stateData = engine_StateData.ReadFile(stateCSVPath);
            makeCityList();
        }
        else
        {
                
            //"http://paultproductions.com/trackerS2/StreamingAssets/IDToCityToState.csv"
            StartCoroutine(loadCityDataWeb(cityCSVPath));
            StartCoroutine(loadStateDataWeb(stateCSVPath));
        }
    }

    // Update is called once per frame
    void Update () {
        if(!DoneInstantiating)
        {
            if (_cityData != null && _stateData != null && dataLoaded)
                DoneInstantiating = true;

            if (_cityData == null)
            {
                CBUG.Do("City Data null.");
            }
            else if (_cityData.Length <= 0)
            {
                CBUG.Do("City Data not yet loaded.");
            }
            else
            {
                string temp = "Sampling CSV Data: " + _cityData[0].City + " in " + _cityData[0].State;
                CBUG.Do(temp);
            }

            if (_stateData == null)
            {
                CBUG.Do("State Data null.");
            }
            else if (_stateData.Length > 0)
            {
                CBUG.Do("State Data not yet loaded.");
            }
            else
            {
                string temp2 = "Sampling CSV Data: " + _stateData[0].State + " is " + _stateData[0].StateAbbreviation;
                CBUG.Do(temp2);
            }
        }
    }

    #region Static Ref Functions
    public static List<string> GetStates(bool GetAbbreviations)
    {
        return GameObject.FindGameObjectWithTag("ParseManager").GetComponent<ParseManager>()._getStates(GetAbbreviations);
    }
    private List<string> _getStates(bool getAbbreviations)
    {
        List<string> StateList = new List<string>();
        if (getAbbreviations)
        {
            for (int x = 0; x < _stateData.Length; x++)
            {
                StateList.Add(_stateData[x].StateAbbreviation);
            }
        }
        else
        {
            for (int x = 0; x < _stateData.Length; x++)
            {
                StateList.Add(_stateData[x].State);
            }
        }
        return StateList;
    }

    public static List<string> GetCities(string State, bool IsAbbreviation)
    {
        return GameObject.FindGameObjectWithTag("ParseManager").GetComponent<ParseManager>()._getCities(State, IsAbbreviation);
    }
    private List<string> _getCities(string state, bool isAbbreviation)
    {
        List<string> cities = new List<string>();
        int stateNum = isAbbreviation ? StateAbbrToID[state] : StateNameToID[state];
        for (int x = 0; x < cityDataByState[stateNum].Count; x++)
        {
            if (isAbbreviation)
                cities.Add(cityDataByState[stateNum][x].CityName);
            else
                cities.Add(cityDataByState[stateNum][x].CityName);
        }
        return cities;
    }
    #endregion


    #region Private Helpers
    /// <summary>
    /// Fills a given array with a non-null empty object. 
    /// </summary>
    /// <typeparam name="T">Object with invokable constructor.</typeparam>
    /// <param name="ArrayToEdit">Array will be completely overwritten.</param>
    private void fillArrayWithClass <T> (ref T[] ArrayToEdit) where T : new()
    {
        for(int x = 0; x < ArrayToEdit.Length; x++)
        {
            ArrayToEdit[x] = new T();
        }
    }

    private IEnumerator loadCityDataWeb(string path)
    {
        CBUG.Do("City Path is: " + path);
        WWW www = new WWW(path);
        yield return www;
        cityCSVWeb = www.text;
        CBUG.Do("CityScript is: ");
        CBUG.Do(cityCSVWeb.Substring(0, 20));
        _cityData = engine_CityData.ReadString(cityCSVWeb);
        if (_cityData == null)
            CBUG.Do("OUR LIBRARY DOESN'T WORK");
        else
        {
            CBUG.Do("City DATA_LENGTH: " + _cityData.Length);
        }
    }
    private IEnumerator loadStateDataWeb(string path)
    {
        CBUG.Do("State Path is: " + path);
        WWW www = new WWW(path);
        yield return www;
        stateCSVWeb = www.text;
        _stateData = engine_StateData.ReadString(stateCSVWeb);
        makeCityList();
    }
    private void makeCityList()
    {
        while (!dataLoaded)
        {
            currentCity++;
            cityDataByState[currentState].Add(
                new City(
                    _cityData[currentCity].CriteriaID,
                    _stateData[currentState].ID,
                    _cityData[currentCity].City,
                    _cityData[currentCity].State,
                    _stateData[currentState].StateAbbreviation
                )
            );
            if (_cityData[currentCity].State != _stateData[currentState].State)
            {
                StateNameToID.Add(_stateData[currentState].State, _stateData[currentState].ID);
                StateNameToID.Add(_stateData[currentState].StateAbbreviation, _stateData[currentState].ID);
                currentState++;
            }
            if (currentState >= TotalStates || currentCity >= _cityData.Length - 1)
            {
                dataLoaded = true;
                CBUG.Do("Data Loaded!");
            }
        }
    }
    #endregion
}

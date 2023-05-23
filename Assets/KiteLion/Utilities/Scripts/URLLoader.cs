/// Programmer: Eliot Carney-Seim
/// Date: April 3rd, 2017
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// Generates URL for reading and taking in.
/// 
/// Sample Usage:
/// URLLoader.DataPoint StartPoint = new DataPoint(mylabel, mytime, mycitycode, mystatecode);
/// ...
/// ...
/// URLLoader.DataPoint EndPoint = new DataPoint(mylabel, mytime, mycitycode, mystatecode);
/// URLLoader.BuildURL(StartPoint, middlepint1, middlepiointX, endpoint);
/// 
/// </summary>
public class URLLoader : MonoBehaviour {

    #region Privates
    private string rootURL; //Used for building the URL only.
                            //Sample: http://paultproductions.com/trackerS2/
    #endregion

    #region Statics
    public static string URL; //Order: [date1][citycode1][statecode1][truckid1]...[dateX][citycodeX][statecodeX][truckidX]
                              //Example: trucktracker.com/app/?0401172340167345005987004021700401673451069870
                              // From April 1st, 2017 @ 2340, City 1673450, State 05, Truck ID 9870
                              // {1 Hour Travel To Destination, across State lines}
                              // To April 2nd, 2017 @ 0040, City 1673451, State 06, Truck ID 9870
    public static bool DoneInstatiating = false;
    #endregion

    [System.Serializable]
    public class OneSetData
    {
        //0,1,2
        public string trailerType;
        public string status;

    }

    [System.Serializable]
    public struct DataPoint
    {
        
        public string TruckID;
        //Date then Time
        public string Time;
        public string CityCode;
        public string StateCode;
        public int TotalLength;
        public int TruckIDLength;
        public int TimeLength;
        public int CityCodeLength;
        public int StateCodeLength;

        /// <summary>
        /// An empty default constructor ... what's a class?
        /// </summary>
        /// <param name="ALL_NULL"></param>
        public DataPoint(string ALL_NULL)
        {
            this.TruckID = "";
            this.Time = "";
            this.CityCode = "";
            this.StateCode = "";
            TruckIDLength = 4;
            TimeLength = 10;
            CityCodeLength = 7;
            StateCodeLength = 2;
            TotalLength = TimeLength + TruckIDLength + CityCodeLength + StateCodeLength;
        }

        //Order: [date1] [citycode1] [statecode1] [truckid1]
        public DataPoint(string Time, string CityCode, string StateCode, string TruckID)
        {
            this.TruckID = TruckID;
            this.Time = Time;
            this.CityCode = CityCode;
            this.StateCode = StateCode;
            TruckIDLength = 4;
            TimeLength = 10;
            CityCodeLength = 7;
            StateCodeLength = 2;
            TotalLength = TimeLength + TruckIDLength + CityCodeLength + StateCodeLength;

            if(Time.Length != TimeLength)
            {
                CBUG.SrsError("BAD TIME LENGTH! Must be " + TimeLength + " characters long.");
            }
            if(CityCode.Length != CityCodeLength)
            {
                CBUG.SrsError("BAD CITY CODE LENGTH! Must be " + CityCodeLength + " characters long.");
            }
            if (StateCode.Length != StateCodeLength)
            {
                CBUG.SrsError("BAD STATE CODE LENGTH! Must be " + StateCodeLength + " characters long.");
            }
            if(TruckID.Length != TruckIDLength)
            {
                CBUG.SrsError("BAD TRUCK ID LENGTH! Must be " + TruckIDLength + " characters long.");
            }
        }
    }

	// Use this for initialization
	void Start () {
        if (GameObject.FindGameObjectsWithTag("URLLoader").Length > 1)
            CBUG.SrsError("There must be only one URLLoader per scene!");

        //If the URL contains a '?' then everything before it is the root.
        //If not, then everything is the root.
        if (Application.absoluteURL.Contains("?"))
            rootURL = Application.absoluteURL.Substring(0, Application.absoluteURL.LastIndexOf("?"));
        else
            rootURL = Application.absoluteURL;
        DoneInstatiating = true;
    }

    // Update is called once per frame
    void Update () {

	}

    /// <summary>
    /// Creates the codified string representing all datapoints, assigns it to URLLoader.URL and returns it.
    /// </summary>
    /// <param name="AllPoints">The datapoints class with everything filled out.</param>
    #region Public Static Refs
    public static string BuildURL(params DataPoint[] AllPoints)
    {
        return GameObject.FindGameObjectWithTag("URLLoader").GetComponent<URLLoader>()._buildURL(AllPoints);
    }
    private string _buildURL(params DataPoint[] allPoints)
    {
        string URLTemp = "";
        //Order: [date1] [citycode1] [statecode1] [truckid1]
        for(int x = 0; x < allPoints.Length; x++)
        {
            URLTemp += allPoints[x].Time + allPoints[x].CityCode + allPoints[x].StateCode + allPoints[x].TruckID;
        }
        URL = rootURL + "?" + URLTemp;
        return URL;
    }

    /// <summary>
    /// Converts the codified information from the URL into DataPoints.
    /// This function assumes that the URL contains a querry string (? + code) and if not,
    /// will throw an error. Called without a parameter, it'll use the Absolute URL.
    /// </summary>
    /// <param name="URL">Optional Full URL + Querry String</param>
    /// <returns>The code converted into a list of usable DataPoints</returns>
    public static List<DataPoint> GetDataFrom([Optional] string URL)
    {
        if (URL == null)
            URL = Application.absoluteURL;
        return GameObject.FindGameObjectWithTag("URLLoader").GetComponent<URLLoader>()._getDataFromURL(URL);
    }
    private List<DataPoint> _getDataFromURL(string url)
    {
        if(checkFormat(url) == false)
        {
            CBUG.SrsError("BAD URL! Didn't pass format check!: " + url);
            return null;
        }
        int startPoint = url.LastIndexOf("?");
        string newURL = url.Substring(startPoint + 1);
        DataPoint t = new DataPoint("");
        int dataPointLength = t.TotalLength;
        float totalDataPoints = newURL.Length / dataPointLength;
        List<DataPoint> newPoints = new List<DataPoint>();
        //Order: [date1] [citycode1] [statecode1] [truckid1]
        for (int x = 0; x < totalDataPoints; x++)
        {
            newPoints.Add(new DataPoint(
                newURL.Substring(x * dataPointLength, t.TimeLength),
                newURL.Substring(x * dataPointLength + t.TimeLength, t.CityCodeLength),
                newURL.Substring(x * dataPointLength + t.TimeLength + t.CityCodeLength, t.StateCodeLength),
                newURL.Substring(x * dataPointLength + t.TimeLength + t.CityCodeLength + t.StateCodeLength, t.TruckIDLength)
                )
            );
        }
        return newPoints;
    }
    #endregion

    #region Private Helpers
    private bool checkFormat(string url)
    {
        string newURL;
        if (!url.Contains("?"))
        {
            CBUG.SrsError("URL DOES NOT CONTAIN '?': " + url);
            return false;
        }
        //Making sure there is only 1 "?".
        int qLocation = url.LastIndexOf("?");
        if (url.LastIndexOf("?", qLocation - 1) != -1)
        {
            CBUG.SrsError("URL CONTAINS TOO MANY '?'s: " + url);
            return false;
        }
        int startPoint = url.LastIndexOf("?");
        newURL = url.Substring(startPoint + 1);
        DataPoint t = new DataPoint("");
        int dataPointLength = t.TotalLength;
        if (newURL.Length % dataPointLength != 0)
        {
            CBUG.SrsError("BAD URL: " + url + " ARG LENGTH! Must be divisible by: " + dataPointLength);
            return false;
        }
        return true;
    }
    #endregion
}

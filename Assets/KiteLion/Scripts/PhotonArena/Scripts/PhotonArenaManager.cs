using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;
using System.Collections.Generic;
using Photon.Realtime;
using Random = UnityEngine.Random;
using ExitGames.Client.Photon;
using KiteLion.Debugging;
/*
todo provide structure for setting up scene items and other stuff.
Problem: When referencing a singleton, several variables that exist that could cause issue.
- Are you grabbing the CORRECT reference? Copies could exist because you got the reference before they deleted. OR they didn't delete.
- The reference doesn't exist at all because the OG hasn't even been loaded in yet.
Solution:
- The call to the server requires a list of all singletons (managers) that must eventually be spawned eventually.
- Wrap ALL singleton references in PhotonArenaManager.instance.UseSingleton("ManagerName");
For example, certain manager objects should exist only one per scene.
roomProp "firstTimeSetupComplete = true"
if true, 
*/
public class PhotonArenaManager : Singleton<PhotonArenaManager>
{

    #region Spoof Server
    public struct FakeServer {
        public Dictionary<string, System.Object> DataStore;
        public int totalPlayers;

        public string Username { get; internal set; }
    }
    FakeServer _fakeServer;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public enum ServerDepthLevel {
        Offline,
        InServer,
        InLobby,
        InRoom
    }
    
    public static Vector3 DefaultSpawnLocation = new Vector3(0f, 15f, 0f);

    private bool newData;
    private List<string> singletonNames;
    private Dictionary<string, GameObject> singletons;
    private bool needsNewRoom;

    private RoomOptions roomOptions = new RoomOptions() {
        MaxPlayers = 9,
        CustomRoomProperties = new Hashtable() { {"GameStartTime", -1} },
        CustomRoomPropertiesForLobby = new string[] { "GameStartTime"}
    };

    private ServerDepthLevel currentServerUserDepth = ServerDepthLevel.Offline;
    public static class Constants {
        public static readonly Vector3 DefaultSpawnLoc = Vector3.one;
    }

    void Awake() {
        _fakeServer.DataStore = new Dictionary<string, object>();
        _fakeServer.totalPlayers = 0;

        newData = false;
        singletonNames = new List<string>();
        singletons = new Dictionary<string, GameObject>();
        needsNewRoom = false;
    }

    public void ConnectAndJoinRoom(string username, string[] singletons) {
        if(CurrentServerUserDepth != ServerDepthLevel.Offline) {
            CBUG.Do("Redundant connect call! Already at " + CurrentServerUserDepth.ToString());
            return;
        }

        CBUG.Do("Connecting!");
        PhotonNetwork.GameVersion = "BlueCouch";

        PhotonNetwork.NetworkingClient.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;

        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = username + "_"+UnityEngine.Random.Range(0, 9999);

        string region = "us";
        bool _result = PhotonNetwork.ConnectToRegion(region);

        CBUG.Log("PunCockpit:ConnectToRegion(" + region + ") -> " + _result);
    }

    public void ConnectAndJoinOffline(string username) {
        _fakeServer.Username = username;
    }

    public GameObject UseSingleton(string singletonName) {
        //todo ??? checkifnull!
        //if (singletons.ContainsKey(singletonName)) {
        //    return singletons[singletonName];
        //} else {
        //    //todo ??? what do we do if a singleton doesn't exist? just spawn it? How does everyone else know to put it into their list?
        //}
        return GameObject.FindGameObjectWithTag(singletonName);
    }

    public bool IsHost {
        get {
            if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
                return true;
            } else {
                return PhotonNetwork.LocalPlayer.IsMasterClient;
            }
        }
    }

    public ServerDepthLevel CurrentServerUserDepth {
        get {
            if(PhotonNetwork.InRoom) {
                currentServerUserDepth = ServerDepthLevel.InRoom;
            } else if (PhotonNetwork.InLobby) {
                currentServerUserDepth = ServerDepthLevel.InLobby;
            } else if (!PhotonNetwork.IsConnectedAndReady) {
                currentServerUserDepth = ServerDepthLevel.Offline;
            }
            return currentServerUserDepth;
        }
    }

    /// <summary>
    /// Time in Milliseconds. Use this to sync!
    /// </summary>
    /// <returns>Accurate down to 1/15 of a second.</returns>
    public int GetClock() {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            //            return DateTime.Now.TimeOfDay.Milliseconds;
            return Convert.ToInt32((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) & int.MaxValue);
        }
        else {
            return PhotonNetwork.ServerTimestamp;
        }
    }

    public int GetClockInSeconds() {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            //            return DateTime.Now.TimeOfDay.Milliseconds;
            return Convert.ToInt32((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) & int.MaxValue) / 60;
        }
        else {
            return PhotonNetwork.ServerTimestamp;
        }
    }

    public ServerDepthLevel GetCurrentDepthLevel() {
        return CurrentServerUserDepth;
    }

    public Photon.Realtime.Room GetRoom() {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            return null; //??? TODO HANDLE BETTER
        }
        else if ( CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            return PhotonNetwork.CurrentRoom;
        } else {
            CBUG.Error("We are not currently in a room!");
            return null; //??? TODO HANDLE BETTER!
        }
    }

    public System.Object GetData(string label) {
        //todo ??? Make generic
        bool containsData = true;
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            if (_fakeServer.DataStore.ContainsKey(label)) {
                return _fakeServer.DataStore[label] as System.Object;
            } else {
                newData = false;
                containsData = false;
            }
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            ExitGames.Client.Photon.Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (roomProps.ContainsKey(label)) {
                newData = false;
                return roomProps[label] as System.Object;
            } else {
                containsData = false;
            }
        } else {
            CBUG.Error("GetData only available when Offline or InRoom, this was called at " + CurrentServerUserDepth.ToString() + ".");
        }
        if(containsData == false) {
            CBUG.Error("No data was found for " + label + ".");
        }
        return null;
    }

    public void SaveData(string label, System.Object data) {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            _fakeServer.DataStore.Remove(label);
            _fakeServer.DataStore.Add(label, data);
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            ExitGames.Client.Photon.Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            roomProps.Remove(label);
            roomProps.Add(label, data);
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
        else {
            CBUG.Error("SaveData only available when Offline or InRoom, this was called at  " + CurrentServerUserDepth.ToString() + ".");
        }
    }

    public GameObject SpawnSingletonResource(string singletonName) {
        //todo ??? network-ize this.
        GameObject singleton = SpawnObject(singletonName);
        if(singleton == null) {
            CBUG.SrsError("No singleton is available for name: " + singletonName);
            return null;
        }
        singletons.Add(singletonName, singleton);
        return singleton;
    }

    public void NewRoom() {
        PhotonNetwork.LeaveRoom();
        CBUG.Log("Leaving current room ...");
        needsNewRoom = true;
    }

    public GameObject SpawnObject(string resourceName) {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            GameObject instance = Instantiate(Resources.Load(resourceName, typeof(GameObject)), DefaultSpawnLocation, Quaternion.Euler(Vector3.zero)) as GameObject;
            return instance;
            /// ??? todo make playerlist local ref 
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            GameObject PlayerObj = PhotonNetwork.Instantiate(resourceName, DefaultSpawnLocation, Quaternion.Euler(Vector3.zero));
            return PlayerObj;
        }
        else {
            CBUG.Error("SpawnObject only available when Offline or InRoom, this was called at " + CurrentServerUserDepth.ToString() + ".");
            return null;
        }
    }
    public GameObject SpawnObject(string resourceName, Vector3 spawnLoc, Quaternion spawnRot) {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            GameObject instance = Instantiate(Resources.Load(resourceName, typeof(GameObject)), spawnLoc, spawnRot) as GameObject;
            return instance;
            /// ??? todo make playerlist local ref 
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            GameObject PlayerObj = PhotonNetwork.Instantiate(resourceName, spawnLoc, spawnRot);
            return PlayerObj;
        }
        else {
            CBUG.Error("SpawnObject only available when Offline or InRoom, this was called at " + CurrentServerUserDepth.ToString() + ".");
            return null;
        }
    }

    public string GetLocalUsername() {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            return _fakeServer.Username;
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            string username = PhotonNetwork.LocalPlayer.UserId;
            username = username.Substring(0, username.IndexOf('_'));
            return username;
        }
        else {
            CBUG.Error("Username only available when Offline or InRoom, this was called at " + CurrentServerUserDepth.ToString() + ".");
            return null;
        }
    }

    public GameObject SpawnPlayer(Vector3 pos, Quaternion rot, string ResourceName="PhotonArenaPlayer") {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            _fakeServer.totalPlayers++;
            //spawn player? ???todo
            return null;
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            //ExitGames.Client.Photon.Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            //roomProps.Add(label, data);
           return PhotonNetwork.Instantiate(ResourceName, pos, rot);
        }
        else {
            CBUG.Error("Spawn Player only available when Offline or InRoom, this was called at " + CurrentServerUserDepth.ToString() + ".");
        }
        return null;
    }

    public bool IsLocalClient(PhotonView playerView) {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            return true;
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            return playerView.IsMine;
        } else {
            CBUG.Error("This can only be called when player is in a room or offline! You're currently in: " + CurrentServerUserDepth.ToString());
            return false;
        }
    }

    public int GetLocalPlayerID() {
        if (CurrentServerUserDepth == ServerDepthLevel.Offline) {
            return 1;
        }
        else if (CurrentServerUserDepth == ServerDepthLevel.InRoom) {
            return PhotonNetwork.LocalPlayer.ActorNumber; //??? unchanging? Unique? Todo;
        }
        else {
            CBUG.Error("This can only be called when player is in a room or offline! You're currently in: " + CurrentServerUserDepth.ToString());
            return PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

    public bool IsNewDataAvailable() {
        return newData;
    }

    #region PUN OVERRIDES
    public override void OnConnected() {
        base.OnConnected();
        CBUG.Do("Connected!");
        currentServerUserDepth = ServerDepthLevel.InServer;
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        CBUG.Do("Connected To Master! Joining Lobby ...");
        TypedLobby sqlLobby = new TypedLobby("Lobby", LobbyType.SqlLobby);
        bool success = PhotonNetwork.JoinLobby(sqlLobby);

        if (!success) {
            CBUG.Do("PunCockpit: Could not join Lobby ...");
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        newData = true;
    }

    public override void OnJoinedLobby() {
        CBUG.Log("Lobby Joined!");
        CBUG.Log("Joining Random Room ...");
        if(needsNewRoom) {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 9, CleanupCacheOnLeave = false }, null);
        }
        else {
            //string sqlLobbyFilter = "GameStartTime = -1"; //todo ??? implement sql lobbying
            PhotonNetwork.JoinRandomRoom();
        }
        currentServerUserDepth = ServerDepthLevel.InLobby;
    }

    public override void OnDisconnected(DisconnectCause cause) {
        CBUG.Log("OnDisconnected(" + cause + ")");
    }

    //public override void On

    public override void OnJoinedRoom() {
        CBUG.Log("Joined Room! Total Players: " + PhotonNetwork.CurrentRoom.PlayerCount);

        currentServerUserDepth = ServerDepthLevel.InRoom;

        SaveData("FirstTimeSetupDone", false);

        if (PhotonNetwork.IsMasterClient) {
            //spawn singletons 
            foreach (var singleton in singletonNames) {
                SpawnObject(singleton);
            }
        }
        SaveData("FirstTimeSetupDone", true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        CBUG.Log("Room Join failed. Creating a room ...");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 9, CleanupCacheOnLeave = false }, null);
    }
    #endregion
}

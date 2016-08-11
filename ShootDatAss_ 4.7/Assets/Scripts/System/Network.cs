using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;
using Sfs2X.Entities.Variables;
using Sfs2X.Entities.Data;

public class Network : MonoBehaviour
{

    public static Network instance;
    public SmartFox sfs;

    public string host = "127.0.0.1";
    public int port = 9933;
    public string zoneName;
    public string lobbyRoomName;
    public string userName;

    public bool isConnected;
    public bool isJoinedQuickStart;
    public bool isTryingQuickPlay;
    public List<string> publicMessage;

    public int quickPlayCount;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (instance == null) instance = this;
        sfs.ProcessEvents();
    }

    void Start()
    {
        sfs = new SmartFox();
        sfs.ThreadSafeMode = true;

        if (sfs == null)
        {
            Application.LoadLevel("MainGame");
            return;
        }

        host = GameData.instance.multiplayerServerHost;
        port = GameData.instance.multiplayerServerPort;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
        sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdd);
        sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnJoinRoomError);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionRespon);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.AddEventListener(SFSEvent.UDP_INIT, OnUDPInit);

        TimeManager.Instance.Init();
    }

    public void Connect()
    {
        Loading.instance.SetActive(true);
        StartCoroutine(GetUserData());
    }

    IEnumerator GetUserData()
    {

        GameData.instance.loadingMessege = "Loading player data";

        WWW www = new WWW(GameData.instance.databseConnectorUrl + "getUserData.php?name=" + GameData.instance.username);

        print(GameData.instance.databseConnectorUrl + "getUserData.php?name=" + GameData.instance.username);

        yield return www;

        if (www.text == "")
        {
            Connect();
        }
        else
        {
            PlayerData.instance.InitializeData(www.text);
            StartCoroutine(GetPlayerFriendList());
        }
    }

    IEnumerator GetPlayerFriendList()
    {
        GameData.instance.loadingMessege = "Loading friend list";

        WWW www = new WWW(GameData.instance.databseConnectorUrl + "getUserFriendList.php?name=" + GameData.instance.username);

        print(GameData.instance.databseConnectorUrl + "getUserFriendList.php?name=" + GameData.instance.username);

        yield return www;

        if (www.text == "")
        {

        }
        else
        {
            PlayerData.instance.InitializeFriendList(www.text);
            sfs.Connect(host, port);
            Loading.instance.SetActive(true);
            GameData.instance.loadingMessege = "Connecting to server";
        }
    }

    void OnConnection(BaseEvent e)
    {
        if ((bool)e.Params["success"])
        {
            GameData.instance.loadingMessege = "Logging in";
            sfs.Send(new LoginRequest(GameData.instance.username, "", zoneName));
        }
        else
        {

        }
    }

    void OnConnectionLost(BaseEvent e)
    {
        GameData.instance.gameState = GameData.GameState.LOGIN_TO_ZONE;
    }

    void OnLogin(BaseEvent e)
    {
        GameData.instance.loadingMessege = "Joining lobby";
        sfs.InitUDP(host, port);
        sfs.Send(new JoinRoomRequest(lobbyRoomName));
    }

    void OnUDPInit(BaseEvent e)
    {
        if ((bool)e.Params["success"])
        {
            print("UDP init success");
        }
        else
        {
            print("UDP init failed");
            Application.LoadLevel("MainGame");
        }
    }

    void OnLoginError(BaseEvent e)
    {
        print(e.Params["errorMessage"]);
        if (sfs.IsConnected) sfs.Disconnect();
    }

    void OnJoinRoom(BaseEvent e)
    {
        Loading.instance.SetActive(false);
        GameData.instance.loadingMessege = "Join success";

        if (((Room)e.Params["room"]).Name == "ChatRoom")
        {
            isConnected = true;
            SendPublicMessage("Welcome " + sfs.MySelf.Name + "!");
        }

        if (((Room)e.Params["room"]).Name.Contains("QuickPlay"))
        {
            isJoinedQuickStart = true;
        }
        print("Join room : " + e.Params["room"]);
    }

    void OnJoinRoomError(BaseEvent e)
    {
        GameData.instance.loadingMessege = "";
        print(e.Params["errorMessage"]);


        if (((string)e.Params["errorMessage"]).Contains("full"))
        {
            JoinAnotherRoom();
        }
        else if (((string)e.Params["errorMessage"]).Contains("exist") && isTryingQuickPlay)
        {
            CreateQuickPlayRoom();
        }
        else
        {
            //if(sfs.IsConnected)sfs.Disconnect();
        }
    }

    void OnApplicationQuit()
    {
        if (sfs.IsConnected) sfs.Disconnect();
    }

    void OnRoomAdd(BaseEvent e)
    {
        print("Room add");
    }

    void OnRoomCreateError(BaseEvent e)
    {
        print(e.Params["error"]);
    }

    void OnPublicMessage(BaseEvent e)
    {
        publicMessage.Add(e.Params["sender"] + " : " + e.Params["message"]);
    }

    public void QuickPlayStart()
    {
        isTryingQuickPlay = true;
        print("Try to join room: " + "QuickPlay" + quickPlayCount);
        sfs.Send(new JoinRoomRequest("QuickPlay" + quickPlayCount));
        GameData.instance.loadingMessege = "Finding available room";
    }

    public void CreateQuickPlayRoom()
    {
        print("Create room");

        RoomSettings room = new RoomSettings("QuickPlay" + quickPlayCount);
        room.MaxUsers = 4;
        room.MaxSpectators = 0;

        RoomPermissions permission = new RoomPermissions();
        permission.AllowNameChange = true;
        permission.AllowResizing = false;

        room.Permissions = permission;

        sfs.Send(new CreateRoomRequest(room, true));
    }

    public void JoinAnotherRoom()
    {
        quickPlayCount += 1;
        sfs.Send(new JoinRoomRequest("QuickPlay" + quickPlayCount));
    }

    public void OnExtensionRespon(BaseEvent evt)
    {
        try
        {
            string cmd = (string)evt.Params["cmd"];
            ISFSObject objIn = (SFSObject)evt.Params["params"];

            if (cmd == "LeaveRoom")
            {
                HandleLeaveRoom(objIn);
            }
            else if (cmd == "QuickPlayWait")
            {
                print("asd");
            }
            else if (cmd == "transform")
            {
                HandleTransform(objIn);
            }
            else if (cmd == "spawnPlayer")
            {
                //HandleSpawnPlayer(objIn);
            }
            else if (cmd == "time")
            {
                HandleServerTime(objIn);
            }
        }
        catch (Exception excep)
        {
            Debug.Log("Exception handling response: " + excep.Message + " >>> " + excep.StackTrace);
        }
    }

    // Synchronize the time from server
    private void HandleServerTime(ISFSObject dt)
    {
        long time = dt.GetLong("t");
        TimeManager.Instance.Synchronize(Convert.ToDouble(time));
    }

    public void SendQuitRoomRequest()
    {
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("name", sfs.MySelf.Name);
        objOut.PutInt("currentCostume", PlayerData.instance.currentCostume);
        sfs.Send(new ExtensionRequest("LeaveRoom", objOut));
    }

    public void Send_UserData()
    {
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", sfs.MySelf.Name);
        objOut.PutInt("playerID", sfs.MySelf.Id);
        objOut.PutUtfString("division", PlayerData.instance.division);
        objOut.PutInt("totalKill", PlayerData.instance.totalKill);
        objOut.PutInt("totalDeath", PlayerData.instance.totalDeath);
        objOut.PutInt("doubleKill", PlayerData.instance.doubleKill);
        objOut.PutInt("tripleKill", PlayerData.instance.tripleKill);
        objOut.PutInt("killingSpree", PlayerData.instance.killingSpree);
        objOut.PutInt("rampage", PlayerData.instance.rampage);
        objOut.PutInt("unstoppable", PlayerData.instance.unstoppable);
        objOut.PutInt("dominating", PlayerData.instance.dominating);
        objOut.PutInt("godlike", PlayerData.instance.godlike);
        objOut.PutInt("legendary", PlayerData.instance.legendary);
        objOut.PutInt("costume", PlayerData.instance.costume);

        sfs.Send(new ExtensionRequest("UserData", objOut));
    }

    public void Send_QuickPlay_RequestPlayerData()
    {

    }

    public void SendPublicMessage(string Message)
    {
        sfs.Send(new PublicMessageRequest(Message));
    }

    public void HandleLeaveRoom(ISFSObject objIn)
    {
        if (objIn.GetUtfString("name") == sfs.MySelf.Name)
        {
            Room roomToLeave = sfs.LastJoinedRoom;
            sfs.Send(new LeaveRoomRequest(roomToLeave));
            isJoinedQuickStart = false;
        }
        else
        {
            if (isJoinedQuickStart)
            {

            }
        }
    }

    // Updating transform of the remote player from server
    private void HandleTransform(ISFSObject dt)
    {
        int userId = dt.GetInt("id");
        NetworkTransform ntransform = NetworkTransform.FromSFSObject(dt);
        if (userId != sfs.MySelf.Id)
        {
            // Update transform of the remote user object

            // NetworkTransformReceiver recipient = PlayerManager.Instance.GetRecipient(userId);
            GameObject[] list =  GameObject.FindGameObjectsWithTag("Player");
            GameObject recObj = null;
            foreach (GameObject g in list)
                if (g.GetComponent<NetworkTransformReceiver>())
                    if (g.GetComponent<CharacterController>().id == userId)
                    {
                        recObj = g;
                    }
            NetworkTransformReceiver recipient = recObj.GetComponent<NetworkTransformReceiver>();
            if (recipient != null)
            {
                recipient.ReceiveTransform(ntransform);
            }
        }
    }

    void HandleSpawnPlayer(ISFSObject objIn)
    {
        ISFSObject playerData = objIn.GetSFSObject("player");
        string userName = playerData.GetUtfString("name");
        int userId = playerData.GetInt("id");
        int score = playerData.GetInt("score");
        NetworkTransform ntransform = NetworkTransform.FromSFSObject(playerData);

        User user = sfs.UserManager.GetUserById(userId);
        string name = userName;

        MatchManager.instance.SpawnPlayer(name,userId);
    }

    public void HendleUserData(ISFSObject objIn)
    {

    }

    public void HendleSendUserData(ISFSObject objIn)
    {
        //sfs.Send ();
    }

    /// <summary>
    /// Send local transform to the server
    /// </summary>
    /// <param name="ntransform">
    /// A <see cref="NetworkTransform"/>
    /// </param>
    public void SendTransform(NetworkTransform ntransform)
    {
        Room room = sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        ntransform.ToSFSObject(data);
        ExtensionRequest request = new ExtensionRequest("sendTransform", data, room, true); // True flag = UDP
        sfs.Send(request);
    }

    public void SendSpawnMe(NetworkTransform ntransform)
    {
        Room room = sfs.LastJoinedRoom;
        ISFSObject data = new SFSObject();
        ntransform.ToSFSObject(data);
        sfs.Send(new ExtensionRequest("spawnMe", data, room, true));
    }

    public void TimeSyncRequest()
    {
        Room room = sfs.LastJoinedRoom;
        ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room);
        sfs.Send(request);
    }
}

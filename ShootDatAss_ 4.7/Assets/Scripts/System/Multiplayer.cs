using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;

public class Multiplayer : MonoBehaviour {

	public string host = "127.0.0.1";
	public int port = 9933;
	public string zoneName;
	public string loobyRoomName;
	public string userName;

	public SmartFox sfs;

	public void Start(){
		sfs = new SmartFox ();
		sfs.ThreadSafeMode = true;

		sfs.AddEventListener (SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener (SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener (SFSEvent.LOGIN_ERROR, OnLoginError);
		sfs.AddEventListener (SFSEvent.ROOM_JOIN, OnJoinRoom);
		sfs.AddEventListener (SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
		sfs.AddEventListener (SFSEvent.ROOM_ADD, OnCreateRoom);

		sfs.Connect (host,port);
	}

	void Update(){
		sfs.ProcessEvents ();
	}

	void OnConnection(BaseEvent e){
		if((bool)e.Params["success"]){
			print("Connection Success");
			sfs.Send(new LoginRequest(userName, "", zoneName));
		}else{
			print("Connection Failed");
		}
	}

	void OnLogin(BaseEvent e){
		print ("Login Success");
		sfs.Send (new JoinRoomRequest(loobyRoomName));
	}

	void OnLoginError(BaseEvent e){
		print ("Login Error :" + e.Params["errorMessage"]);
	}

	void OnJoinRoom(BaseEvent e){
		print ("Join room success");
	}

	void OnJoinRoomError(BaseEvent e){
		foreach(Room room in sfs.RoomManager.GetRoomList()){
			print(room.Name);
		}
		print (e.Params["errorMessage"]);
	}

	void OnCreateRoom(BaseEvent e){
		print ("good");
	}



	void OnApplicationQuit(){
		if(sfs.IsConnected)sfs.Disconnect();
	}


}

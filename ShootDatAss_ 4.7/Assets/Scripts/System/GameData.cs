using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {

	public static GameData instance;
	
	public string username;
	public string password;

	public string _username
	{
		get
		{
			return PlayerPrefs.GetString ("username");
		}
		set
		{
			username = value;
			PlayerPrefs.SetString ("username",value);
			PlayerPrefs.Save();
		}
	}
	public string _password
	{
		get
		{
			return PlayerPrefs.GetString ("password");
		}
		set
		{
			password = value;
			PlayerPrefs.SetString ("password",value);
			PlayerPrefs.Save();
		}
	}

	public string multiplayerServerHost;
	public int multiplayerServerPort;

	public string databseConnectorUrl;
	public GameObject currentScene;

	public string loadingMessege;

	public enum GameState{
		REGISTER,
		LOGIN_TO_ZONE,
		MENU,
		QUICKPLAY_CONNCECT,
		QUICKPLAY_WAIT,
		QUICKPLAY_GAMEPLAY,
		PRACTICE_WAIT,
		PRACTICE_GAMEPLAY,
	}

	public GameState gameState = GameState.REGISTER;

	void Awake(){
		instance = this;
		PlayerPrefs.DeleteAll ();
		username = _username;
		password = _password;

		#if UNITY_STANDALONE
        //Screen.SetResolution(400, 240, false);
        Screen.SetResolution(800, 480, false);
		#endif
	}

	public void SaveUsernameAndPassword(string usr, string pwd){
		_username = usr;
		_password = pwd;
	}

	void Update(){
		if(instance == null) instance = this;

		Loading.instance.loadingText.text = GameData.instance.loadingMessege;

		switch(gameState){
		case GameState.REGISTER:
			Register();
			break;
		case GameState.LOGIN_TO_ZONE:
			LoginToZone();
			break;
		case GameState.MENU:
			Menu();
			break;
		case GameState.QUICKPLAY_WAIT:
			QuickPlayWait();
			break;
		case GameState.PRACTICE_WAIT:
			PracticeWait();
			break;
		case GameState.PRACTICE_GAMEPLAY:
			PracticePlay();
			break;
		}
	}

	void Register(){
        if (string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password))
        {
			if(!currentScene || currentScene.name != "Register"){
				try{
					Destroy(currentScene);
				}catch{}

				currentScene = Instantiate(ObjectLibrary.instance.login)as GameObject;
				currentScene.name = "Register";
			}
		}else{
			gameState = GameState.LOGIN_TO_ZONE;
		}
	}

	void LoginToZone(){
		if(!currentScene || currentScene.name != "LoginToZone"){
			try{ 
				Destroy(currentScene);
			}catch{}

			currentScene = Instantiate(ObjectLibrary.instance.loginToZone)as GameObject;
			currentScene.name = "LoginToZone";
			Network.instance.Connect();
		}

		if(currentScene){
			//Destroy(currentScene);
		}

		if(Network.instance.isConnected){
			gameState = GameState.MENU;
		}
	}

	void Menu(){
		if(!currentScene || currentScene.name != "Menu"){
			try{ 
				Destroy(currentScene);
			}catch{}

			currentScene = Instantiate(ObjectLibrary.instance.menu)as GameObject;
			currentScene.name = "Menu";
		}
	}

	private float sendDataRate;
	void QuickPlayWait(){
		if(Network.instance.isJoinedQuickStart){
			if(!currentScene || currentScene.name != "QuickPlayWait"){
				try{ 
					Destroy(currentScene);
				}catch{}

				currentScene = Instantiate(ObjectLibrary.instance.quickPlay)as GameObject;
				currentScene.name = "QuickPlayWait";
			}
		}else{
			if(currentScene)Destroy(currentScene);
			else Loading.instance.SetActive(true);
		}

		if(sendDataRate > 0.5f){
			//Network.instance.SendQuickPlayUserData();
		}else{
			sendDataRate += Time.deltaTime;
		}
	}

	void PracticeWait(){
		if(!currentScene || currentScene.name != "PracticeWait"){
			try{ 
				Destroy(currentScene);
			}catch{}

			Loading.instance.SetActive(false);
			currentScene = Instantiate(ObjectLibrary.instance.practice)as GameObject;
			currentScene.name = "PracticeWait";
		}
	}

	void PracticePlay(){
	}

}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	
	public bool isShowPlayMenu;

	public GameObject[] buttonPlay;
	public Vector2[] buttonPositionTarget;
	public GameObject practice;
	public GameObject chatRoom;
	public string Message;
	

	void Update () {
		if(isShowPlayMenu){
			buttonPlay[0].transform.position = new Vector3(Mathf.Lerp(buttonPlay[0].transform.position.x, buttonPositionTarget[0].x, 10 * Time.deltaTime),
			                                               buttonPlay[0].transform.position.y, buttonPlay[0].transform.position.z);
			buttonPlay[1].transform.position = new Vector3(Mathf.Lerp(buttonPlay[1].transform.position.x, buttonPositionTarget[1].x, 10 * Time.deltaTime),
			                                               buttonPlay[1].transform.position.y, buttonPlay[1].transform.position.z);
		}else {
			buttonPlay[0].transform.position = new Vector3(Mathf.Lerp(buttonPlay[0].transform.position.x, buttonPositionTarget[0].y, 10 * Time.deltaTime),
			                                               buttonPlay[0].transform.position.y, buttonPlay[0].transform.position.z);
			buttonPlay[1].transform.position = new Vector3(Mathf.Lerp(buttonPlay[1].transform.position.x, buttonPositionTarget[1].y, 10 * Time.deltaTime),
			                                               buttonPlay[1].transform.position.y, buttonPlay[1].transform.position.z);
		}
	}

	public void ButtonPlay(){
		isShowPlayMenu = !isShowPlayMenu;
	}

	public void ButtonQuickPlay(){
		GameData.instance.gameState = GameData.GameState.QUICKPLAY_WAIT;
		Network.instance.QuickPlayStart ();
	}

	public void ButtonPractice(){
		GameData.instance.gameState = GameData.GameState.PRACTICE_WAIT;
	}

	public void ButtonShowChatRoom(){

	}

	public void SendMessage(){
	}
	
}

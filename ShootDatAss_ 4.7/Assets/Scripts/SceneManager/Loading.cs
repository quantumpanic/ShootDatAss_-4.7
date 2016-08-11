using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Loading : MonoBehaviour {

	public static Loading instance;

	public bool isActive;

	public Text loadingText;

	void Awake(){
		instance = this;
		SetActive (false);
	}

	public void SetLoadingPage(string text){
		loadingText.text = text;
	}

	public void SetActive(bool condition){
		foreach (Transform t in transform) {
			t.gameObject.SetActive(condition);
		}
	}
}

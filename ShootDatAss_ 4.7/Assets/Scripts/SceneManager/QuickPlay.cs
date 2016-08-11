using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuickPlay : MonoBehaviour {

	public bool isJoinedRoom;
	public PlayerVariable[] playerVariable;
	public Text totalKill;
	public Text totalDeath;
	public Text doubleKill;
	public Text killingSpree;
	public Text rampage;
	public Text unstoppable;
	public Text dominating;
	public Text godlike;
	public Text legendary;

	void Start(){
		totalKill.text = PlayerData.instance.totalKill.ToString();
		totalDeath.text = PlayerData.instance.totalDeath.ToString() ;
		doubleKill.text = PlayerData.instance.doubleKill.ToString();
		killingSpree.text = PlayerData.instance.killingSpree.ToString();
		rampage.text = PlayerData.instance.rampage.ToString();
		unstoppable.text = PlayerData.instance.unstoppable.ToString();
		dominating.text = PlayerData.instance.dominating.ToString();
		godlike.text = PlayerData.instance.godlike.ToString();
		legendary.text = PlayerData.instance.legendary.ToString();
	}

	void Update(){
	
	}

}

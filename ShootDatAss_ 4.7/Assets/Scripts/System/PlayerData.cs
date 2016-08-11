using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour {

	public static PlayerData instance;

	//Player Data
	public int currentCostume;
	public List<int> ownedCostumes;
	public string username;
	public string division;
	public int totalKill;
	public int totalDeath;
	public int doubleKill;
	public int tripleKill;
	public int killingSpree;
	public int rampage;
	public int unstoppable;
	public int dominating;
	public int godlike;
	public int legendary;
	public int costume;
	public int costumeOwned;

	public List<string> friends;

	public void Awake(){
		instance = this;
	}


	public void InitializeData(string data){
		print (data);
		String[] datas = data.Split ('@');

		username = datas [0];
		division = datas [1];
		int.TryParse (datas[2],out totalKill);
		int.TryParse (datas[3],out totalDeath);
		int.TryParse (datas[4],out doubleKill);
		int.TryParse (datas[5],out tripleKill);
		int.TryParse (datas[6],out killingSpree);
		int.TryParse (datas[7],out rampage);
		int.TryParse (datas[8],out unstoppable);
		int.TryParse (datas[9],out dominating);
		int.TryParse (datas[10],out godlike);
		int.TryParse (datas[11],out legendary);
		int.TryParse (datas[12],out costume);
		int.TryParse (datas[13],out costumeOwned);
	}

	public void InitializeFriendList(string data){
		print (data);
		string[] datas = data.Split ('@');
		string[] friendlist = datas [1].Split ('|');
		foreach(string s in friendlist){
			friends.Add(s);
		}
	}

	public void SetOwnedCostume(){
		/*
		foreach(string costume in ownedCostumesSeparated){
			int costumeNumber = 0;
			int.TryParse(costume, out costumeNumber);
			if(!ownedCostumes.Contains(costumeNumber))ownedCostumes.Add(costumeNumber);
		}
		*/
	}

	public void BuyCostume(int costumeNumber){
		if (!ownedCostumes.Contains (costumeNumber)) {
			ownedCostumes.Add(costumeNumber);
			string costumes = "";
			for(int i=0; i<ownedCostumes.ToArray().Length; i++){
				costumes += ownedCostumes[i];
				if(i != ownedCostumes.ToArray().Length - 1) costumes += "|";
			}
			PlayerPrefs.SetString("ownedCostumes", costumes);
		}
	}

	public void ChangeCostume(int costumeNumber){
		currentCostume = costumeNumber;
	}
}

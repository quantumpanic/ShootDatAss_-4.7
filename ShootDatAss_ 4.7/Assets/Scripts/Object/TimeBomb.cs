using UnityEngine;
using System.Collections;

public class TimeBomb : ItemFunction {

	public CharacterController characterController;

	public GameObject timeBomb;
	private string playerName; 
	public int slotNumber;

	void Start(){
		characterController = gameObject.GetComponent<CharacterController>();
	}

	public void Execute(){
		//playerName = name;

		if(!timeBomb){
			timeBomb = new GameObject("TimeBomb");
			timeBomb.transform.parent = transform.parent;
			timeBomb.transform.position = transform.position;

			GameObject timeBombSprite = new GameObject("TimeBombSprite");
			timeBombSprite.transform.parent = timeBomb.transform;
			timeBombSprite.transform.position = new Vector3(timeBomb.transform.position.x, timeBomb.transform.position.z, 0);
			timeBombSprite.AddComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.GetItemSprite("timeBomb");
			timeBombSprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			timeBombSprite.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10);

			//if(characterController.itemReady.Contains("timeBomb"))characterController.itemReady.Remove("timeBomb");
			//characterController.itemHold[slotNumber] = "timeBombExplode" + slotNumber;
		}else{
			Destroy(timeBomb);
			if(characterController.itemReady.Contains("timeBombExplode" + slotNumber))characterController.itemReady.Remove("timeBombExplode" + slotNumber);
			Explode();
			Destroy(this);
		}
	}

	public void FixedUpdate(){
		CheckExist ();
		if(timeBomb)CheckReady ();
		else characterController.SetReady("timeBomb");
	}

	void Explode(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		bool there = false;

		GameObject explotion = Instantiate(ObjectLibrary.instance.explotion[0])as GameObject;
		explotion.name = "explotion";
		explotion.transform.parent = transform.parent;
		explotion.transform.position = new Vector3(timeBomb.transform.position.x, timeBomb.transform.position.z - 0.5f, 0);
		
		foreach(GameObject player in players){
			if(player.GetComponent<SphereCollider>()){
				if(Vector3.Distance(timeBomb.transform.position, player.transform.position) < 3){
					MatchManager.instance.SendHitPlayer(name, player.name, 10);
				}
			}
		}

		characterController.itemHold [slotNumber] = "";
	}

	public void CheckReady(){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		bool there = false;

		foreach(GameObject player in players){
			if(player.GetComponent<SphereCollider>() && player != gameObject){
				//print (Vector3.Distance(timeBomb.transform.position, player.transform.position));
				if(Vector3.Distance(timeBomb.transform.position, player.transform.position) < 3){
					there = true;
					break;
				}
			}
		}
		
		if(there){
			characterController.SetReady("timeBombExplode" + slotNumber);
		}else{
			if(characterController.itemReady.Contains("timeBombExplode" + slotNumber))characterController.itemReady.Remove("timeBombExplode" + slotNumber);			
		}
	}

	public void CheckExist(){
		bool there = false;

		if((!timeBomb && characterController.itemHold[slotNumber] == "timeBomb" ) || (timeBomb && characterController.itemHold[slotNumber] == "timeBombExplode" + slotNumber)) there = true;

		if(!there){
			if(timeBomb){
				Destroy(timeBomb);
				if(characterController.itemReady.Contains("timeBombExplode" + slotNumber))characterController.itemReady.Remove("timeBombExplode" + slotNumber);	
			}else{
				if(characterController.itemReady.Contains("timeBomb"))characterController.itemReady.Remove("timeBomb");	
			}
			//Destroy(this);
		}
	}
}

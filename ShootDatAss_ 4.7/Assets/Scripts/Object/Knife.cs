using UnityEngine;
using System.Collections;

public class Knife : ItemFunction {

	public CharacterController characterController;
	public GameObject target;

	private float knifeTime;
	private bool isKnife;
	private string playerName;
	private float knifeRate = 1;

	void Start(){
		characterController = gameObject.GetComponent<CharacterController>();
	}

	void Update () {
		if(!target){
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject player in players){
				if(player.GetComponent<SphereCollider>() && player != gameObject){
					if(Vector3.Distance(transform.position, player.transform.position) < 2){

						target = player;
					}
				}
			}
		}else{
			if(!isKnife){
				if(Vector3.Distance(transform.position, target.transform.position) > 2){
					if(characterController.itemReady.Contains("knife"))characterController.itemReady.Remove("knife");
					target = null;
				}else{
					characterController.SetReady("knife");
				}
			}
		}


		if(isKnife){
			if(knifeTime < 5){
				characterController.player.GetComponent<CharacterAnimationController>().gun = "Knife";

				if(Vector3.Distance(transform.position, target.transform.position) < 1){
					if(knifeRate > 0.5f){
						MatchManager.instance.SendHitPlayer(playerName, target.name, 5);
						knifeRate = 0;
					}else{
						knifeRate += Time.deltaTime;
					}
				}

				knifeTime += Time.deltaTime;

				characterController.isRocketLauncher = true;

			}else{
				characterController.player.GetComponent<CharacterAnimationController>().gun = "DefaultGun";
				characterController.isRocketLauncher = false;
				//Destroy(this);
			}
		}

		CheckExist ();
	}

	public void Execute(){
		//playerName = name;
		//if(target){
        characterController.isKnife = true;
			//if(characterController.itemReady.Contains("knife"))characterController.itemReady.Remove("knife");
			//characterController.itemHold[slotNumber] = "";
		//}
	}

	public void CheckExist(){
		bool there = false;
		foreach(string item in characterController.itemHold){
			if(item == "knife") there = true;
		}
        if (!there && !isKnife)
        {
            //Destroy(this);
        }
	}
}

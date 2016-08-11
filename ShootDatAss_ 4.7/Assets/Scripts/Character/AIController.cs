using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour {

	public CharacterController characterController;
	public  List<GameObject> target;
	public GameObject targetToCatch;
	public string direction;

	void Start(){
		target = new List<GameObject>();
		characterController = gameObject.GetComponent<CharacterController>();
	}

	void LateUpdate () {
		MatchManager.instance.SendMove (gameObject, 270, true, transform.position);

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach(GameObject player in players){
			if(player.GetComponent<SphereCollider>() && player != gameObject){
				if(Vector3.Distance(transform.position, player.transform.position) < 10){
					if(!target.Contains(player))target.Add(player);
				}
			}
		}

		foreach(GameObject go in target){
			if(!targetToCatch){
				if(Vector3.Distance(go.transform.position, transform.position) < 10){
					targetToCatch = go;
				}
			}else{
				if(Vector3.Distance(transform.position, go.transform.position) < Vector3.Distance(targetToCatch.transform.position, transform.position)){
					targetToCatch = go;
				}
			}
		}

		if(targetToCatch){
			if(Vector3.Distance(targetToCatch.transform.position, transform.position) > 10)targetToCatch = null;
		}

		if(targetToCatch){
			float angle = Mathf.Atan2(targetToCatch.transform.position.z - transform.position.z, targetToCatch.transform.position.x - transform.position.x)*180/Mathf.PI;
			if(angle < 0) angle = angle + 360;

			if(Vector3.Distance(targetToCatch.transform.position, transform.position) > 3){
				direction = GetDirection(angle);
			}else{
				direction = GetDirection(angle);
				if(!direction.Contains("Idle"))direction += "Idle";
				MatchManager.instance.SendShoot(gameObject, angle);
			}
			//MatchManager.instance.SendMove(gameObject, angle,false, transform.position);
		}
	}

	string GetDirection(float angle){
		string direction = "";
		
		if(angle < 22.5f || angle > 337.5){
			direction = "right";
		}else if(angle > 22.5f && angle < 67.5f){
			direction = "upRight";
		}else if(angle > 67.5 && angle < 112.5f){
			direction = "up";
		}else if(angle > 112.5f && angle < 157.5f){
			direction = "upLeft";
		}else if(angle > 157.5f && angle < 202.5f){
			direction = "left";
		}else if(angle > 202.5f && angle < 247.5f){
			direction = "downLeft";
		}else if(angle > 247.5f && angle < 292.5f){
			direction = "down";
		}else if(angle > 292.5f && angle < 337.5f){
			direction = "downRight";
		}
		return direction;
	}
}

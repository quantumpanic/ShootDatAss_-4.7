using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adrenaline : ItemFunction {

	public List<GameObject> target;
	public List<GameObject> targetArrow;
	public GameObject adrenaline;
	public GameObject adrenalinePutar;
	public float adrenalineTime = 15;

	private float adrenalineBlink;

	public void Start(){
		target = new List<GameObject> ();
		targetArrow = new List<GameObject> ();
		CreateAdrenaline ();
	}

	public void CreateAdrenaline(){
		adrenaline = Instantiate (ObjectLibrary.instance.adrenaline)as GameObject;
		adrenaline.transform.parent = transform;
		adrenaline.transform.localPosition = new Vector3 (0,0,0);

		adrenalinePutar = adrenaline.transform.FindChild ("adrenalinePutar").gameObject;
	}

	public void AdrenalineController(){
		adrenaline.transform.Rotate (0,0,-20 * Time.deltaTime);
		adrenalinePutar.transform.Rotate (0,0,40 * Time.deltaTime);
        adrenaline.transform.position = MatchManager.IsometricScaling(transform.position, 0.4f);
	}

	public void Update(){


		if(adrenalineTime < 5){
			adrenalineBlink += Time.deltaTime;

			if(adrenalineBlink > 0.2f){
				adrenaline.SetActive(!adrenaline.activeSelf);
				adrenalineBlink = 0;
			}
		}

		if(adrenalineTime > 0){
			FindPlayer ();
			AdrenalineController ();
		}else{
			DestroyAll();
		}

		adrenalineTime -= Time.deltaTime;
	}

	public void DestroyAll(){
		Destroy (adrenaline);
		Destroy (this);
	}

	public void FindPlayer(){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

 		foreach(GameObject player in players){
			if(player.GetComponent<SphereCollider>()){
				if(!target.Contains(player) && player != gameObject){
					target.Add(player);

					GameObject arrow = Instantiate(ObjectLibrary.instance.adrenalineArrow)as GameObject;
					arrow.name = player.name;
					arrow.transform.parent = adrenaline.transform;
					arrow.transform.localPosition = new Vector3(0,0,0);

					targetArrow.Add(arrow);
				}
			}
		}

		try{
			foreach(GameObject t in target){
				if(!t){
					GameObject remove = targetArrow.Find(x => x.name == t.name);
					targetArrow.Remove(remove);
					target.Remove(t);
				}
			}
		}catch{}

		foreach(GameObject arr in targetArrow){
			float angle = Mathf.Atan2(transform.position.z - target.Find(x => x.name == arr.name).transform.position.z, 
			                          transform.position.x - target.Find(x => x.name == arr.name).transform.position.x)*180/Mathf.PI;
			if(angle < 360) angle = angle + 360;

			arr.transform.eulerAngles = new Vector3(0,0,angle);
		}
	}

	public void Recharge(){
		adrenalineTime = 15;
	}


}

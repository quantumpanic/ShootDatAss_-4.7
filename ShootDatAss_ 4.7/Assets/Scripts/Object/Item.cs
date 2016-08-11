using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public GameObject objectBase;
	private Vector3 objectBasePosition;

	private float time;
	private float blinkTime;

	private float targetScale = 0.7f;
	private float scale = 1;

    public bool active;
    public int id;

	void Start () {
		InitializeItem ();
	}

	void FixedUpdate(){
		time += Time.deltaTime;
		if(time > 20) Destroy(gameObject); 
		if(time > 15){
			if(blinkTime > 0.25f){
				blinkTime = 0;
				objectBase.SetActive(!objectBase.activeSelf);
			}else{
				blinkTime += Time.deltaTime;
			}
		}
 
		scale = Mathf.MoveTowards (scale, targetScale, 0.5f * Time.deltaTime);

		if(scale == targetScale){
			if(targetScale == 0.7f){
				targetScale = 1;
			}else{
				targetScale = 0.7f;
			}
		}else{
		
		}

		objectBase.transform.localScale = new Vector3(scale, scale, scale);
	}

	void OnTriggerEnter(Collider other){
        if (!active) return;
		if(other.tag == "Player"){
            active = false;
			MatchManager.instance.SendGetItem
                (other.GetComponent<CharacterController>().playerName,
                ObjectLibrary.instance.itemFromCrate.Find(x => x.itemObject.name == name));
			Destroy(gameObject);
		}
	}

	void InitializeItem(){
        active = true;
		objectBase.transform.position  = MatchManager.IsometricScaling(objectBase.transform.position);
		try{
			objectBase.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10)-3;
		}catch{
			objectBase.transform.FindChild(name).GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10)-3;
		}
	}
}

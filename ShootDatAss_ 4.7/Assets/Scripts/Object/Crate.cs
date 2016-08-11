using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	private BoxCollider parentCollider;
	private BoxCollider2D myCollider;
    public bool isDead = false;

	void Start(){
		parentCollider = transform.parent.GetComponent<BoxCollider>();
		myCollider = gameObject.GetComponent<BoxCollider2D>();

		parentCollider.isTrigger = false;
		myCollider.isTrigger = true;

		gameObject.GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.crateSprite[Random.Range(0,4)];
	}


}

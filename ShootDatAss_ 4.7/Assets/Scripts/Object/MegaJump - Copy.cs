using UnityEngine;
using System.Collections;

public class MegaJumpCopy : MonoBehaviour {

	public CharacterController characterController;
	ChargeMechanic chrg;
	public bool isJump;
	public GameObject jumpArea;
	public Camera camera;
	public float targetAlpha;
	public int slotNumber;

	private Vector3 targetJump;
	private float speed;

	void Start(){
		characterController = gameObject.GetComponent<CharacterController>();
		jumpArea = Instantiate (ObjectLibrary.instance.jumpArea)as GameObject;
		jumpArea.transform.parent = transform;
		jumpArea.transform.position = new Vector3 (transform.position.x,transform.position.z,0);
		camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
	}

	void Update(){

		targetAlpha = Mathf.Lerp (targetAlpha, 1, 10 * Time.deltaTime);
		if(jumpArea)jumpArea.GetComponent<SpriteRenderer> ().color = new Color (1,1,1,targetAlpha);

		if(!isJump && targetAlpha > 0.97f){
			characterController.isMegaJump = true;

			targetJump = new Vector3(0,0,0);

			if(Input.GetMouseButtonUp(0)){
				targetJump = camera.ScreenToWorldPoint(Input.mousePosition);
			}

		

			if (targetJump != new Vector3 (0, 0, 0)) {
				if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetJump.x, targetJump.y)) < 5.5f){
					speed = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetJump.x, targetJump.y));
					float velocityY = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetJump.x, targetJump.y));

					//print(velocityY);

					if(velocityY < 3.5f) velocityY += 2;

					GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x,
					                                 velocityY
					                                 ,GetComponent<Rigidbody>().velocity.z);
					Destroy(jumpArea);
					isJump = true;
				}else{
					characterController.isMegaJump = false;
					Destroy(jumpArea);
					Destroy(this);
				}
			}
		}else{
			transform.position = Vector3.MoveTowards(transform.position, 
						                             new Vector3(targetJump.x, transform.position.y, targetJump.y), 
			                                         speed * Time.deltaTime);
			if(transform.position == new Vector3(targetJump.x, transform.position.y, targetJump.y)){
				characterController.isMegaJump = false;
				characterController.itemHold[slotNumber] = "";
				characterController.itemReady.Remove("megaJump");
				Destroy(jumpArea);
				Destroy(this);
			}
		}

	}
}

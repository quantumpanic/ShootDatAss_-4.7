using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemInfo
{
    public GameObject itemObject;
    public BeltItem beltItem;
}

public class ObjectLibrary : MonoBehaviour {

	public static ObjectLibrary instance;

	public GameObject[] maps;
	public GameObject characterBase;
	public GameObject playerBase;
    public List<ItemInfo> itemFromCrate = new List<ItemInfo>();
	public GameObject[] itemForController;
	public Sprite[] bullets;
	public Sprite[] crateSprite;
	public Sprite[] controllerSprite;
	public GameObject crate;
	public GameObject shadow;
	public GameObject spring;
	public GameObject touchController;
	public GameObject trail;
	public Sprite detonator;
	public GameObject adrenaline;
	public GameObject adrenalineArrow;
	public GameObject jumpArea;
	public GameObject[] explotion;
	public GameObject shootTarget;
	public GameObject menu;
	public GameObject quickPlay;
	public GameObject createGame;
	public GameObject practice;
	public GameObject login;
	public GameObject loginToZone;
	public Sprite[] status;

	public void Awake(){
		instance = this;
	}

	public void FixedUpdate(){
		if (instance == null) instance = this;
	}

	public GameObject GetBullet(int costumeID, int power, string direction, Vector2 velocity, string owner, float shootRange){
		GameObject bullet = new GameObject("Bullet");
		bullet.AddComponent<SpriteRenderer>();

		if (costumeID == -1) {
			bullet.GetComponent<SpriteRenderer>().sprite = bullets[4];
		}else if (costumeID < 21) {
			if(power < 6) bullet.GetComponent<SpriteRenderer>().sprite = bullets[1];
			else if(power < 11)bullet.GetComponent<SpriteRenderer>().sprite = bullets[2];
			else bullet.GetComponent<SpriteRenderer>().sprite = bullets[3];
		}

		bullet.AddComponent<Rigidbody2D> ().gravityScale = 0;
		bullet.AddComponent<BoxCollider2D> ().isTrigger = true;
		bullet.AddComponent<Bullet> ().InitializeBullet (owner, shootRange, power);
		bullet.GetComponent<Rigidbody2D> ().velocity = velocity * 25;
		bullet.transform.eulerAngles = GetRotation (direction);

		if (costumeID == -1)bullet.GetComponent<Bullet> ().type = -1; 

		GameObject brust = new GameObject ("Brust");
		brust.transform.parent = bullet.transform;
		brust.AddComponent<SpriteRenderer> ().sprite = bullets [0];
		brust.transform.eulerAngles = GetRotation (direction);


		return bullet;
	}

	public Sprite GetItemSprite(string itemName){
		Sprite sprite = new Sprite ();
		foreach(ItemInfo item in itemFromCrate){
			if(item.itemObject.name == itemName){
                foreach (Transform t in item.itemObject.transform)
                {
					sprite = t.GetComponent<SpriteRenderer>().sprite;
				}
				break;
			}
		}


		if (itemName.Contains ("timeBombExplode")) {
			sprite = detonator;
		}
		return sprite;
	}

	public GameObject GetPlayerBase(string playerName, int costumeID){
		GameObject playerBaseCostume = playerBase;
		playerBaseCostume.GetComponent<CharacterController> ().costumeID = costumeID;
		playerBaseCostume.GetComponent<CharacterController> ().playerName = playerName;

		return playerBaseCostume;
	}

	public GameObject GetCharacter(int costumeID){
		GameObject playerBaseCostume = characterBase;
		playerBaseCostume.GetComponent<CharacterAnimationController> ().SetCostume(costumeID);
		playerBaseCostume.GetComponent<CharacterAnimationController> ().FaceCharacter("downIdle");
		
		return playerBaseCostume;
	}

	Vector3 GetRotation(string direction){
		float rotation = 0;
		if(direction == "right"){
			rotation = 0;
		}else if(direction == "upRight"){
			rotation = 45;
		}else if(direction == "up"){
			rotation = 90;
		}else if(direction == "upLeft"){
			rotation = 135;
		}else if(direction == "left"){
			rotation = 180;
		}else if(direction == "downLeft"){
			rotation = 225;
		}else if(direction == "down"){
			rotation = 270;
		}else if(direction == "downRight"){
			rotation = 315;
		}

		return new Vector3(0,0,rotation);
	}

}

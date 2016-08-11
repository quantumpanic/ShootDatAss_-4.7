using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public string owner;
	private string direction;
	private int power;
	private Vector2 firstPosition;
	private float range;
	private SpriteRenderer spriteRenderer;
	public int type;

	void Start(){
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		firstPosition = transform.position;
	}

	void Update () {
		spriteRenderer.sortingOrder = (int)(-transform.position.y * 20);
		if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), firstPosition) > range)Destroy(gameObject);
	}

	public void InitializeBullet(string shooter, float shootRange, int powers){
		owner = shooter;
		range = shootRange;
		power = powers;

		if(range > 1) range = range - 1;
		else range = range - 0.2f;
	}

	public void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Crate"){
			MatchManager.instance.SendHitCrate(other.name);
			DestroyTrail();
			InstantiateExplosion();
			Destroy(gameObject);
		}

		if(other.tag == "Player"){
			return;
			MatchManager.instance.SendHitPlayer(owner, other.GetComponent<CharacterAnimationController>().playerName, power);
			DestroyTrail();
			InstantiateExplosion();
			Destroy(gameObject);
		}
	}

	public void DestroyTrail(){
		try{
			Destroy(transform.FindChild("Trail").GetComponent<Animator>());
			transform.FindChild("Trail").parent = transform.parent;
		}catch{}
	}

	public void InstantiateExplosion(){
		if(type < 0) {
            SoundManager.instance.PlaySfx("sfx_grndDie");
			GameObject explotion = Instantiate(ObjectLibrary.instance.explotion[1])as GameObject;
			explotion.name = "explotion";
			explotion.transform.parent = transform.parent;
			explotion.transform.position = transform.position;
		}
	}
}

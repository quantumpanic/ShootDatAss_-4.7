using UnityEngine;
using System.Collections;

public class Paintball001 : Projectile {
	
	public Paintball001 (GameObject shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public Paintball001 CreateComponent(GameObject go = null)
	{
		if (go == null) go = new GameObject("paintball");
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		Paintball001 init = go.AddComponent<Paintball001>();
		init.owner = owner;
		return init;
	}
	
	public WeaponStats curWeapon;
	CharacterController character;
	
	// Use this for initialization
	void Start()
	{
		ownerName = owner.name;
		character = owner.GetComponent<CharacterController>();
		curWeapon = new WeaponStats(Weapon.nailGun);
		base.Start();
		speed = curWeapon.speed;
		maxRange = curWeapon.range + character.shootRange;
		canCollide = true;
		radCollider = curWeapon.radius;
		gameObject.GetComponent<SphereCollider>().radius = radCollider;
		PlaySfx();
		
		GameObject canv = new GameObject("canvas");
		canvasObj = canv;
		
		SpriteRenderer canvasSprt = canvasObj.AddComponent<SpriteRenderer>();
		heightAdder = transform.position.y - 0.2f;
		canvasSprt.sortingOrder = (int)(-transform.position.z * 10) + 3;
		canvasObj.transform.Rotate(Vector3.forward, character.shotPivot.transform.eulerAngles.y);
		transform.Rotate(Vector3.down, character.shotPivot.transform.eulerAngles.y - 90);
		transform.position = character.shotPivot.transform.position;
		canvasSprt.sprite = ObjectLibrary.instance.bullets[1];
		
		DrawOnCanvas();
	}
	
	public virtual void PlaySfx()
	{
		SoundManager.instance.PlaySfx ("sfx_paintball", owner);
	}
	
	public override void HitSomething(Collider hit)
	{
		string tags = "Player Crate Border";
		if (hit.transform.name == ownerName) return;
		if (tags.Contains(hit.transform.tag)) // hit.transform.tag == "Player" || hit.transform.tag == "Crate")
		{
			RegisterHit(hit);
		}
	}
	
	public override void DoAfterHit(Collider hit)
	{
		float totalDmg = curWeapon.baseDmg + character.power;
		if (targetsHit <= 1)
		{
			if (hit.transform.tag == "Player")
			{
				MatchManager.instance.SendHitPlayer(ownerName, hit.transform.name, totalDmg);
				SoundManager.instance.PlaySfx("sfx_paintbuff",hit.gameObject);
				print("player " + ownerName + " damaged " + hit.transform.name + " for " + totalDmg + " damage.");
			}
			if (hit.transform.tag == "Crate")
			{
				MatchManager.instance.SendHitCrate(hit.transform.name);
				print("player hit crate #" + hit.transform.name);
			}
			collided = true;
			OnExpire();
		}
	}
}

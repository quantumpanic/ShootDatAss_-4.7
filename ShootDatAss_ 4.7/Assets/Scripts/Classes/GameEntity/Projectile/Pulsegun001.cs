using UnityEngine;
using System.Collections;

public class Pulsegun001 : Projectile {
	
	public Pulsegun001 (GameObject shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public Pulsegun001 CreateComponent(GameObject go = null)
	{
		if (go == null) go = new GameObject("pulse");
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		Pulsegun001 init = go.AddComponent<Pulsegun001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats (Weapon.pulseLaser);
		return init;
	}
	
	public WeaponStats curWeapon;
	CharacterController character;
	
	// Use this for initialization
	void Start()
	{
		ownerName = owner.name;
		character = owner.GetComponent<CharacterController>();
		base.Start();
		speed = curWeapon.speed;
		maxRange = curWeapon.range + character.shootRange;
		canCollide = true;
		radCollider = curWeapon.radius;
		gameObject.GetComponent<SphereCollider>().radius = radCollider;
		PlaySfx ();
		
		GameObject canv = new GameObject("canvas");
		canvasObj = canv;
		
		SpriteRenderer canvasSprt = canvasObj.AddComponent<SpriteRenderer>();
		heightAdder = transform.position.y - 0.2f;
		canvasSprt.sortingOrder = (int)(-transform.position.z * 10) + 3;
		canvasObj.transform.Rotate(Vector3.forward, character.shotPivot.transform.eulerAngles.y);
		transform.Rotate(Vector3.down, character.shotPivot.transform.eulerAngles.y - 90);
		transform.position = character.shotPivot.transform.position;
		canvasSprt.sprite = ObjectLibrary.instance.bullets[6];
		
		DrawOnCanvas();
		
		GameObject brust = new GameObject("Brust");
		brust.AddComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[5];
		
		brust.transform.parent = character.player.transform;
		brust.transform.Rotate(Vector3.forward, character.shotPivot.transform.eulerAngles.y);
		brust.transform.position = new Vector3(transform.position.x, transform.position.z*canvas2map.y, 0f);
		Brust burstScrp = brust.AddComponent<Brust>();
		string direction = character.player.GetComponent<CharacterAnimationController>().faceDirection;
		brust.transform.localPosition = burstScrp.GetShootPoint(direction);
		brust.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) + 3;
	}
	
	public virtual void PlaySfx()
	{
		SoundManager.instance.PlaySfx("sfx_pulse",owner);
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
				ShockDebuff hasBuff = hit.gameObject.GetComponent<ShockDebuff>();
				if (!hasBuff) hasBuff = new ShockDebuff(owner).CreateComponent(hit.gameObject);
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

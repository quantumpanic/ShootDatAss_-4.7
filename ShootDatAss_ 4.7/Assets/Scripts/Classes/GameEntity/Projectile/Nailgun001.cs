using UnityEngine;
using System.Collections;

public class Nailgun001 : Projectile {

	public Nailgun001 (GameObject shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}

    public Nailgun001 CreateComponent(GameObject go = null)
    {
        if (go == null) go = new GameObject("nail");
        if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
        if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
        Nailgun001 init = go.AddComponent<Nailgun001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats(Weapon.nailGun);
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
        SoundManager.instance.PlaySfx("sfx_nail");

        transform.Rotate(Vector3.down, character.shotPivot.transform.eulerAngles.y - 90);
        transform.position = character.shotPivot.transform.position;

        GameObject canv = new GameObject("canvas");
        canvasObj = canv;
        SpriteRenderer canvasSprt = canvasObj.AddComponent<SpriteRenderer>();
        
        heightAdder = transform.position.y - 0.2f;
        canvasSprt.sortingOrder = (int)(-transform.position.z * 10) + 3;
        canvasObj.transform.Rotate(Vector3.forward, character.shotPivot.transform.eulerAngles.y);
        canvasSprt.sprite = ObjectLibrary.instance.bullets[8];

        DrawOnCanvas();
    }

    public override void HitSomething(Collider hit)
    {
        string tags = "Player Crate Border";
        if (hit.transform.name == ownerName) return;
        if (tags.Contains(hit.transform.tag)) // hit.transform.tag == "Player" || hit.transform.tag == "Crate")
        {
            RegisterHit(hit);
        }
        else if (hit.transform.tag == "Projectile")
        {
            if (hit.gameObject.GetComponent<Projectile>().owner == owner) return;
            if (hit.gameObject.GetComponent<Nailgun001>() == null)
                RegisterHit(hit, false);
        }
    }

	void OnTriggerEnter(Collider col)
	{
		HitSomething (col);
	}

    public override void DoAfterHit(Collider hit)
    {
        float totalDmg = curWeapon.baseDmg + character.power;
        if (hit.transform.tag == "Projectile")
        {
            Projectile missile = hit.GetComponent<Projectile>();
            // try add buff to owner of projectile
            NailDebuff hasBuff = missile.owner.GetComponent<NailDebuff>();
            if (!hasBuff) hasBuff = new NailDebuff(owner).CreateComponent(missile.owner);
            if (hasBuff.hasPierce)
			{
				missile.OnExpire();
                SoundManager.instance.PlaySfx("sfx_pierce");

                GameObject brust = new GameObject("spark");
                brust.AddComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[7];

                Brust burstScrp = brust.AddComponent<Brust>();
                brust.transform.position = canvasObj.transform.position;
                brust.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) + 3;
			}
            return;
        }
        if (targetsHit <= 1)
        {
            if (hit.transform.tag == "Player")
            {
                MatchManager.instance.SendHitPlayer(ownerName, hit.transform.name, totalDmg);
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

    void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, radCollider);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(canvasObj.transform.position, radCollider);
    }
}

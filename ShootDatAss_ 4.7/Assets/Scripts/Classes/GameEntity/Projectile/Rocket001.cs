using UnityEngine;
using System.Collections;

public class Rocket001 : Projectile {
	
	public Rocket001 (GameObject shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public static Rocket001 CreateComponent(GameObject go)
	{
		//Bullet001 init = new Bullet001(go);
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		Rocket001 init = go.AddComponent<Rocket001>();
		return init;
	}

    public AltWeaponStats curWeapon;
    CharacterController character;
	
	// Use this for initialization
	void Start()
	{
        ownerName = owner.name;
        character = owner.GetComponent<CharacterController>();
		curWeapon = new AltWeaponStats(AltWeapon.rocketLauncher);
		base.Start();
		speed = curWeapon.speed;
        maxRange = curWeapon.range + character.shootRange;
        canCollide = true;
        radCollider = curWeapon.radius;
        gameObject.GetComponent<SphereCollider>().radius = radCollider;
        SoundManager.instance.PlaySfx("sfx_rocket",gameObject);

        GameObject canv = new GameObject("rktCanvas");
        canvasObj = canv;

        SpriteRenderer canvasSprt = canvasObj.AddComponent<SpriteRenderer>();
        heightAdder = transform.position.y - 0.2f;
        canvasSprt.sortingOrder = (int)(-transform.position.z * 10) + 3;
        canvasObj.transform.Rotate(Vector3.forward, character.shotPivot.transform.eulerAngles.y);
        transform.Rotate(Vector3.down, character.shotPivot.transform.eulerAngles.y - 90);
        transform.position = character.shotPivot.transform.position;

        GameObject trail = Instantiate(ObjectLibrary.instance.trail) as GameObject;
        trail.name = "Trail";
        trail.transform.parent = canvasObj.transform;
        trail.transform.localPosition = new Vector3(0, 0, 0);

        DrawOnCanvas();

        canvasSprt.sprite = ObjectLibrary.instance.bullets[4];

        GameObject brust = new GameObject("Brust");
        brust.AddComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[0];

        brust.transform.parent = character.player.transform;
        brust.transform.Rotate(Vector3.forward, character.shotPivot.transform.eulerAngles.y);
        brust.transform.position = new Vector3(transform.position.x, transform.position.z * canvas2map.y, 0f);
        Brust burstScrp = brust.AddComponent<Brust>();
        string direction = character.player.GetComponent<CharacterAnimationController>().faceDirection;
        brust.transform.localPosition = burstScrp.GetShootPoint(direction);
        brust.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) + 3;
	}

    public override void HitSomething(Collider hit)
    {
        string tags = "Player Crate Border";
        if (hit.transform.name == ownerName) return;
        //if (tags.Contains(hit.transform.tag)) // hit.transform.tag == "Player" || hit.transform.tag == "Crate")
        if (tags.Contains(hit.transform.tag))
        {
            RegisterHit(hit);
        }
    }

    public override void DoAfterHit(Collider hit)
    {
        float totalDmg = curWeapon.baseDmg + character.power;
        if (targetsHit > 0)
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

    public override void OnExpire()
    {
        InstantiateExplosion();
        base.MarkToDestroy(base._states);
    }

    public void InstantiateExplosion()
    {
        GameObject explotion = Instantiate(ObjectLibrary.instance.explotion[1]) as GameObject;
        explotion.name = "explotion";
        //explotion.transform.parent = canvasObj.transform;
		explotion.transform.position = canvasObj.transform.position;
		SoundManager.instance.PlaySfx("sfx_grndDie",transform.position);
    }

    void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, radCollider);
    }
}

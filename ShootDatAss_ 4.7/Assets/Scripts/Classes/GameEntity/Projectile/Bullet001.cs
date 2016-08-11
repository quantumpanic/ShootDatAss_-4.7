using UnityEngine;
using System.Collections;

public class Shotgun001 : Bullet001
{
    public Shotgun001(GameObject shooter)
        : base(shooter)
    {
        owner = shooter;
        ownerName = shooter.name;
    }

    public Shotgun001[] CreateComponents(int numShells, float spreadAngle)
    {
		SoundManager.instance.PlaySfx("sfx_shotgun",owner);
        Shotgun001[] allshells = new Shotgun001[numShells];
        for (int s = 0; s <= numShells-1; s++)
        {
            GameObject go = new GameObject("sgShell");
            if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
            if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
            allshells[s] = go.AddComponent<Shotgun001>();
            allshells[s].spreadAngle = spreadAngle;
			allshells[s].owner = owner;
			allshells[s].curWeapon = new WeaponStats(Weapon.shotgun);
        }
        return allshells;
    }

    public float spreadAngle;

    public override void PlaySfx()
    {
        // silence original sfx
    }

    void Start()
    {
        base.Start();
        float angleVariance = Random.Range(-spreadAngle,spreadAngle);
        transform.Rotate(Vector3.down, angleVariance);
    }
}

public class AssaultRifle001 : Bullet001
{
	public AssaultRifle001(GameObject shooter)
		: base(shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public AssaultRifle001 CreateComponent(GameObject go = null)
	{
		if (go == null) go = new GameObject("assaultround");
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		AssaultRifle001 init = go.AddComponent<AssaultRifle001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats(Weapon.AK47);
		return init;
	}

	public override void PlaySfx()
	{
		SoundManager.instance.PlaySfx ("sfx_ak47",owner);
	}
}

public class SubMachineGun001 : Bullet001
{
	public SubMachineGun001(GameObject shooter)
		: base(shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public SubMachineGun001 CreateComponent(GameObject go = null)
	{
		if (go == null) go = new GameObject("SMGround");
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		SubMachineGun001 init = go.AddComponent<SubMachineGun001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats(Weapon.SMG);
		return init;
	}
	
	public override void PlaySfx()
	{
		SoundManager.instance.PlaySfx ("sfx_smg",owner);
	}
}

public class Musket001 : Bullet001
{
	public Musket001(GameObject shooter)
		: base(shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public Musket001 CreateComponent(GameObject go = null)
	{
		if (go == null) go = new GameObject("musketbead");
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		Musket001 init = go.AddComponent<Musket001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats(Weapon.musket);
		return init;
	}
	
	public override void PlaySfx()
	{
		SoundManager.instance.PlaySfx ("sfx_musket",owner);
	}
}

public class PirateGun001 : Bullet001
{
	public PirateGun001(GameObject shooter)
		: base(shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}
	
	public PirateGun001 CreateComponent(GameObject go = null)
	{
		if (go == null) go = new GameObject("parleybead");
		if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
		if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
		PirateGun001 init = go.AddComponent<PirateGun001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats(Weapon.parley);
		return init;
	}
	
	public override void PlaySfx()
	{
		SoundManager.instance.PlaySfx ("sfx_parley",owner);
	}
}

public class Bullet001 : Projectile
{
	public WeaponStats curWeapon;
	public CharacterController character;

	public Bullet001 (GameObject shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}

    public Bullet001 CreateComponent(GameObject go = null)
    {
        if (go == null) go = new GameObject("bullet");
        if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
        if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
        Bullet001 init = go.AddComponent<Bullet001>();
		init.owner = owner;
		init.curWeapon = new WeaponStats(Weapon.gun);
        return init;
    }

	// Use this for initialization
    public void Start()
    {
        ownerName = owner.name;
        character = owner.GetComponent<CharacterController>();
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

        DrawOnCanvas();

        if (character.costumeID == -1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[4];
        }
        else if (character.costumeID < 21)
        {
            float totalDmg = curWeapon.baseDmg + character.power;
            if (totalDmg < 6) canvasObj.GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[1];
            else if (totalDmg < 11) canvasObj.GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[2];
            else canvasObj.GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[3];
        }

        GameObject brust = new GameObject("Brust");
        brust.AddComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.bullets[0];

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
		SoundManager.instance.PlaySfx("sfx_gun",owner);
    }

	public override void HitSomething (Collider hit)
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

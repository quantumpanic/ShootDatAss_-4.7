using UnityEngine;
using System.Collections;

public class SmokeBomb001 : Grenade001
{
    public SmokeBomb001(GameObject shooter) : base(shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}

    public SmokeBomb001 CreateComponent(GameObject go = null)
    {
        if (go == null) go = new GameObject("smokebomb");
        if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
        if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
        SmokeBomb001 init = go.AddComponent<SmokeBomb001>();
		init.owner = owner;
		init.curWeapon = new AltWeaponStats(AltWeapon.smokeBomb);
        return init;
    }
}

public class Grenade001 : Projectile
{
    public Grenade001(GameObject shooter)
	{
		owner = shooter;
		ownerName = shooter.name;
	}

    public Grenade001 CreateComponent(GameObject go = null)
    {
        if (go == null) go = new GameObject("grenade");
        if (!go.GetComponent<Rigidbody>()) go.AddComponent<Rigidbody>().useGravity = false;
        if (!go.GetComponent<Collider>()) go.AddComponent<SphereCollider>().isTrigger = true;
        Grenade001 init = go.AddComponent<Grenade001>();
		init.owner = owner;
		init.curWeapon = new AltWeaponStats(AltWeapon.grenade);
        return init;
    }

    public AltWeaponStats curWeapon;
    CharacterController character;
    public bool isSeeking;
    public GameObject target;
    Vector3 trgPos;
    public float throwRange;

    void Start()
    {
        ownerName = owner.name;
        character = owner.GetComponent<CharacterController>();
        base.Start();
        speed = curWeapon.speed;
        maxRange = throwRange;
        canCollide = true;
        radCollider = curWeapon.radius;
        gameObject.GetComponent<SphereCollider>().radius = radCollider;
        SoundManager.instance.PlaySfx("sfx_grndThrw");

        transform.Rotate(Vector3.down, character.shotPivot.transform.eulerAngles.y - 90);
        transform.position = character.shotPivot.transform.position;

        GameObject canv = new GameObject("canvas");
        canvasObj = canv;
        canvasObj.AddComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.GetItemSprite("granade");
        canvasObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        DrawOnCanvas();
    }

    public override void HitSomething(Collider hit)
    {
        string tags = "Player Border";
        if (hit.transform.name == ownerName || isSeeking) return;
        if (tags.Contains(hit.transform.tag)) // hit.transform.tag == "Player" || hit.transform.tag == "Crate")
        {
            RegisterHit(hit);
        }
    }

    public override void DoAfterHit(Collider hit)
    {
        if (targetsHit <= 1)
        {
            collided = true;
            OnExpire();
        }
    }

    public override void OnExpire()
    {
        InstantiateExplosion();
        base.MarkToDestroy(base._states);
    }

    void Update()
    {
        base.Update();
        canvasObj.transform.Rotate(new Vector3(0, 0, 20));
        Transform trg = FindTarget();
        if (trg != null) trgPos = trg.position;
        if (isSeeking)
        {
            maxRange = Vector3.Distance(startPos, trgPos);
            transform.LookAt(trg);
        }
    }

    public Transform FindTarget()
    {
        if (target != null)
        {
            isSeeking = true;
            return target.transform;
        }
        else
        {
            isSeeking = false;
            return null;
        }
    }

    public virtual void InstantiateExplosion()
    {
        SoundManager.instance.PlaySfx("sfx_grndDie");
        GameObject explosion = Instantiate(ObjectLibrary.instance.explotion[1]) as GameObject;
        explosion.name = "explosion";
        explosion.transform.position = canvasObj.transform.position;
        explosion.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) + 15;
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

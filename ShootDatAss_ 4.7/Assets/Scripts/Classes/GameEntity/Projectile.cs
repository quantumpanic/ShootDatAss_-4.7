using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticProjectile : Projectile
{

}

public class Projectile : GameEntity {

	public GameObject owner;
	public string ownerName;

	public Vector3 startPos;
	public float maxRange;
	float travelled;
    public float maxTravel;
	float lifeTime;
    public float lifeSpan;
	public float speed;
	public float rayLength;
	public float rayRadius;
	public float radCollider;
	public bool collided;
	public bool rayHit;
    public float lastDelay;

    Collider lastHit;
    string lastHitName;
	public int targetsHit;
    public List<Hit> hitList;

    public bool doRayCast = false;
    public bool canCollide = false;

    public struct States
    {
        public Vector3 lastPos;
        public float lastLifetime;
        public float distTravelled;
        public bool wasCollided;
        public bool wasRayHit;
        public Vector3 velocity;
        public string lastHit;
        public int totalHits;
    }

	public States _states {
		get{
			States temp = new States();
			temp.lastPos = transform.position;
			temp.lastLifetime = lifeTime;
            temp.distTravelled = travelled;
			temp.wasCollided = collided;
			temp.wasRayHit = rayHit;
			temp.velocity = GetComponent<Rigidbody>().velocity;
            temp.lastHit = lastHitName;
            temp.totalHits = targetsHit;
			return temp;
				}
	}

    public class Hit
    {
        public Hit(Collider col)
        {
            collider = col;
            hitDelay = 0;
        }
        public Collider collider;
        public float hitDelay;
    }

    States currentState;

	// Use this for initialization
    public void Start()
    {
        base.Start();
        gameObject.tag = "Projectile";
        hitList = new List<Hit>();
        transform.position = owner.transform.position;
		startPos = transform.position;
        maxRange = 10000;
        maxTravel = 10000;
        lifeSpan = 10000;
	}
	
	// Update is called once per frame
    public void FixedUpdate()
    {
        CheckLastDelay();
		CheckLifetime (); // check time first. if long durations pass before next frame, will clean projectile.
		if (doRayCast) DoRayCast(); //send ray before moving forward. better not to use unless have super-fast bullets.
		GetComponent<Rigidbody>().velocity = transform.forward * speed; // move the bullet
		CheckRange (); // now check if bullet is outside allowed range
		CheckTravel (); // finally check projectile total distance travelled

        currentState = _states;
	}

    public virtual void HitSomething(Collider hit) // hit a collider. what to do with it (filter, ignore, etc)
	{
        //override in child as you want
		if (hit.transform.tag == "Player")
		{
            print("hit a player. register hit");
            RegisterHit(hit);
		}
		if (hit.transform.tag == "Crate")
        {
            print("hit a crate and ignore");
		}
    }

    public void RegisterHit(Collider hit, bool register = true)
    {
        //here to check if bullet die immediately or other condition like pierce X number of enemies before die

        bool contains = false;
        for (int h = 0; h < hitList.Count; h++)
        {
            if (hitList[h].collider == hit)
            {
                contains = true;
                break;
            }
        }

        if (!contains)
        {
            lastHit = hit; // assign the collider to the public variable 'lastHit' for reference purposes
            lastHitName = hit.transform.name;

            // add the hit to history
            if (register)
            {
                targetsHit++;
                Hit newHit = new Hit(hit);
                hitList.Add(newHit);
            }

            DoAfterHit(lastHit);
        }
    }

    public virtual void DoAfterHit(Collider hit)
    {
        //override changes appropriately
        MarkToDestroy(_states);
    }

    public virtual void CheckLastDelay() // make collider unable to be hit again for duration
    {
        // change interval by setting 'lastDelay'
        for (int h = hitList.Count - 1; h >= 0; h-- )
        {
            hitList[h].hitDelay += Time.deltaTime;
            if (hitList[h].hitDelay >= lastDelay) hitList.RemoveAt(h);
        }
    }

	public virtual void OnExpire()
    {
        //override changes appropriately
        MarkToDestroy(_states);
	}

	void DoRayCast()
	{
        Ray newRay = new Ray(transform.position, transform.position + transform.forward);
		RaycastHit newHit;
		bool rayHit = Physics.SphereCast (newRay,rayRadius,out newHit,rayLength) ;
		if (rayHit) HitSomething(newHit.collider);
        rayHit = true;
	}

	void OnTriggerStay (Collider hit)
	{
        HitSomething(hit);
	}

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*rayLength);
    }

    void CheckLifetime()
	{
		lifeTime += Time.deltaTime;
		if (lifeTime > lifeSpan) OnExpire();
	}

    void CheckRange()
	{
		float range = Vector3.Distance (transform.position, startPos);
		if (range >= maxRange) OnExpire();
	}

    void CheckTravel()
	{
		travelled += Vector3.Distance(transform.position, transform.forward * speed);
		if (travelled >= maxTravel) OnExpire();
	}

	public virtual void MarkToDestroy(States states)
	{
        //add stuff
        print(ownerName +
            "'s projectile, '" +
            transform.name +
            "' died at pos " +
            states.lastPos +
            " with velocity " +
            states.velocity +
            " after travelling " +
            states.distTravelled +
            " distance over " +
            states.lastLifetime +
            " secs and colliding with '" +
            states.lastHit +
            "'. Total collisions:" +
            states.totalHits +
            ". Collider hit? " +
            states.wasCollided +
            ". Raycast hit? " +
            states.wasRayHit +
            ".");
        Destroy(gameObject);
	}
}

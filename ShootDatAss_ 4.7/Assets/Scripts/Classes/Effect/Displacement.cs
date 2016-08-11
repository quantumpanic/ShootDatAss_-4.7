using UnityEngine;
using System.Collections;

public class WallSlip : Displacement
{
    public Collider onWall;
    public WallSlip(Collider wall)
    {
        onWall = wall;
    }

    public void Awake()
    {
    }

    public void FixedUpdate()
    {
        Rigidbody body = unitToMove.gameObject.GetComponent<Rigidbody>();
        if (unitToMove.aboveTheWall)
        {
        }
        // do displacement
        if (unitToMove.grounded)
        {
            unitToMove.aboveTheWall = false;
            Destroy(this);
        }
    }
}

public class Dash : Displacement
{
    public float dashDist;
    public GameObject dashTrgt;
    public Dash(float distance, GameObject trgt = null)
    {
        dashDist = distance;
        dashTrgt = trgt;
    }

    public void FixedUpdate()
    {

    }

    public class DashDistance : Projectile
    {
    }
}

public class Jump : Displacement
{

}

public class Bounce : Displacement
{
    public Collider surface;
    public Bounce(Collider srfc)
    {
        surface = srfc;
    }
}

public class Displacement : Effect {

    public Unit unitToMove;

	// Use this for initialization
	void Start () {
        unitToMove = gameObject.GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

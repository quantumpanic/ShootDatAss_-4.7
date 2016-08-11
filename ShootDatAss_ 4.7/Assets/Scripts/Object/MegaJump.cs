using UnityEngine;
using System.Collections;

public class MegaJump : ItemFunction {

    public CharacterController controller;
    public GameObject target;
    public GameObject jumpArea;
    public ChargeMechanic chrg;
	GameObject node;

	public bool isJump;
	public Camera camera;
	public float targetAlpha;
	public int slotNumber;

	private GameObject targetJump;
	private float speed;

    GameObject crosshairs;

	void Start(){
		// no charger found. execute auto-aim
		controller = gameObject.GetComponent<CharacterController>();
		jumpArea = Instantiate(ObjectLibrary.instance.jumpArea) as GameObject;
		jumpArea.GetComponent<SpriteRenderer>().color = Color.green;
		jumpArea.transform.localScale = new Vector3(0, 0, 0);
		jumpArea.transform.parent = transform;
		jumpArea.transform.position = new Vector3 (transform.position.x,transform.position.z,0);
		camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
	}

    public void CancelJump()
    {
        chrg.isCharging = false;
        chrg.gaugeFill = 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if(targetJump) Gizmos.DrawSphere(targetJump.transform.position, 1);
    }

    public void ChargedJump()
    {
        string face = controller.gameObject.GetComponentInChildren<CharacterAnimationController>().faceDirection;
        float kokang = 3f + (chrg.gaugeFill / chrg.gaugeMax * 3f);

        Vector3 originOfAngle; //this will be where you are casing your angle from.
        Vector3 angle; //this will be your target angle.
        float distanceToTestFor = kokang; //this will be your testing radius
        //set the above values however you wish

        if (face == "right") angle = Vector3.right;
        else if (face == "upRight") angle = Vector3.forward + Vector3.right;
        else if (face == "up") angle = Vector3.forward;
        else if (face == "upLeft") angle = Vector3.forward + Vector3.left;
        else if (face == "left") angle = Vector3.left;
        else if (face == "leftDown") angle = Vector3.back + Vector3.left;
        else if (face == "down") angle = Vector3.back;
        else if (face == "downLeft") angle = Vector3.back + Vector3.left;
        else if (face == "downRight") angle = Vector3.back + Vector3.right;
        else angle = Vector3.back;

        Ray rayToTest = new Ray(Vector3.zero, angle);
        Vector3 targetPoint = rayToTest.GetPoint(kokang);
        //The above line should return your desired Vector3

        // make the target node
        node = new GameObject();
        node.transform.position = targetPoint + controller.gameObject.transform.position;

		targetJump = node;

		SpeedCheck ();

        //gameObject.GetComponent<CharacterController>().itemHold[slotNumber] = "";
        //gameObject.GetComponent<CharacterController>().itemReady.Remove("megaJump");
		CancelJump();
		controller.isMegaJump = true;
		isJump = true;
		//controller.itemHold[slotNumber] = "";
		//controller.itemReady.Remove("megaJump");
	}
	
	public void Jump(){
        SoundManager.instance.PlaySfx("sfx_mJump1");
		if (chrg.isCharging)
		{
			ChargedJump();
			return;
		}
		if(target)
		{
			targetJump = target;
			SpeedCheck();
			isJump = true;
			//controller.itemHold[slotNumber] = "";
			//controller.itemReady.Remove("megaJump");
		}
	}

	void SpeedCheck ()
	{
        //targetJump = new Vector3(0, 0, 0);
        float velocityY = 6f;// Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetJump.transform.position.x, targetJump.transform.position.y));
        float factor = (velocityY / 10) * 2;
        speed = velocityY-2;// Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetJump.transform.position.x, targetJump.transform.position.z)) / factor;
		
		//print(velocityY);
		
		//if (velocityY < 3.5f) velocityY += 2;
		//else if (velocityY > 5) velocityY -= 2;
		
		GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, velocityY, GetComponent<Rigidbody>().velocity.z);
	}

    void Update()
	{
		if (chrg.isCharging)
		{
			float kokang = (chrg.gaugeFill / chrg.gaugeMax * 5);
			jumpArea.transform.localScale = new Vector3(kokang, kokang);
			jumpArea.transform.position = new Vector3(transform.position.x, transform.position.z, 0);
		}
		else
		{
			jumpArea.transform.localScale = new Vector3(0, 0, 0);
		}

        if (isJump)
		{
			if (GetComponent<Rigidbody>().velocity.y < -0.5f) GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, -0.5f,GetComponent<Rigidbody>().velocity.y);
			transform.position = Vector3.MoveTowards(transform.position,
			                                         targetJump.transform.position,
			                                         speed*2 * Time.deltaTime);
			Vector2 vect1 = new Vector2(transform.position.x,transform.position.z);
			Vector2 vect2 = new Vector2(targetJump.transform.position.x,targetJump.transform.position.z);
			if (Vector2.Distance(vect1,vect2) <= .5 || controller.outLimit || controller.aboveTheWall && GetComponent<Rigidbody>().velocity.y<=0)
            {
                SoundManager.instance.PlaySfx("sfx_mJump2");
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
                controller.outLimit = false;
                isJump = false;
                controller.isMegaJump = false;
                Destroy(this);
            }
        }
    }

    public void FixedUpdate()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 7 && player != gameObject && player.GetComponent<SphereCollider>())
            {
                target = player;
            }
        }

        if (target)
        {
            if (!crosshairs) crosshairs = Instantiate(ObjectLibrary.instance.shootTarget) as GameObject;
            crosshairs.transform.position = new Vector3(target.transform.position.x, target.transform.position.z, 0);
            if (Vector3.Distance(transform.position, target.transform.position) > 7)
            {
                target = null;
                Destroy(crosshairs);
            }
            gameObject.GetComponent<CharacterController>().SetReady("megaJump");
        }
        else
        {
            gameObject.GetComponent<CharacterController>().itemReady.Remove("megaJump");
        }

        CheckExist();
    }

    public void CheckExist()
    {
        bool there = false;
        foreach (string item in controller.itemHold)
        {
            if (item == "megaJump") there = true;
        }
        if (!there)
        {
            //if (!isJump) Destroy(this);
        }
    }

    void OnDestroy()
    {
		if (node) Destroy (node);
        Destroy(jumpArea);
        Destroy(chrg);
        Destroy(crosshairs);
    }
}

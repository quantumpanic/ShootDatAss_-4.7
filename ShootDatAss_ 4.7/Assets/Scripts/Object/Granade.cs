using UnityEngine;
using System.Collections;

public class SmokeBomb : Granade
{
    public override void SpawnGrenade()
    {
        SmokeBomb001 grenade = new SmokeBomb001(gameObject).CreateComponent();
        grenade.target = target;
    }
}

public class Granade : ItemFunction {

	private CharacterController controller;
    public GameObject target;
    public GameObject jumpArea;
    public ChargeMechanic chrg;
    GameObject crosshairs;

    public void Start()
    {
        /*ChargeMechanic[] chrgs = gameObject.GetComponents<ChargeMechanic>(); //check if has charger
        if (chrgs.Length > 0)
        {
            foreach (ChargeMechanic a in chrgs)
            {
                if (a.id == slotNumber) // find the charger
                {
                    chrg = a;
                    break;
                }
            }
        }*/
        // no charger found. execute auto-aim
        controller = gameObject.GetComponent<CharacterController>();
        jumpArea = Instantiate(ObjectLibrary.instance.jumpArea) as GameObject;
        jumpArea.GetComponent<SpriteRenderer>().color = Color.red;
        jumpArea.transform.localScale = new Vector3(0, 0, 0);
        jumpArea.transform.parent = transform;
	}

    public void CancelThrow()
    {
        chrg.isCharging = false;
        chrg.gaugeFill = 0;
    }

    public void ChargedThrow()
    {
        string face = controller.gameObject.GetComponentInChildren<CharacterAnimationController>().faceDirection;
        float kokang = (chrg.gaugeFill / chrg.gaugeMax * 6);
        
        /*Vector3 originOfAngle; //this will be where you are casting your angle from.
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
        GameObject node = new GameObject();
        node.transform.position = targetPoint + controller.gameObject.transform.position;

        GameObject granade = new GameObject("Granade");
        granade.transform.parent = transform.parent;
        granade.transform.position = transform.position;
        GranadeThrow grnd = granade.AddComponent<GranadeThrow>();
        grnd.SetTarget(node, playerName);
        grnd.node = node;*/

        Grenade001 grenade = new Grenade001(gameObject).CreateComponent();
        grenade.throwRange = kokang/6 * new AltWeaponStats(AltWeapon.grenade).range;

        //gameObject.GetComponent<CharacterController>().itemHold[slotNumber] = "";
        //gameObject.GetComponent<CharacterController>().itemReady.Remove("granade");
        CancelThrow();
        Destroy(this);
    }

    public void Execute()
    {
        if (chrg.isCharging)
        {
            ChargedThrow();
            return;
        }
		if(target)
        {
            SpawnGrenade();
            //gameObject.GetComponent<CharacterController>().itemHold[slotNumber] = "";
            //gameObject.GetComponent<CharacterController>().itemReady.Remove("granade");
            Destroy(this);
        }
	}

    public virtual void SpawnGrenade()
    {
        Grenade001 grenade = new Grenade001(gameObject).CreateComponent();
        grenade.target = target;
    }

    void Update()
    {
        if (chrg.isCharging)
        {
            float kokang = (chrg.gaugeFill / chrg.gaugeMax * 4);
            jumpArea.transform.localScale = new Vector3(kokang, kokang);
            jumpArea.transform.position = MatchManager.IsometricScaling(transform.position);
        }
        else
        {
            jumpArea.transform.localScale = new Vector3(0, 0, 0);
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
            crosshairs.transform.position = new Vector3(target.transform.position.x, target.transform.position.z * MatchManager.instance.map2scrRatio, target.transform.position.y);
            if (Vector3.Distance(transform.position, target.transform.position) > 7)
            {
                target = null;
                Destroy(crosshairs);
            }
            gameObject.GetComponent<CharacterController>().SetReady("granade");
        }
        else
        {
            gameObject.GetComponent<CharacterController>().itemReady.Remove("granade");
        }

        CheckExist();
    }

    public void CheckExist()
    {
        bool there = false;
        foreach (string item in controller.itemHold)
        {
            if (item == "granade") there = true;
        }
        if (!there)
        {
            //Destroy(this);
        }
    }

    public void OnDestroy()
    {
        Destroy(jumpArea);
        Destroy(chrg);
        Destroy(crosshairs);
    }
}

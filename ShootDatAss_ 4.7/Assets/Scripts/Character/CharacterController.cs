using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemoteObjectManager
{
    public void SyncLocalPositionWithRemote()
    {
        //
    }

    public void SendLocalStateToServer()
    {
        //
    }

    public void InterpolateLastReceivedPosition()
    {
        //
    }

    public void ExtrapolateNextTargetPosition()
    {
        //
    }
}

public class CharacterController : MonoBehaviour {

    public int id;

	public string playerName;
	public float hp;
	public int speed = 1;
	public int shootRate = 1;
	public int shootRange = 1;
	public int power = 1;
	public int costumeID;

	public float baseSpeed = 2;

	public string[] itemHold;
	public List<string> itemReady;

	private float shootTime = 2;
	public bool remote;
	public Vector3 targetRemote;

	[HideInInspector]
	public GameObject player;
	private GameObject shadow;
	public bool onTheGround;
	public bool aboveTheWall;
    public bool outLimit;

    public GameObject shotPivot;
    public Weapon curWeapon;

	private bool isMovementControlled;
	private bool isMove;
	[HideInInspector]
	public bool isShoot;
	[HideInInspector]
	public bool isKnife;
	[HideInInspector]
	public bool isRocketLauncher;
	[HideInInspector]
	public bool die;
	[HideInInspector]
	public bool isMegaJump;
	[HideInInspector]
	public bool isSniping;

    public CharacterAnimationController animScript;
    public Unit unitComponent;

    public void Start()
    {
        if (remote)
        {
            //rigidbody.useGravity = false;
            //gameObject.GetComponent<SphereCollider> ().isTrigger = false;
        }
        animScript = gameObject.GetComponent<CharacterAnimationController>();
        unitComponent = gameObject.GetComponent<Unit>();
    }

    public void Move(float direction)
    {
        if (isMegaJump || isSniping) return;
        if (onTheGround && !die)
        {
            Vector3 towards = transform.position + Quaternion.AngleAxis(direction + 90, Vector3.down) * transform.forward * (baseSpeed + (speed * baseSpeed * 0.05f));
            //rigidbody.MovePosition(towards);
            //if (gameObject.GetComponent<TouchController>()) print(towards);
            Vector3 to = transform.position - towards;
            GetComponent<Rigidbody>().velocity = new Vector3(to.x, 0, to.z);
        }
        if (!isShoot)
            if (player) player.SendMessage("SetFaceDirection", new KeyValuePair<float, bool>(direction, false));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, .5f);
        Gizmos.DrawSphere(transform.position, .5f);
    }

    public CollisionFlags Move(Vector3 vect)
    {
        return new CollisionFlags();
    }

	public void ForceMove(string direction){
		if(direction.Contains("Idle"))direction = direction.Replace("Idle","");
		GetComponent<Rigidbody>().velocity = new Vector3 (GetVelocity(direction).x * (baseSpeed + (speed * baseSpeed * 0.05f))
		                                  , GetComponent<Rigidbody>().velocity.y
		                                  , GetVelocity(direction).y *  (baseSpeed + (speed * baseSpeed * 0.05f)));
		isMove = true;
	}
	
	public void PositionBaseMove (Vector3 targetPosition)
    {
		transform.position = targetPosition;
	}

    public void PositionBaseMove(float direction)
    {
        player.SendMessage("SetFaceDirection", new KeyValuePair<float, bool>(direction, false));
    }

    public void Shoot(float direction)
    {
        isShoot = true;
        bool idle = (isMove);

        player.SendMessage("SetFaceDirection", new KeyValuePair<float, bool>(direction, idle));
    }

	public void StopShoot(){
		isShoot = false;
	}

    void Shoot()
    {
        if (isShoot && !isKnife && !isRocketLauncher && !die && !isMegaJump && !isSniping)
        {
            if (shootTime > (0.5f - (shootRate * 0.03)))
            {
                if (gameObject == MatchManager.instance.localPlayer)
                {
                    //
                }
                switch (curWeapon)
                {
                    case Weapon.nailGun:
                        Nailgun001 nail = new Nailgun001(gameObject).CreateComponent();
                        break;
                    case Weapon.pulseLaser:
                        Pulsegun001 pulse = new Pulsegun001(gameObject).CreateComponent();
                        break;
                    case Weapon.paintBall:
                        Paintball001 paint = new Paintball001(gameObject).CreateComponent();
                        break;
                    case Weapon.AK47:
                        AssaultRifle001 assault = new AssaultRifle001(gameObject).CreateComponent();
                        break;
                    case Weapon.musket:
                        Musket001 musket = new Musket001(gameObject).CreateComponent();
                        break;
                    case Weapon.parley:
                        PirateGun001 parley = new PirateGun001(gameObject).CreateComponent();
                        break;
                    case Weapon.SMG:
                        SubMachineGun001 smg = new SubMachineGun001(gameObject).CreateComponent();
                        break;
                    case Weapon.shotgun:
                        Bullet001[] bulls = new Shotgun001(gameObject).CreateComponents(3, 7);
                        break;
                    default:
                        Bullet001 bullet = new Bullet001(gameObject).CreateComponent();
                        break;
                }
                shootTime = 0;
            }
        }
        if (shootTime < 2) shootTime += Time.deltaTime;
    }

    public float GetShootDirection()
    {
        float dir = 0;
        dir = shotPivot.transform.rotation.z-90;
        return dir-90;
    }

	void _Shoot(){
		if(isShoot && !isKnife && !isRocketLauncher && !die && !isMegaJump && !isSniping){
			GameObject bullet = null;

			string direction = player.GetComponent<CharacterAnimationController> ().faceDirection;
			if(direction.Contains("Idle"))direction = direction.Replace("Idle", "");

			if(shootTime > (0.6f - (shootRate * 0.03))) {
				bullet = ObjectLibrary.instance.GetBullet(costumeID, power, direction, GetVelocity(direction), playerName, shootRange);
				bullet.transform.parent = player.transform;
				bullet.transform.localPosition = player.GetComponent<CharacterAnimationController>().GetShootPoint();	
				bullet.transform.parent = transform.parent.transform;

				GameObject brust = bullet.transform.FindChild("Brust").gameObject;
				brust.transform.position = bullet.transform.position;
				brust.transform.parent = transform;
				brust.AddComponent<Brust>();
				brust.GetComponent<SpriteRenderer> ().sortingOrder = (int)(-transform.position.z * 10) + 3;

				shootTime = 0;
                SoundManager.instance.PlaySfx("sfx_gun");
			}
		}
		if(shootTime < 2) shootTime += Time.deltaTime;
	}

	public GameObject GetBullet(int costumeID){
		GameObject bullet = null;
		if(costumeID < 21){

		}else{
			
		}
		return bullet;
	}

	public Vector2 GetVelocity(string direction){

		Vector2 velocity = new Vector2 (0,0);

		if(direction == "right"){
			velocity = Vector2.right;
		}else if(direction == "upRight"){
			velocity = Vector2.right * 0.7f + Vector2.up * 0.7f;
		}else if(direction == "up"){
			velocity = Vector2.up;
		}else if(direction == "upLeft"){
			velocity = -Vector2.right * 0.7f + Vector2.up * 0.7f;
		}else if(direction == "left"){
			velocity = -Vector2.right;
		}else if(direction == "downLeft"){
			velocity = -Vector2.up * 0.7f + -Vector2.right * 0.7f;
		}else if(direction == "down"){
			velocity = -Vector2.up;
		}else if(direction == "downRight"){
			velocity = -Vector2.up  * 0.7f + Vector2.right * 0.7f;
		}else{
			velocity = Vector2.zero;
		}
		return velocity;
	}

	public void Update(){
		InstantiatePlayer ();
		PlayerPosition ();
		Shoot ();
		AddItemFunction ();
        RemoteSync();
        isMove = false;
	}

	public void InstantiatePlayer(){
		if(!player) {
			player = Instantiate(ObjectLibrary.instance.characterBase)as GameObject;
			player.name = ObjectLibrary.instance.characterBase.name;
			player.transform.parent = transform;
			player.SendMessage("SetCostume", costumeID);
			player.GetComponent<CharacterAnimationController>().playerName = playerName;
			name = playerName;

			hp = 100;
			speed = 1;
			shootRate = 1;
			shootRange = 1;
			power = 1;
		}

		if(!shadow){
			shadow = Instantiate(ObjectLibrary.instance.shadow)as GameObject;
			shadow.name = "shadow";
			shadow.transform.parent = transform;
		}
	}

	public void PlayerPosition(){
		float scaleToScr = MatchManager.instance.scr2mapRatio;
		float heightAdder = transform.position.y;
        if (onTheGround)
        {
            player.transform.position = new Vector3(transform.position.x, transform.position.z * scaleToScr, 0);
        }
        else
        {
            player.transform.position = new Vector3(transform.position.x, (transform.position.z + heightAdder) * scaleToScr, 0);
        }

        shadow.transform.position = new Vector3(transform.position.x, (transform.position.z - 0.17f) * MatchManager.instance.scr2mapRatio, 0);
		shadow.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) - 2;
		float scale = 0.5f - Mathf.Abs(transform.position.y / 10);
		if(scale > 0) shadow.transform.localScale = new Vector3 (scale, scale, scale);

		player.SendMessage("SetCostume", costumeID);
		player.SendMessage ("SetSortingLayer", transform.position.z);

		if(aboveTheWall && !onTheGround && !isMegaJump){
			MatchManager.instance.SendFallOnWall(playerName, player.GetComponent<CharacterAnimationController>().faceDirection);
		}
	}

	private int itemNew;
	public void AddItem(string itemName){

		if (itemName == "speed") {
			speed +=1;
            SoundManager.instance.PlaySfx("sfx_getPU");
		}else if (itemName == "shootRate"){
            shootRate += 1;
            SoundManager.instance.PlaySfx("sfx_getPU");
		}else if (itemName == "shootRange"){
            shootRange += 1;
            SoundManager.instance.PlaySfx("sfx_getPU");
		}else if (itemName == "power"){
            power += 1;
            SoundManager.instance.PlaySfx("sfx_getPU");
		}else if(itemHold[0] == ""){
            itemHold[0] = itemName;
            SoundManager.instance.PlaySfx("sfx_getweap");
		}else if(itemHold[1] == ""){
            itemHold[1] = itemName;
            SoundManager.instance.PlaySfx("sfx_getweap");
		}else if(itemHold[2] == ""){
            itemHold[2] = itemName;
            SoundManager.instance.PlaySfx("sfx_getweap");
		}else{
			if(itemNew == 0){
				itemHold[0] = itemName;
                itemNew = 1;
                SoundManager.instance.PlaySfx("sfx_getweap");
			}else if(itemNew == 1){
				itemHold[1] = itemName;
                itemNew = 2;
                SoundManager.instance.PlaySfx("sfx_getweap");
			}else if(itemNew == 2){
				itemHold[2] = itemName;
                itemNew = 0;
                SoundManager.instance.PlaySfx("sfx_getweap");
			}
		}
	}

    public void AddItemFunction()
    {
        for (int i = 0; i < itemHold.Length; i++)
        {
            if (itemHold[i] == "adrenaline")
            {
                SetReady("adrenaline");
            }
            else if (itemHold[i] == "granade")
            {
                if (!gameObject.GetComponent<Granade>())
                {
                    Granade grnd = gameObject.AddComponent<Granade>();
                    GrenadeCharger chrg = gameObject.AddComponent<GrenadeCharger>();
                    grnd.chrg = chrg;
                }
            }
            else if (itemHold[i] == "megaJump")
            {
                if (!gameObject.GetComponent<MegaJump>())
                {
                    MegaJump jump = gameObject.AddComponent<MegaJump>();
                    JumpCharger chrg = gameObject.AddComponent<JumpCharger>();
                    jump.chrg = chrg;
                }
            }
            else if (itemHold[i] == "hpBooster")
            {
                SetReady("hpBooster");
            }
            else if (itemHold[i] == "knife")
            {
                if (!gameObject.GetComponent<Knife>())
                {
                    gameObject.AddComponent<Knife>();
                }
            }
            else if (itemHold[i] == "power")
            {

            }
            else if (itemHold[i] == "rocketLauncher")
            {
                SetReady("rocketLauncher");
            }
            else if (itemHold[i] == "sniper")
            {

                if (!gameObject.GetComponent<Sniper>())
                {
                    gameObject.AddComponent<Sniper>().id = i;
                }

            }
            else if (itemHold[i] == "timeBomb")
            {
                TimeBomb[] tbs = gameObject.GetComponents<TimeBomb>();
                bool there = false;
                foreach (TimeBomb tb in tbs)
                {
                    if (tb.slotNumber == i)
                    {
                        there = true;
                        break;
                    }

                }
                if (!there)
                {
                    gameObject.AddComponent<TimeBomb>().slotNumber = i;
                }
            }
            else if (itemHold[i] == "timeBombExplode")
            {

            }
        }
    }

	public void ReadyChecker(){

	}

	public void SetReady(string itemName){
		if(!itemReady.Contains(itemName))itemReady.Add(itemName);
	}

	public void UseItem(string itemName, int slotNumber){
	}

	public void ChargeItem(string itemName, int slotNumber)
	{
        // charger not found. make new
        if (itemName == "granade")
        {
            GrenadeCharger chrgr = gameObject.GetComponent<GrenadeCharger>();
            if (chrgr) chrgr.isCharging = true;
        }

        else if (itemName == "megaJump")
        {
            JumpCharger chrgr = gameObject.GetComponent<JumpCharger>();
            if (chrgr) chrgr.isCharging = true;
        }
    }

	public void Hit(string shooter, float damage){
		hp -= damage;
		player.SendMessage ("GetHit");

		if(hp <= 0) die = true;
		if(die)MatchManager.instance.SendDie(playerName, shooter);
	}

	public void ResetPlayer(){
		speed = Reset(speed);
		shootRate = Reset(shootRate);
		shootRange = Reset(shootRange);
		power = Reset(power);
		hp = 100;
		die = false;
	}

	int Reset(int toReset){
		toReset -= Random.Range (0,15);
		if(toReset < 1) toReset = 1;
		return toReset;
	}

    public void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Ground")
        {
            onTheGround = true;
            aboveTheWall = false;
        }
        if (other.transform.tag == "Crate" && !onTheGround && isMegaJump)
        {
            MatchManager.instance.SendHitCrate(other.transform.name);
			//aboveTheWall = true;
        }
        if ((other.transform.tag == "Wall" || other.transform.tag == "Player") && !onTheGround)
        {
			aboveTheWall = true;
            float xDiff = transform.position.x - other.transform.position.x;
            float zDiff = transform.position.z - other.transform.position.z;
			GetComponent<Rigidbody>().velocity = new Vector3((xDiff>0)?3:-3, GetComponent<Rigidbody>().velocity.y, (zDiff>0)?3:-3);
        }

        if (other.transform.tag == "LimitWall")
        {
			outLimit = true;
			GetComponent<Rigidbody>().velocity = new Vector3((GetComponent<Rigidbody>().velocity.x>0)?3:-3, GetComponent<Rigidbody>().velocity.y, (GetComponent<Rigidbody>().velocity.z>0)?3:-3);
        }

        if (other.transform.tag == "Player" && isMegaJump)
        {
            MatchManager.instance.SendHitPlayer(playerName, other.transform.name, 10);
        }

    }

	public void OnCollisionExit(Collision other){
		if(other.transform.tag == "Ground"){
			onTheGround = false;
		}
		if(other.transform.tag == "LimitWall"){
			outLimit = false;
		}
	}

	public void SyncPosition(Vector3 position){
		//if(remote)targetRemote = new Vector3(position.x, transform.position.y, position.z);

	}

	void RemoteSync(){
		if (remote) {
			//transform.position = Vector3.MoveTowards(transform.position, targetRemote, 10 * Time.deltaTime);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour {
	
	public GameObject movementPad;
	public GameObject shootingPad;

	public GameObject movementAnalog;
	public GameObject shootingAnalog;

	public GameObject[] item;
	public GameObject[] slotBackGound;

    List<Touch> touchEvts = new List<Touch>();

	public Vector4 cameraLimit;
	
	private Vector3 movementPadPosition;
	private Vector3 shootingPadPosition;
	private Touch movementTouch;
	private Touch shootingTouch;
	private bool isMove;
	private bool isShoot;
	private float momentAngle;
	private float shootAngle;
    private Camera camera;
    private Camera indicators;

	private float directionFromKeyboard = 0;
	public float shootDirectionFromMouse;

	public List<SpriteRenderer> powerBar;
	public List<SpriteRenderer> shootRangeBar;
	public List<SpriteRenderer> shootRateBar;
	public List<SpriteRenderer> speedBar;

	void Awake(){
		item = new GameObject[3];
		slotBackGound = new GameObject[3];

		powerBar = new List<SpriteRenderer> ();
		shootRangeBar = new List<SpriteRenderer> (); 
		shootRateBar =  new List<SpriteRenderer> ();
		speedBar =  new List<SpriteRenderer> ();
	}

	void Start(){
		GetMovementAndShootPostition ();
		FindSlotSprite ();
		InitializeItem ();
	}


	void FixedUpdate () {
		//Test
		if(Input.GetKeyUp(KeyCode.Escape))Application.LoadLevel(0);

		if(!movementPad) movementPad = GameObject.Find("movementPad");
		if(!shootingPad) shootingPad = GameObject.Find("shootingPad");
		if(!movementAnalog) movementAnalog = GameObject.Find("movementAnalog");
		if(!shootingAnalog) shootingAnalog = GameObject.Find("shootingAnalog");
		if(!item[0]) item[0] =  GameObject.Find("item1");
		if(!item[1]) item[1] =  GameObject.Find("item2");
		if(!item[2]) item[2] =  GameObject.Find("item3");

#if UNITY_EDITOR || UNITY_STANDALONE
        DetectKeyboardAndMouse();
#elif UNITY_ANDROID
        DetectTouch ();
#endif
        AnalogPosition ();
		CameraController ();

	}

	void Update(){
		ChangeItemSlot ();
	}

	void FindSlotSprite(){
		for(int i=0 ; i<slotBackGound.Length ; i++){
			slotBackGound[i] = GameObject.Find("slotBackGound" + (i +1));
			slotBackGound[i].AddComponent<TouchUseItem>().Initialize(i, gameObject.GetComponent<TouchController>());
		}
	}

	void CameraController(){
		if(camera){
			float targetCameraX = transform.position.x; 
			float targetCameraY = transform.position.z; 

			//Position X limit
			if(transform.position.x < cameraLimit.x)  targetCameraX = cameraLimit.x;
			else if(transform.position.x > cameraLimit.y)  targetCameraX = cameraLimit.y;

			//Position Y limit
			if(transform.position.z < cameraLimit.z)  targetCameraY = cameraLimit.z;
			else if(transform.position.z > cameraLimit.w)  targetCameraY = cameraLimit.w;

			camera.transform.position = Vector3.Lerp(camera.transform.position,
			                                         new Vector3 (targetCameraX, targetCameraY, camera.transform.position.z),
			                                         4 * Time.deltaTime);
		}
	}

	void DetectTouch(){
        if (Input.touchCount > 0)
        {
            touchEvts.Clear();
            for (int t = 0; t <= Input.touchCount - 1; t++)
            {
                touchEvts.Add(Input.GetTouch(t));
            }
        }

		if(Input.touchCount == 1){
			if(Input.GetTouch(0).position.x < Screen.width / 2){
				isMove = true;
				isShoot = false;
				movementTouch = Input.GetTouch(0);
			}else{
				isMove = false;
				isShoot = true;
				shootingTouch = Input.GetTouch(0);
            }
            touchEvts.Add(Input.GetTouch(0));
		}

		if(Input.touchCount > 1){ 
			if(Input.GetTouch(0).position.x < Screen.width / 2){
				isMove = true;
				movementTouch = Input.GetTouch(0);
			}else{
				isShoot = true;
				shootingTouch = Input.GetTouch(0);
			}

			if(Input.GetTouch(1).position.x < Screen.width / 2){
				isMove = true;
				movementTouch = Input.GetTouch(1);
			}else{
				isShoot = true;
				shootingTouch = Input.GetTouch(1);
			}
		}

		if(Input.touchCount == 0){
			isMove = false;
			isShoot = false;
		}

		MovePlayer ();
		ShootPlayer ();
	}

    bool idle = false;
	void DetectKeyboardAndMouse(){

		if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)){
			directionFromKeyboard = 45;
		}else if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)){
			directionFromKeyboard = 135;
		}else if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)){
			directionFromKeyboard = 315;
		}else if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)){
			directionFromKeyboard = 225;
		}else if(Input.GetKey(KeyCode.W)){
			directionFromKeyboard = 90;
		}else if(Input.GetKey(KeyCode.D)){
			directionFromKeyboard = 0;
		}else if(Input.GetKey(KeyCode.S)){
			directionFromKeyboard = 270;
		}else if(Input.GetKey(KeyCode.A)){
			directionFromKeyboard = 180;
		}
        else if (Input.GetKey(KeyCode.E))
        {
            gameObject.GetComponent<CharacterController>().AddItem("granade");
		}
		else if (Input.GetKey(KeyCode.R))
		{
            gameObject.GetComponent<CharacterController>().AddItem("megaJump");
		}

		if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W)){
			if(!idle)idle=true;
			isMove = false;
		}else{
			isMove = true;
        }

		if(Input.GetMouseButton(0)){
			Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 playerPosition = transform.FindChild("Character").position;

			float angle = Mathf.Atan2 (mousePosition.y - playerPosition.y, mousePosition.x - playerPosition.x)*180/Mathf.PI;
			if(angle < 0) angle = angle + 360;

			shootDirectionFromMouse = angle;

			MatchManager.instance.SendShoot (gameObject, angle);
		}else{
			MatchManager.instance.SendStopShoot (gameObject);
		}


		if(Input.GetKeyDown(KeyCode.Alpha1)){
			UseItem(0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			UseItem(1);
		}
		if(Input.GetKeyDown(KeyCode.Alpha3)){
			UseItem(2);
        }

        if (isMove) MatchManager.instance.SendMove(gameObject, directionFromKeyboard, idle, transform.position);
	}


	void GetMovementAndShootPostition(){
		if(!movementPad) movementPad = GameObject.Find("movementPad");
		if(!shootingPad) shootingPad = GameObject.Find("shootingPad");
		if(!movementAnalog) movementAnalog = GameObject.Find("movementAnalog");
		if(!shootingAnalog) shootingAnalog = GameObject.Find("shootingAnalog");
		if(!camera) camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		if(!item[0]) item[0] =  GameObject.Find("item1");
		if(!item[1]) item[1] =  GameObject.Find("item2");
		if(!item[2]) item[2] =  GameObject.Find("item3");

		cameraLimit = MatchManager.instance.cameraLimit;

		movementPadPosition = camera.WorldToScreenPoint(movementPad.transform.position);
		shootingPadPosition = camera.WorldToScreenPoint(shootingPad.transform.position);
	}

	void AnalogPosition(){
		Vector3 movementTouchOnWorld = camera.ScreenToWorldPoint (movementTouch.position);
		Vector3 shootingTouchOnWorld = camera.ScreenToWorldPoint (shootingTouch.position);

		if(!isMove){
			movementAnalog.transform.localPosition = Vector3.Lerp(movementAnalog.transform.localPosition, Vector3.zero, 0.1f);
		}else{
			if(movementTouch.position != Vector2.zero)
				SetAnalogPosition (movementTouchOnWorld, movementPad.transform.position, movementAnalog);
		}

		if(!isShoot){
			shootingAnalog.transform.localPosition = Vector3.Lerp(shootingAnalog.transform.localPosition, Vector3.zero, 0.1f);
		}else{
			if(shootingTouch.position != Vector2.zero)
				SetAnalogPosition (shootingTouchOnWorld, shootingPad.transform.position, shootingAnalog);
		}
	}

	void SetAnalogPosition(Vector3 touchPosition, Vector3 center, GameObject objectToMove){
		if (Vector2.Distance (new Vector2 (touchPosition.x, touchPosition.y), new Vector2 (center.x, center.y)) < 0.25f) {
			objectToMove.transform.localPosition = new Vector3(touchPosition.x, touchPosition.y, 0);
		}else{
			Vector3 newPos = touchPosition - center;
			newPos.Normalize ();
			newPos *= 0.25f;
			objectToMove.transform.localPosition = new Vector3(newPos.x, newPos.y, 0);
		}
	}

	void MovePlayer(){
		float angle = Mathf.Atan2 (movementTouch.position.y - movementPadPosition.y, movementTouch.position.x - movementPadPosition.x)*180/Mathf.PI;
		if(angle < 0) angle = angle + 360;
		
		if(isMove){
			MatchManager.instance.SendMove(gameObject, angle,false, transform.position);
		}else{
            MatchManager.instance.SendMove(gameObject, angle, true, transform.position);
		}
	}

	void ShootPlayer(){

		float angle = Mathf.Atan2 (shootingTouch.position.y - shootingPadPosition.y, shootingTouch.position.x - shootingPadPosition.x)*180/Mathf.PI;
		if(angle < 0) angle = angle + 360;

		if(isShoot){
            //MatchManager.instance.SendShoot(gameObject, GetDirection(angle));
            MatchManager.instance.SendShoot(gameObject, angle);
		}else{
			MatchManager.instance.SendStopShoot(gameObject);
		}
	}


	public void UseItem(int slotNumber){
		//if(gameObject.GetComponent<CharacterController>().itemHold[slotNumber] != "")
		MatchManager.instance.SendUseItem (name,
		                                   gameObject.GetComponent<CharacterController>().itemHold[slotNumber],
		                                   slotNumber);
	}
	
	
	public void ChargeItem(int slotNumber){
		//if(gameObject.GetComponent<CharacterController>().itemHold[slotNumber] != "")
			MatchManager.instance.SendChargeItem (name,
			                                   gameObject.GetComponent<CharacterController>().itemHold[slotNumber],
			                                   slotNumber);
	}

	string GetDirection(float angle){
		string direction = "";

		if(angle < 22.5f || angle > 337.5){
			direction = "right";
		}else if(angle > 22.5f && angle < 67.5f){
			direction = "upRight";
		}else if(angle > 67.5 && angle < 112.5f){
			direction = "up";
		}else if(angle > 112.5f && angle < 157.5f){
			direction = "upLeft";
		}else if(angle > 157.5f && angle < 202.5f){
			direction = "left";
		}else if(angle > 202.5f && angle < 247.5f){
			direction = "downLeft";
		}else if(angle > 247.5f && angle < 292.5f){
			direction = "down";
		}else if(angle > 292.5f && angle < 337.5f){
			direction = "downRight";
		}

		return direction;
	}

	void ChangeItemSlot(){
        Player001 plyr = gameObject.GetComponent<Player001>();
        if (plyr == null) return;
        for (int s = 0; s < item.Length; s++)
        {
            Player001.Belt.Slot slt = plyr.belt.slots[s];
            SpriteRenderer sprt = item[s].GetComponent<SpriteRenderer>();
            SpriteRenderer slotsprt = slotBackGound[s].GetComponent<SpriteRenderer>();

            //update item sprite
            if (slt.item == BeltItem.Empty)
            {
                sprt.sprite = null;
            }
            else
            {
                ItemInfo toChange = ObjectLibrary.instance.itemFromCrate.Find(x => x.beltItem == slt.item);
                sprt.sprite = toChange.itemObject.GetComponent<Item>().objectBase.GetComponent<SpriteRenderer>().sprite;
                item[s].transform.eulerAngles = Vector3.zero;
            }

            //check active
            if (!slt.ready)
            {
                slotsprt.sprite = ObjectLibrary.instance.controllerSprite[0];
            }
            else
            {
                slotsprt.sprite = ObjectLibrary.instance.controllerSprite[1];
            }

            //check slot state
            if (slt.state == slotState.disabled)
            {
                slotsprt.sprite = ObjectLibrary.instance.controllerSprite[2];
            }
            else if (slt.state == slotState.locked)
            {
                slotsprt.sprite = ObjectLibrary.instance.controllerSprite[3];
            }
        }

		/*if(gameObject.GetComponent<CharacterController>().itemHold[0] != ""){
			item[0].GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.GetItemSprite(gameObject.GetComponent<CharacterController>().itemHold[0]);
		}else{
			item[0].GetComponent<SpriteRenderer>().sprite = null;
		}

		if(gameObject.GetComponent<CharacterController>().itemHold[1] != ""){
			item[1].GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.GetItemSprite(gameObject.GetComponent<CharacterController>().itemHold[1]);
		}else{
			item[1].GetComponent<SpriteRenderer>().sprite = null;
		}

		if(gameObject.GetComponent<CharacterController>().itemHold[2] != ""){
			item[2].GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.GetItemSprite(gameObject.GetComponent<CharacterController>().itemHold[2]);
		}else{
			item[2].GetComponent<SpriteRenderer>().sprite = null;
		}

		item[0].transform.eulerAngles = new Vector3 (0,0,0);
		item[1].transform.eulerAngles = new Vector3 (0,0,0);
		item[2].transform.eulerAngles = new Vector3 (0,0,0);*/


		for(int i=0; i < gameObject.GetComponent<CharacterController>().itemHold.Length; i++){
			bool there = false;
			foreach(string itemName in gameObject.GetComponent<CharacterController>().itemReady){
				if(gameObject.GetComponent<CharacterController>().itemHold[i] == itemName){
					there = true;
					break;
				}else if(gameObject.GetComponent<CharacterController>().itemHold[i] == ("timeBombExplode" + (i - 1))){
					there = true;
					break;
				}
			}

			try{
				if(there){
					//slotBackGound[i].GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.controllerSprite[1];
				}else{
					//slotBackGound[i].GetComponent<SpriteRenderer>().sprite = ObjectLibrary.instance.controllerSprite[0];
				}
			}catch{}
		}
	}

	void InitializeItem(){
		SearchBarObject (powerBar, "powerBar");
		SearchBarObject (shootRangeBar, "shootRange");
		SearchBarObject (shootRateBar, "shootRate");
		SearchBarObject (speedBar, "speed");
	}

	void SearchBarObject(List<SpriteRenderer> bar, string barName){
		for(int i=0; i<5; i++){
			//print(GameObject.Find(barName + (i+1)).name);
			bar.Add(GameObject.Find(barName + (i+1)).GetComponent<SpriteRenderer>());
		}
	}
}

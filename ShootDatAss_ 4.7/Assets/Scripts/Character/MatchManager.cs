using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class MatchManager : MonoBehaviour
{

    public static MatchManager instance;

    //Test
    //public List<GameObject> playerToSpawn;
    public GameObject ownerName;
    public GameObject playerBase;
    public List<GameObject> player;
    public GameObject[] playerFirstSpawnBase;
    public GameObject[] playerSpawnPoint;
    public Vector4 cameraLimit;
    public List<GameObject> crate;
    public List<string> gameEvent;
    public bool multiplayer;

    public BoxCollider realMap;
    public BoxCollider scrMap;

    public float map2scrRatio;
    public float scr2mapRatio;

    public GameObject localPlayer;

    public void Start()
    {
        instance = this;

        //real coords to map
        map2scrRatio = realMap.size.z / scrMap.size.y;
        scr2mapRatio = scrMap.size.y / realMap.size.z;

        IntitializeCrate();
        //InitializeSpring();
        InitializeController();

        //Test
        //SetPlayer (playerToSpawn);
    }

    public void Update()
    {
        if (instance == null) instance = this;
    }

    public static Vector3 IsometricScaling(Vector3 VectToScale, float heightMod = 0, bool toCanvas = true)
    {
        float scaleRatio;
        float vecX;
        float vecY;
        float vecZ;

        if (toCanvas)
        {
            scaleRatio = instance.scr2mapRatio;
            vecX = VectToScale.x;
            vecY = (VectToScale.z * scaleRatio) + heightMod;
            vecZ = 0;
        }
        else
        {
            scaleRatio = instance.map2scrRatio;
            vecX = VectToScale.x;
            vecY = 0;
            vecZ = (VectToScale.y * scaleRatio) - heightMod;
        }

        return new Vector3(vecX, vecY, vecZ);
    }

    #region Game preps

    public void PreparePracticeGame(List<string> names, List<int> costumes, int id)
    {
        for (int i = 0; i < names.ToArray().Length; i++)
        {
            GameObject playerSpawn = Instantiate(ObjectLibrary.instance.playerBase, playerFirstSpawnBase[i].transform.position, Quaternion.identity) as GameObject;
            playerSpawn.GetComponent<CharacterController>().costumeID = costumes[i];
            playerSpawn.GetComponent<CharacterController>().playerName = names[i] + i;
            playerSpawn.GetComponent<CharacterController>().curWeapon = (Weapon)Random.Range(0, 9);
            playerSpawn.GetComponent<CharacterController>().id = i;
            playerSpawn.transform.parent = transform;
            player.Add(playerSpawn);
            if (id == i)
            {
                playerSpawn.AddComponent<TouchController>();
                playerSpawn.AddComponent<Player001>();
                //playerSpawn.AddComponent<NetworkTransformSender>();
                //NetworkTransform trns = NetworkTransform.FromTransform(playerSpawn.transform);
                //Network.instance.SendSpawnMe(trns);
                localPlayer = playerSpawn;
            }
            else
            {
                playerSpawn.AddComponent<AIController>();
                //playerSpawn.AddComponent<NetworkTransformInterpolation>();
                //playerSpawn.AddComponent<NetworkTransformReceiver>();
            }
        }
    }

    public void PrepareQuickPlayGame(List<PlayerVariable> playerVariable, int id)
    {
        for (int i = 0; i < playerVariable.ToArray().Length; i++)
        {
            GameObject playerSpawn = Instantiate(ObjectLibrary.instance.playerBase, playerFirstSpawnBase[i].transform.position, Quaternion.identity) as GameObject;
            playerSpawn.GetComponent<CharacterController>().costumeID = playerVariable[i].costume;
            playerSpawn.GetComponent<CharacterController>().playerName = playerVariable[i].playerID.ToString();
            if (id == playerVariable[i].playerID) playerSpawn.AddComponent<TouchController>();
            else playerSpawn.GetComponent<CharacterController>().remote = true;
            playerSpawn.transform.parent = transform;
            player.Add(playerSpawn);
        }
        multiplayer = true;
    }

    public void SetPlayer(List<GameObject> players)
    {

        for (int i = 0; i < players.ToArray().Length; i++)
        {
            if (!player.Contains(players[i]))
            {
                GameObject go = Instantiate(players[i], playerFirstSpawnBase[i].transform.position, Quaternion.identity) as GameObject;
                go.transform.parent = transform;
                go.name = "Player" + i;
                player.Add(go);
            }
        }
    }

    public void IntitializeCrate()
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("Crate");
        foreach (GameObject crt in crates)
        {
            if (!crate.Contains(crt))
            {
                crate.Add(crt);
                GameObject crts = Instantiate(ObjectLibrary.instance.crate,
                                              new Vector3(crt.transform.position.x,
                                                          (crt.transform.position.z * scr2mapRatio),
                                                          0),
                                              Quaternion.identity) as GameObject;
                crts.name = crt.name;
                crts.transform.parent = crt.transform;
                crts.GetComponent<SpriteRenderer>().sortingOrder = (int)(-crt.transform.position.z * 10);
                crts.tag = "Crate";
            }
        }
    }

    public void InitializeSpring()
    {
        GameObject[] springs = GameObject.FindGameObjectsWithTag("Spring");
        foreach (GameObject spring in springs)
        {
            GameObject spr = Instantiate(ObjectLibrary.instance.spring,
                                            new Vector3(spring.transform.position.x,
                                                        spring.transform.position.z,
                                                        0),
                                            Quaternion.identity) as GameObject;
            spr.name = spring.name;
            spr.transform.parent = spring.transform;
            spr.transform.FindChild("Spring").GetComponent<SpriteRenderer>().sortingOrder = (int)(-spring.transform.position.z * 10);
            spring.AddComponent<Spring>();
        }
    }

    public void InitializeController()
    {
        GameObject controller = Instantiate(ObjectLibrary.instance.touchController) as GameObject;
        controller.name = "Controller";
        controller.transform.parent = GameObject.Find("Main Camera").transform;
        controller.transform.localPosition = new Vector3(0, 0, 11);
    }

    #endregion

    #region Network correspondence

    public void SendMove(GameObject sender, float direction, bool idle, Vector3 position)
    {
        if (!multiplayer) Move(sender.name, direction, position);
        else
        {

        }
    }

    public void SendShoot(GameObject sender, float direction)
    {
        if (!multiplayer) Shoot(sender.name, direction);
    }

    public void SendStopShoot(GameObject sender)
    {
        if (!multiplayer) StopShoot(sender.name);
    }

    public void SendHitCrate(string crateName)
    {
        if (!multiplayer) HitCrate(crateName, Random.Range(0, ObjectLibrary.instance.itemFromCrate.Count));
    }

    public void SendGetItem(string playerName, ItemInfo info)
    {
        if (!multiplayer) GetItem(playerName, info);
    }

    public void SendUseItem(string playerName, string item, int slotNumber)
    {
        if (!multiplayer)
        {
            UseItem(playerName, item, slotNumber);
        }
    }

    public void SendChargeItem(string playerName, string item, int slotNumber)
    {
        if (!multiplayer)
        {
            ChargeItem(playerName, item, slotNumber);
        }
    }

    public void SendFallOnWall(string playerName, string direction)
    {
        if (!multiplayer) FallOnWall(playerName, direction);
    }

    public void SendHitPlayer(string playerName, string victimName, float power)
    {
        if (!multiplayer) HitPlayer(playerName, victimName, power);
    }

    public void SendDie(string playerName, string shooter)
    {
        if (!multiplayer) Die(playerName, shooter);
    }

    #endregion

    #region In-game events

    public void Move(string sender, float direction, Vector3 position)
    {
        player.Find(x => x.name == sender).GetComponent<CharacterController>().Move(direction);
        if (multiplayer) player.Find(x => x.name == sender).GetComponent<CharacterController>().SyncPosition(position);
    }

    public void Shoot(string sender, float direction, int id = 0)
    {
        if (multiplayer)
        {
            ISFSObject data = new SFSObject();
            string bulletId = Network.instance.sfs.MySelf.Id.ToString() + id.ToString();
            data.PutUtfString("id", bulletId);
            data.PutInt("rot", (int)direction);
            ExtensionRequest rqst = new ExtensionRequest("shot", data);
            Network.instance.sfs.Send(rqst);
        }
        else player.Find(x => x.name == sender).GetComponent<CharacterController>().Shoot(direction);
    }

    public void StopShoot(string sender)
    {
        player.Find(x => x.name == sender).GetComponent<CharacterController>().StopShoot();
    }

    public void HitCrate(string crateName, int item)
    {
        foreach (GameObject crt in crate)
        {
            try
            {
                if (crt.name == crateName)
                {
                    Crate chk = crt.GetComponentInChildren<Crate>();
                    if (chk.isDead) return;
                    else chk.isDead = true;

                    GameObject itemFromCrate = Instantiate(ObjectLibrary.instance.itemFromCrate[item].itemObject) as GameObject;
                    itemFromCrate.name = ObjectLibrary.instance.itemFromCrate[item].itemObject.name;
                    itemFromCrate.transform.position = crt.transform.position;
                    itemFromCrate.transform.parent = transform;
                    Destroy(crt);
                    SoundManager.instance.PlaySfx("sfx_crateBrk");
                    break;
                }
            }
            catch { }
        }
    }

    public void GetItem(string playerName, ItemInfo item)
    {
        /*foreach (GameObject plyr in player)
        {
            if (plyr.GetComponent<CharacterController>().playerName == playerName)
            {
                plyr.GetComponent<CharacterController>().AddItem(itemName);
                break;
            }
        }*/
        CharacterController plyr = GameObject.Find(playerName).GetComponent<CharacterController>();
        foreach (ItemInfo i in ObjectLibrary.instance.itemFromCrate)
        {
            if (i.beltItem == item.beltItem)
            {
                Player001 unit = plyr.unitComponent as Player001;
                if (unit == null) return;
                if (i.beltItem == BeltItem.PowerUp)
                {
                    unit.AddPowerUp(item.itemObject.name);
                }
                else
                {
                    unit.AddItemToBelt(i.beltItem);
                }
                return;
            }
        }
    }

    public void UseItem(string playerName, string item, int slotNumber)
    {
        /*foreach (GameObject plyr in player)
        {
            if (plyr.GetComponent<CharacterController>().playerName == playerName)
            {
                plyr.GetComponent<CharacterController>().UseItem(item, slotNumber);
                break;
            }
        }*/
        Player001 plyr = GameObject.Find(playerName).GetComponent<Player001>();
        if (plyr != null) plyr.UseBeltItem(slotNumber);
    }

    public void ChargeItem(string playerName, string item, int slotNumber)
    {
        /*foreach (GameObject plyr in player)
        {
            if (plyr.GetComponent<CharacterController>().playerName == playerName)
            {
                plyr.GetComponent<CharacterController>().ChargeItem(item, slotNumber);
                break;
            }
        }*/
        Player001 plyr = GameObject.Find(playerName).GetComponent<Player001>();
        if (plyr != null) plyr.UseBeltItem(slotNumber,true);
    }

    public void FallOnWall(string playerName, string direction)
    {
        foreach (GameObject plyr in player)
        {
            if (plyr.GetComponent<CharacterController>().playerName == playerName)
            {
                plyr.GetComponent<CharacterController>().ForceMove(direction);
                break;
            }
        }
    }

    public void HitPlayer(string playerName, string victimName, float power)
    {
        foreach (GameObject plyr in player)
        {
            if (plyr.GetComponent<CharacterController>().playerName == victimName)
            {
                plyr.GetComponent<CharacterController>().Hit(playerName, power);
                SoundManager.instance.PlaySfx("sfx_hurt");
                break;
            }
        }
    }

    public void Die(string playerName, string shooter)
    {
        gameEvent.Add(shooter + " killed " + playerName);
        player.Find(x => x.name == playerName).SetActive(false);
        int id = player.Find(x => x.name == playerName).GetComponent<CharacterController>().id;
        
        //bakal di calculate di server
        StartCoroutine(WaitToSpawn(playerName,id)); // spawn from client. just for preview
    }

    public void SpawnPlayer(string playerName)
    {
        GameObject player = GameObject.Find(playerName);
        player.GetComponent<CharacterController>().ResetPlayer();
        player.SetActive(true);

        int rndspwn = Random.Range(0, playerSpawnPoint.Length);
        player.transform.position = playerSpawnPoint[rndspwn].transform.position;
    }

    public void SpawnPlayer(string playerName, int playerId)
    {
        if (GameObject.Find(playerName+playerId) == null) InitPlayer(playerName, playerId);

        GameObject.Find(playerName).GetComponent<CharacterController>().ResetPlayer();
        GameObject.Find(playerName).SetActive(true);

        if (playerName != localPlayer.name) return; // not local player, no need to randomspawn

        int rndspwn = Random.Range(0, playerSpawnPoint.Length);
        GameObject.Find(playerName).transform.position = playerSpawnPoint[rndspwn].transform.position;
    }

    void InitPlayer(string name, int id)
    {
        Vector3 spwnpt = new Vector3(-100, -100, -100); // spawn outside game area. interpolate will move it back inside
        //int spwnptIndex = Random.Range(0, playerFirstSpawnBase.Length);
        spwnpt = new Vector3(0, 0.5f, 0);
        
        //create the gameObject first
        //GameObject playerSpawn = Instantiate(ObjectLibrary.instance.playerBase, playerFirstSpawnBase[spwnptIndex].transform.position, Quaternion.identity) as GameObject;
        GameObject playerSpawn = Instantiate(ObjectLibrary.instance.playerBase, spwnpt, Quaternion.identity) as GameObject;
        playerSpawn.name = name;

        //attach the controller and add player to match list
        playerSpawn.transform.parent = transform;
        player.Add(playerSpawn);

        //now create the animator and attach to object
        GameObject character = Instantiate(ObjectLibrary.instance.characterBase) as GameObject;
        character.transform.parent = playerSpawn.transform;
        character.transform.localPosition = Vector3.zero;

        // set all relevant values
        playerSpawn.GetComponent<CharacterController>().costumeID = 0;
        playerSpawn.GetComponent<CharacterController>().playerName = name;
        playerSpawn.GetComponent<CharacterController>().id = id;

        character.GetComponent<CharacterAnimationController>().preview = true;
        character.GetComponent<CharacterAnimationController>().costume = 0;
        character.GetComponent<CharacterAnimationController>().SetSortingLayer(10000);
        character.GetComponent<CharacterAnimationController>().SetFaceDirection(new KeyValuePair<float, bool>(270, true));
        character.name = name;
        
        return;
        
        // if it's the local player, add input receiver and network communicator. else add interpolator
        if (name == Network.instance.sfs.MySelf.Name)
        {
            playerSpawn.AddComponent<TouchController>();
            playerSpawn.AddComponent<NetworkTransformSender>();
            NetworkTransform trns = NetworkTransform.FromTransform(playerSpawn.transform);
            Network.instance.SendSpawnMe(trns);
            localPlayer = playerSpawn;
        }
        else
        {
            playerSpawn.AddComponent<NetworkTransformInterpolation>();
            playerSpawn.AddComponent<NetworkTransformReceiver>();
        }
    }

    IEnumerator WaitToSpawn(string playerName, int playerId = -1)
    {
        yield return new WaitForSeconds(5);
        
        int spwnpt = Random.Range(0, playerSpawnPoint.Length);
        if (playerId < 0) SpawnPlayer(playerName);
        else SpawnPlayer(playerName,playerId);

        Transform spwnptObj = playerSpawnPoint[spwnpt].transform;
        NetworkTransform trns = NetworkTransform.FromTransform(spwnptObj);

        Network.instance.SendSpawnMe(trns);
    }

    #endregion
}

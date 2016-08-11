using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GranadeThrow : MonoBehaviour {

	public string playerName;
	public GameObject target;
	private GameObject granade;
    public GameObject node;

    Vector3 oriPos;
    // starting position when throw
    // so grenade wont' follow target to very far distances (like when teleport)

    bool isTracking;
    Vector3 lastTrack;
    // last track the target. for cases like teleport.
    // grenade just land on location where target is last seen.

    void Start()
    {
        isTracking = true;
        lastTrack = target.transform.position;
        oriPos = gameObject.transform.localPosition;

		granade = new GameObject ("granadeThrow");
		granade.transform.parent = transform;
		granade.AddComponent<SpriteRenderer> ().sprite = ObjectLibrary.instance.GetItemSprite ("granade");
        granade.transform.position = new Vector3(transform.position.x, (transform.position.z + transform.position.y)*MatchManager.instance.scr2mapRatio, 0);
		granade.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
	}

    void Update()
    {
        if (isTracking) lastTrack = (target)?target.transform.position:node.transform.position; // track target if within max distance
        granade.transform.position = new Vector3(transform.position.x, (transform.position.z + transform.position.y + 0.3f) * MatchManager.instance.scr2mapRatio, 0);
        granade.GetComponent<SpriteRenderer>().sortingOrder = (int)(-10 * transform.position.z) + 3;
        granade.transform.Rotate(new Vector3(0, 0, 20));

        transform.position = Vector3.MoveTowards(transform.position, lastTrack, 15 * Time.deltaTime);
        float distMin = Vector3.Distance(transform.position, lastTrack);
        float distMax = Vector3.Distance(oriPos, lastTrack); // distance to target when first throw
        if (distMin < 0.1f)
        {
            InstantiateExplosion();
            Destroy(gameObject);
            if (node) Destroy(node);
        }
        else if (distMax > 100)
        {
            //isTracking = false; // went outside max distance. stop tracking
        }
    }

	public void SetTarget(GameObject targetGranade, string name){
		target = targetGranade;
		playerName = name;
	}

    public void InstantiateExplosion()
    {
        SoundManager.instance.PlaySfx("sfx_grndDie");
		GameObject explotion = Instantiate(ObjectLibrary.instance.explotion[1])as GameObject;
		explotion.name = "explotion";
		explotion.transform.parent = transform.parent;
        explotion.transform.position = new Vector3(transform.position.x, (transform.position.z + 0.25f) * MatchManager.instance.scr2mapRatio, 0);
		explotion.GetComponent<SpriteRenderer> ().sortingOrder = (int)(-transform.position.z * 10) + 5;
        //print (transform.position + " | " + explotion.transform.position);

        //explode and damage all nearby enemies
        foreach (GameObject a in MatchManager.instance.player)
        {
            float distAOE = Vector3.Distance(a.transform.position, this.transform.position);
            if (distAOE < 2) MatchManager.instance.SendHitPlayer(playerName, a.name, 5);
        }

	}

}

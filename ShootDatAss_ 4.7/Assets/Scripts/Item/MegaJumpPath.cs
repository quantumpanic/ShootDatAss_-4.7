using UnityEngine;
using System.Collections;

public class MegaJumpPath : MonoBehaviour {

    public GameObject jumper;
    public string playerName;
    public GameObject target;
    private GameObject path;

    Vector3 oriPos;
    // starting position when jump
    // so player wont' follow target to very far distances (like when teleport)

    bool isTracking;
    Vector3 lastTrack;
    // last track the target. for cases like teleport.
    // player just land on location where target is last seen.

    void Start()
    {
        isTracking = true;
        lastTrack = target.transform.position;
        oriPos = gameObject.transform.localPosition;
    }

    void Update()
    {
        if (isTracking) lastTrack = target.transform.position; // track target if within max distance

        transform.position = Vector3.MoveTowards(transform.position, lastTrack, 15 * Time.deltaTime);
        jumper.transform.position = new Vector3(transform.position.x, transform.position.y, 0); // player follow jump path

        float distMin = Vector3.Distance(transform.position, lastTrack);
        float distMax = Vector3.Distance(oriPos, lastTrack); // distance to target when first jump
        if (distMin < 0.1f)
        {
            MatchManager.instance.SendHitPlayer(playerName, target.name, 5);
            InstantiateExplosion();
            Destroy(gameObject);
        }
        else if (distMax > 100)
        {
            //isTracking = false; // went outside max distance. stop tracking
        }
    }

    public void SetTarget(GameObject targetGranade, string name)
    {
        target = targetGranade;
        playerName = name;
    }

    public void InstantiateExplosion()
    {
        GameObject explotion = Instantiate(ObjectLibrary.instance.explotion[1]) as GameObject;
        explotion.name = "explotion";
        explotion.transform.parent = transform.parent;
        explotion.transform.position = new Vector3(target.transform.position.x, target.transform.position.z + 0.5f, 0);
        explotion.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) + 5;
        //print (transform.position + " | " + explotion.transform.position);
    }
}

using UnityEngine;
using System.Collections;

public class Buff : Effect {

    public GameObject owner;
    public string ownerName;
    public GameObject carrier;
    public string carrierName;

    public float elapsedTime;
    public float maxDur;
    public double netTime;
    float lifeTime;
    public float lifeSpan;

    public struct States
    {
        public Vector3 lastPos;
        public float lastLifetime;
        public string lastCarrier;
    }

    public States _states
    {
        get
        {
            States temp = new States();
            temp.lastPos = transform.position;
            temp.lastLifetime = lifeTime;
            temp.lastCarrier = carrierName;
            return temp;
        }
    }

	public void Awake()
	{
		carrier = gameObject;
		carrierName = carrier.name;
		effectName = "genericbuff";
		netTime = TimeManager.Instance.NetworkTime;
		elapsedTime = 0;
	}

	// Use this for initialization
	public void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        DoEffect();
        if (elapsedTime >= maxDur)
        {
            OnExpire();
        }
    }

    public virtual void DoEffect()
    {
        // do something
    }

    public virtual void OnExpire()
    {
        MarkToDestroy(_states);
    }

    public void MarkToDestroy(States states)
    {
        //add stuff
        print(ownerName +
            "'s buff, '" +
            effectName +
            "' died at pos " +
            states.lastPos +
            " after duration of " +
            states.lastLifetime +
            " while affecting " +
            states.lastCarrier +
            ".");
        Destroy(this);
    }
}

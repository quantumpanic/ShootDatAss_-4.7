using UnityEngine;
using System.Collections;

public class NailDebuff : Buff {

    public bool hasPierce;
    public bool isDelay;

    public NailDebuff(GameObject caster)
	{
		owner = caster;
		ownerName = caster.name;
	}

    public NailDebuff CreateComponent(GameObject go)
    {
        NailDebuff init = go.AddComponent<NailDebuff>();
		init.owner = owner;
		init.ownerName = ownerName;
        return init;
    }

	public void Awake()
	{
		base.Awake();
		effectName = "nailpierce";
		isDelay = false;
		maxDur = 3;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void DoEffect()
    {
        if (!isDelay) hasPierce = true;
        else hasPierce = false;
    }

    public override void OnExpire()
    {
        if (!isDelay)
        {
            effectName = "nailpierce_delay";
			isDelay = true;
			elapsedTime = 0;
            maxDur = 10;
        }
        else base.OnExpire();
    }
}

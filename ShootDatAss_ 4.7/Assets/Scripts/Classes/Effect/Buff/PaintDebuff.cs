using UnityEngine;
using System.Collections;

public class PaintDebuff : Buff
{
    public bool hasSlow;
    public bool isDelay;

    public PaintDebuff(GameObject caster)
	{
		owner = caster;
		ownerName = caster.name;
	}

    public PaintDebuff CreateComponent(GameObject go)
    {
        PaintDebuff init = go.AddComponent<PaintDebuff>();
        return init;
    }

	// Use this for initialization
	void Start () {
        base.Start();
        effectName = "paintslow";
        isDelay = false;
        maxDur = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void DoEffect()
    {
		if (!isDelay) hasSlow = true;
		else hasSlow = false;
    }

    public override void OnExpire()
    {
        if (!isDelay)
        {
            effectName = "paintslow_delay";
            isDelay = true;
			elapsedTime = 0;
            maxDur = 10;
        }
        else base.OnExpire();
    }
}

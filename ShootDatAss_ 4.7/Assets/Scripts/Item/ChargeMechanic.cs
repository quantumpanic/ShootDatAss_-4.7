using UnityEngine;
using System.Collections;

public class GrenadeCharger : ChargeMechanic
{
    void Start()
    {
        gaugeMax = 70;
        buildupAmt = 1;
        secsPerTick = .01f;
    }
}
public class JumpCharger : ChargeMechanic
{
    void Start()
    {
        gaugeFill = 35;
        gaugeMax = 70;
        buildupAmt = 1;
        secsPerTick = .01f;
    }
}

public class ChargeMechanic : MonoBehaviour {

	public float gaugeMax;
	public float gaugeFill;
	public float buildupAmt;
	public float decayAmt;
	public int linger;
	public int lingerTicks; //gauge stays still if not charging (per tick)
    public float secsPerTick;
	public bool isCharging = false;

	float timer;
	int ticks;
	public bool _tick
	{
		get
		{
			timer += Time.deltaTime;
            while (timer >= secsPerTick)
            {
                timer -= secsPerTick;
                ticks++;
                if (secsPerTick <= 0) return true;
                if (timer <= secsPerTick)
				{
					return true;
				}
			}
			return false;
		}
		set
		{
		}
	}

	void Start(){
	}

	void Update () {
		if (_tick) // check if a tick occurred
		{
			while (ticks > 0) // ticks that occured since last frame
			{
				ticks--;
				if (isCharging)
				{
                    lingerTicks = 0;
					gaugeFill += buildupAmt;
					if (gaugeFill > gaugeMax)gaugeFill = gaugeMax;
				}
				else
				{
					if (lingerTicks < linger)
					{
						lingerTicks++;
					}
					else
					{
						//lingerTicks = 0;
						gaugeFill -= decayAmt;
						if (gaugeFill < 0)gaugeFill = 0;
					}
				}
			}
		}
	}
}

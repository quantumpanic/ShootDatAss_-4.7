using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShockDebuff : Buff {
    public DamageModifier dmg;
	public List<DamageSource> dmgsrc;
	public bool isDelay;
	public AudioSource audio;
    public float alpha;
    public float alphaTo;
    GameObject visual;
    SpriteRenderer renderer;

    public ShockDebuff(GameObject caster)
	{
		owner = caster;
		ownerName = caster.name;
	}

    public ShockDebuff CreateComponent(GameObject go)
    {
        ShockDebuff init = go.AddComponent<ShockDebuff>();
		init.owner = owner;
		init.ownerName = ownerName;
        return init;
    }

	public void Awake()
	{
		base.Awake();
		effectName = "pulseshock";
		dmg = gameObject.GetComponent<DamageModifier> ();
		if (dmg) dmgsrc = dmg.baseSources;
		isDelay = false;
		maxDur = 2.5f;
	}

	// Use this for initialization
	void Start () {
        audio = SoundManager.instance.PlaySfx("sfx_pulsebuff", carrier, true);

        visual = new GameObject("zap");
        visual.transform.parent = transform;
        visual.transform.localScale = Vector2.one * 2;
        renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = ObjectLibrary.instance.bullets[5];
        alphaTo = .8f;
        Flicker();
	}

    // Update is called once per frame
    void Update()
    {
    }

    void Flicker()
    {
        float flickerRate = 0.05f;
        StartCoroutine(FlickerCycle(flickerRate));
    }

    IEnumerator FlickerCycle(float rate)
    {
        if (visual)
        {
            visual.transform.position = MatchManager.IsometricScaling(carrier.transform.position, 0.5f);
            renderer.sortingOrder = (int)(-transform.position.z * 10) + 10;
            if (alpha == alphaTo) alpha = 0;
            else alpha = alphaTo;
            renderer.color = new Color(1, 1, 1, alpha);

            yield return new WaitForSeconds(rate);
            Flicker();
        }
    }

    public override void DoEffect()
	{
        if (!isDelay)
		{
			if (dmgsrc == null) return;
			for (int s = 0; s <= dmgsrc.Count-1; s++)
			{
				dmgsrc[s].active = false;
			}
		}
    }

    public override void OnExpire()
	{
        if (!isDelay)
		{
			if (dmgsrc != null)
			{
				for (int s = 0; s <= dmgsrc.Count-1; s++)
				{
					dmgsrc[s].active = true;
				}
			}
            Destroy(audio.gameObject);
            Destroy(visual);
            effectName = "pulseshock_delay";
            isDelay = true;
			elapsedTime = 0;
            maxDur = 10;
		}
        else base.OnExpire();
    }
}

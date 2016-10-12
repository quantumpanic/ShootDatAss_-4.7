using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum Weapon { gun, nailGun, paintBall, pulseLaser, shotgun, musket, parley, AK47, SMG }
public class WeaponStats
{
    public enum Type { physical = 0, shell = 1, explosion = 2, energy = 3 }

    float[] __baseDmg = new float[9] {2, 2, 4, 2, 2, 2, 2, 6, 2 };
    int[] __dmgType = new int[9] {1, 1, 0, 3, 1, 1, 1, 1, 1 };
    float[] __range = new float[9] {1, 2, 1, 3, 1, 3, 2, 1, 1 };
    float[] __radius = new float[9] {.2f, .2f, .2f, .2f, .2f, .2f, .2f, .2f, .2f };
    float[] __speed = new float[9] { 12.5f, 12.5f, 8, 20, 12.5f, 10f, 10f, 12.5f, 12.5f };
    float[] __fireRate = new float[9] {1, 2, 1, 2, 1, 1, 1, 2, 3 };

    public string weaponName;
    public float baseDmg;
    public int dmgType;
    public float range;
    public float radius;
    public float speed;
    public float fireRate;

    public WeaponStats(Weapon weap)
    {
        weaponName = weap.ToString();
        baseDmg = __baseDmg[(int)weap];
        dmgType = __dmgType[(int)weap];
        range = __range[(int)weap];
        radius = __radius[(int)weap];
        speed = __speed[(int)weap];
        fireRate = __fireRate[(int)weap];
    }
}

public enum AltWeapon { grenade, timeBomb, smokeBomb, megaJump, rocketLauncher, robot, knife, sniper }
public class AltWeaponStats
{
    public enum Type { physical = 0, shell = 1, explosion = 2, energy = 3 }

    float[] __baseDmg = new float[8] { 8, 12, 0, 0, 6, 8, 8, 10 };
    int[] __dmgType = new int[8] { 2, 2, 0, 0, 2, 3, 0, 1 };
    float[] __range = new float[8] { 5, 0, 5, 5, 5, 6, 2, 10 };
    float[] __radius = new float[8] { .3f, .3f, .3f, 1, .3f, .2f, 1, 0 }; // size of collider
    float[] __speed = new float[8] { 15f, 0, 0, 12.5f, 15f, 20f, 12.5f, 0 };
    float[] __fireRate = new float[8] { 0, 0, 0, 0, 3, 3, 4, 0 };
    float[] __effectRadius = new float[8] { 2, 3, 2, .5f, 2, 0, .5f, 0 };

    public string weaponName;
    public float baseDmg;
    public int dmgType;
    public float range;
    public float radius;
    public float speed;
    public float fireRate;
    public float effectRadius;

    public AltWeaponStats(AltWeapon weap)
    {
        weaponName = weap.ToString();
        baseDmg = __baseDmg[(int)weap];
        dmgType = __dmgType[(int)weap];
        range = __range[(int)weap];
        radius = __radius[(int)weap];
        speed = __speed[(int)weap];
        fireRate = __fireRate[(int)weap];
        effectRadius = __effectRadius[(int)weap];
    }
}

public class DamageStats : WeaponStats
{
	public DamageStats (Weapon weap): base(weap) {}
}

/// <summary> [b = Base, r = Raw, a = Absolute. m = Multiplier, a = Adder].  
/// Before damage is calculated, all the modifiers are set from the Character script.
/// (kept all in one place because creating new Lists in every projectile may be too heavy)
/// </summary>
public class DamageSource
{
	public bool active = true;
	public float baseAmt;
	public float multiplier;
    public float _modifiedAmt
    {
        get
        {
            return baseAmt * multiplier;
        }
    }
	public float unmodifiedAmt;
    public float _totalAmt
    {
        get
        {
            return _modifiedAmt + unmodifiedAmt;
        }
    }

    public DamageSource(float ba, float mo = 0f, float um = 0f)
    {
        baseAmt = ba;
        multiplier = mo;
        unmodifiedAmt = um;
    }
}

public class DamageModifier : MonoBehaviour {
    public GameObject owner;
	public string ownerName;
	public GameObject[] receivers;
	public List<DamageSource> baseSources;
	public List<DamageSource> modifierSources;
	public List<DamageSource> finalModifiers;
	
	public float finalDmg;

    public DamageModifier()
    {
        owner = gameObject;
        ownerName = owner.name;
    }

	/// <summary>
	/// Calculates before returning. to get current, use 'finalDmg'
	/// </summary>
	public float _finalDmg
	{
		get
		{
			CalculateDamage();
			return finalDmg;
		}
	}

	public float baseDmg; // the main factor. usually what is stated in the weapon tooltip e.g. (5 base dmg)
    float baseMultiplier = 1f; // multiplicative factor specifically for base damage e.g. (+50% dmg to zombies)
    float baseAdder = 0; // extra points that also get multiplied along with base damage. usually from stats
    float rawMultiplier = 1f; // raw multiplier. percentage effects such as (+66% damage). 
    float rawAdder = 0; // raw points that don't get multiplied. usually from equipment like (+10 dmg) 
	float absMultiplier = 1f; // the main raw multiplier factor. different multipliers stack additively
	float absAdder = 0; // bonus flat damage on top of everything else. cases such as (adds 25 fire dmg per hit)


	public DamageSource NewBaseSource(float amt = 0, float mod = 0, float add = 0)
	{
		DamageSource baseSrc = new DamageSource(amt, mod, add);
		baseSources.Add (baseSrc);
		return baseSrc;
	}

	public DamageSource NewBaseModifier(float amt = 0, float mod = 0, float add = 0)
    {
		DamageSource modSrc = new DamageSource(amt, mod, add);
		modifierSources.Add (modSrc);
		return modSrc;
    }

	public DamageSource NewFinalModifier(float amt = 0, float mod = 0, float add = 0)
    {
		DamageSource finMod = new DamageSource(amt, mod, add);
		finalModifiers.Add (finMod);
		return finMod;
    }

	protected void CalculateDamage()
	{
        //float totalBaseDmg = (baseDmg*baseMultiplier) + baseAdder;
		float rawDmg = (baseSources.Sum(src => (src.active)? src._totalAmt:0 ));
		float rawMultipliers = modifierSources.Sum(src => (src.active)? src.multiplier :0);
		float rawAdders = modifierSources.Sum(src => (src.active)? src.unmodifiedAmt :0);
		float enhancedDmg = (rawDmg * (rawMultipliers+1)) + rawAdders;
		finalDmg = (enhancedDmg * (finalModifiers.Sum(src => (src.active)? src.multiplier:0)+1))
			+ finalModifiers.Sum(src => (src.active)? src.unmodifiedAmt:0);
	}

	/// <summary>
	/// Standalone modifier to manually get/set final damage. for things such as Critical
	/// [fm = Final Multiplier, fa = Final Adder, set = whether to change the original value]
	/// </summary>

	public float ModifiedFinalDamage(float fm = 0f, float fa = 0f, bool set = false)
	{
		float newDmg = (_finalDmg * fm) + fa;
        if (set) finalDmg = newDmg;
        return finalDmg;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

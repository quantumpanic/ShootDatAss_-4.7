using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sniper : ItemFunction
{
    public CharacterController characterController;
    public List<GameObject> target;
    public List<GameObject> shootTargets;
    public float zoom = 5;
    public float zoomTarget = 5;

    public bool isSniping;
    public bool expired;

    public int id;

    void Start()
    {
        shootTargets = new List<GameObject>();
        target = new List<GameObject>();
        characterController = gameObject.GetComponent<CharacterController>();
        expired = false;
    }

    void Update()
    {
        AddTargets();
        SniperState();
        if (isSniping) Crosshairs();
        ZoomCycle();
    }

    void AddTargets()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<SphereCollider>() && player != gameObject)
            {
                if (Vector3.Distance(transform.position, player.transform.position) < 10)
                {
                    if (!target.Contains(player)) target.Add(player);
                }
            }
        }
    }

    void Crosshairs()
    {
        {
            for (int g = target.Count - 1; g >= 0; g--)
            {
                if (Vector3.Distance(transform.position, target[g].transform.position) > 10)
                {
                    if (target.Contains(target[g]))
                    {
                        if (target[g].transform.FindChild("target"))
                        {
                            shootTargets.Remove(target[g].transform.FindChild("target").gameObject);
                            Destroy(target[g].transform.FindChild("target").gameObject);
                        }
                        target.Remove(target[g]);
                    }
                }
                else
                {
                    if (!target[g].transform.FindChild("target"))
                    {
                        GameObject shootTarget = Instantiate(ObjectLibrary.instance.shootTarget) as GameObject;
                        shootTarget.name = "target";
                        shootTarget.AddComponent<SniperTarget>().parentSniper = this;
                        shootTarget.transform.parent = target[g].transform;
                        shootTarget.transform.position = new Vector3(transform.position.x,
                                                                     transform.position.z,
                                                                     0);
                        shootTargets.Add(shootTarget);
                    }
                }
            }
            foreach (GameObject go in shootTargets)
            {
                go.transform.position = new Vector3(
                    go.transform.parent.position.x,
                    (go.transform.parent.position.z + 0.5f) * MatchManager.instance.scr2mapRatio,
                    0);
            }
        }
    }

    void ZoomCycle()
    {
        if (isSniping && !expired)
        {
            characterController.isSniping = true;
            characterController.player.GetComponent<CharacterAnimationController>().gun = "Sniper";
            zoomTarget = 8;
            GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = zoom;
            GameObject.Find("indicatorCamera").GetComponent<Camera>().orthographicSize = zoom;
        }
        else
        {
            characterController.isSniping = false;
            characterController.player.GetComponent<CharacterAnimationController>().gun = "DefaultGun";
            zoomTarget = 5;
            GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = zoom;
			GameObject.Find("indicatorCamera").GetComponent<Camera>().orthographicSize = zoom;
			CheckExpired();
        }

        zoom = Mathf.Lerp(zoom, zoomTarget, 10 * Time.deltaTime);
		if (Mathf.Approximately(zoom,zoomTarget)) zoom = zoomTarget;
    }

    void CheckExpired()
    {
		if (zoom <= 5)
            if (expired)
            {
				Destroy(this);
            }
    }

    void RemoveSniper()
    {
        characterController.itemHold[id] = "";
        if (characterController.itemReady.Contains("sniper")) characterController.itemReady.Remove("sniper");
        foreach (GameObject crosshair in shootTargets) Destroy(crosshair);
        target.Clear();
		shootTargets.Clear();
		expired = true;
		isSniping = false;
    }

    void SniperState()
    {
        if (target.ToArray().Length > 0)
        {
            characterController.SetReady("sniper");
            ready = true;
        }
        else
        {
            isSniping = false;
            if (characterController.itemReady.Contains("sniper")) characterController.itemReady.Remove("sniper");
            ready = false;
        }
    }

    public void SnipePlayer(string targetName)
    {
        float totalDmg = new AltWeaponStats(AltWeapon.sniper).baseDmg + characterController.power;
        SoundManager.instance.PlaySfx("sfx_sniper");
        MatchManager.instance.SendHitPlayer(characterController.name, targetName, totalDmg);
		RemoveSniper ();
        discarded = true;
    }

    public void Sniping()
    {
        if (!expired) isSniping = true;
    }
}

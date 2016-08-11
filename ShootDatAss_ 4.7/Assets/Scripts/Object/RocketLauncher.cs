using UnityEngine;
using System.Collections;

public class RocketLauncher : ItemFunction {

	public CharacterController characterController;
	private float shootTime = 1;
	public float rocketTime;
    public float dur = 5;

    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        characterController.isShoot = false;
        if (rocketTime < dur)
        {
            characterController.player.GetComponent<CharacterAnimationController>().gun = "RocketLauncher";
            characterController.isRocketLauncher = true;
            Shoot();
        }
        else
        {
            characterController.player.GetComponent<CharacterAnimationController>().gun = "DefaultGun";
            characterController.isRocketLauncher = false;
            Destroy(this);
        }
        rocketTime += Time.deltaTime;
    }

    void Shoot()
    {
        if (shootTime > (0.4f))
        {
            GameObject rktGo = new GameObject("rocket");
            Rocket001 rocket = Rocket001.CreateComponent(rktGo);
            rocket.owner = gameObject;

            shootTime = 0;
        }
        shootTime += Time.deltaTime;
    }

    void ShootOld()
    {
        GameObject bullet = null;

        string direction = characterController.player.GetComponent<CharacterAnimationController>().faceDirection;
        if (direction.Contains("Idle")) direction = direction.Replace("Idle", "");

        if (shootTime > (0.4f))
        {
            bullet = ObjectLibrary.instance.GetBullet(-1, 3, direction,
                                                      characterController.GetVelocity(direction),
                                                      characterController.playerName,
                                                      6);
            bullet.transform.parent = characterController.player.transform;
            bullet.transform.localPosition = characterController.player.GetComponent<CharacterAnimationController>().GetShootPoint();
            bullet.transform.parent = transform.parent.transform;
            bullet.AddComponent<TrailRenderer>();

            GameObject trail = Instantiate(ObjectLibrary.instance.trail) as GameObject;
            trail.name = "Trail";
            trail.transform.parent = bullet.transform;
            trail.transform.localPosition = new Vector3(0, 0, 0);

            shootTime = 0;
        }

        shootTime += Time.deltaTime;
    }

}

package dk.fullcontrol.fps.simulation;

import java.util.ArrayList;
import java.util.List;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;

// Player class representing an individual soldier in the world simulation
public class CombatPlayer {

	public static final int maxHealth = 100; // Maximum amount of health for a player
	public static final int maxAmmoReserve = 50; // Maximum amount of ammo that can be held in reserve inventory
	private static final int defaultHitHP = 30; // Amount of damage done per shot that hits

	private User sfsUser; // SFS user that corresponds to this player

	private Weapon weapon; // Weapon of this player
	
	private boolean isShooting;
	private float shootingBuffer; // give 1 sec delay before isShooting = false
	
	private float shootRate;
	private float shootTime;
	
	private boolean isStunned;
	private boolean isDisarmed;
	private boolean isSnared;
	private boolean isSilenced;
	private boolean isSlowed;

	public List<Projectile> bullets = new ArrayList<Projectile>(); // Bullets owned by this player

	private Transform transform; // Transform of the player that is synchronized with clients

	private Collider collider; // Collider - determines the size of the player for shooting calculations

	private int health = 100;
	private int score = 0;

	private int ammoReserve = maxAmmoReserve - Weapon.maxAmmo; // Load the weapon and deduct ammo from reserve

	public boolean canShoot()
	{
		return shootTime > shootRate;
	}
	
	public void orderShoot()
	{
		isShooting = true;
		shootingBuffer = 0;
	}

	public boolean isDead() {
		return health <= 0;
	}

	public void removeHealth(int count) {
		health -= count;
	}

	public void hit() {
		removeHealth(defaultHitHP);
	}

	public User getSfsUser() {
		return sfsUser;
	}

	public Transform getTransform() {
		return transform;
	}
	
	public Projectile getBullet(int id)
	{
		for (Projectile bullet : bullets)
		{
			if (bullet.id == id) 
				return bullet;
		}
		return null; // bullet not found
	}

	public CombatPlayer(User sfsUser, ISFSObject trans) {
		Transform trns = fromSFSObject(trans);
		
		this.sfsUser = sfsUser;
		this.weapon = new Weapon();
		this.transform = trns;
		//this.transform = Transform.random();
		this.collider = new Collider(0, 1, 0, 0.5, 2);
	}

	public void toSFSObject(ISFSObject data) {
		ISFSObject playerData = new SFSObject();

		playerData.putUtfString("name", sfsUser.getName());
		playerData.putInt("id", sfsUser.getId());
		playerData.putInt("score", this.score);

		transform.toSFSObject(playerData);
		data.putSFSObject("player", playerData);
	}

	public Transform fromSFSObject(ISFSObject data) {
		ISFSObject transformData = data.getSFSObject("transform");

		double x = transformData.getDouble("x");
		double y = transformData.getDouble("y");
		double z = transformData.getDouble("z");

		double rx = transformData.getDouble("rx");
		double ry = transformData.getDouble("ry");
		double rz = transformData.getDouble("rz");

		long timeStamp = transformData.getLong("t");

		Transform transform = new Transform(x, y, z, rx, ry, rz);
		transform.setTimeStamp(timeStamp);
		return transform;
	}

	public Collider getCollider() {
		return collider;
	}

	public double getX() {
		return this.collider.getCenterx() + this.transform.getX();
	}

	public double getY() {
		return this.collider.getCentery() + this.transform.getY();
	}

	public double getZ() {
		return this.collider.getCenterz() + this.transform.getZ();
	}

	public int getHealth() {
		return health;
	}

	// Restore player stats when he's respawning
	public void resurrect() {
		health = maxHealth;
		weapon.resetAmmo();
		ammoReserve = maxAmmoReserve - Weapon.maxAmmo;
		this.transform = Transform.random();
	}

	public void addKillToScore() {
		this.score++;
	}

	public Weapon getWeapon() {
		return weapon;
	}

	// Reload the weapon
	public void reload() {
		if (ammoReserve == 0) {
			return;
		}
		if (weapon.isFullyLoaded()) {
			return;
		}

		int ammoUsedInReload = weapon.reload(ammoReserve);
		ammoReserve -= ammoUsedInReload;
	}

	public int getAmmoReserve() {
		return ammoReserve;
	}

	// Add more ammo (when player gets ammo item)
	public void addAmmoToReserve(int i) {
		this.ammoReserve += i;
		if (this.ammoReserve > maxAmmoReserve) {
			this.ammoReserve = maxAmmoReserve;
		}
	}

	// Add more health (when player gets health item)
	public void addHealth(int i) {
		this.health += i;
		if (this.health > maxHealth) {
			this.health = maxHealth;
		}
	}

	public boolean hasMaxAmmoInReserve() {
		return ammoReserve == maxAmmoReserve;
	}

	public boolean hasMaxHealth() {
		return health == maxHealth;
	}

	public int getScore() {
		return score;
	}
}

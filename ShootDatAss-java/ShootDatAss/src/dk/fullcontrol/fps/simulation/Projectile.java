package dk.fullcontrol.fps.simulation;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;

// Player class representing an individual soldier in the world simulation
public class Projectile {

	private User sfsUser; // SFS user that corresponds to this player

	public int id; // unique bullet ID

	private CombatPlayer owner; // SFS user that corresponds to this player

	private Weapon weapon; // Weapon of this player

	private Transform transform; // Transform of the player that is synchronized with clients

	private Collider collider; // Collider - determines the size of the player for shooting calculations
	
	private double touchDistance;
	private float speed;
	
	private int health = 100;

	public boolean isDead() {
		return health <= 0;
	}

	public User getSfsUser() {
		return sfsUser;
	}

	public CombatPlayer getOwner() {
		return owner;
	}

	public Transform getTransform() {
		return transform;
	}
	
	public double getTouchDistance()
	{
		return touchDistance;
	}
	
	public void MoveForward()
	{
		double x = this.transform.getX();
		double y = this.transform.getY();
		double z = this.transform.getY();
		double rotX = this.transform.getRotx();
		double rotZ = this.transform.getRotz();
		
	    x += speed * Math.sin(rotX);
	    z += speed * Math.cos(rotZ);
	    
	    Transform step = new Transform(x,y,z,0,0,0);
	    transform.load(step);
	}

	public Projectile(CombatPlayer owner, int id, ISFSObject trans) {
		Transform trns = fromSFSObject(trans);
		
		//this.sfsUser = sfsUser;
		this.owner = owner;
		this.id = id;
		this.transform = trns;
		
		this.weapon = new Weapon();
		this.collider = new Collider(0, 1, 0, 0.5, 2);
	}

	public boolean isClose(Transform newTransform) {
		double d = newTransform.distanceTo(this.transform);
		return (d <= touchDistance);
	}

	public void toSFSObject(ISFSObject data) {
		ISFSObject playerData = new SFSObject();

		playerData.putUtfString("name", sfsUser.getName());
		playerData.putInt("id", sfsUser.getId());
		//playerData.putInt("score", this.score);

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

	public Weapon getWeapon() {
		return weapon;
	}
}

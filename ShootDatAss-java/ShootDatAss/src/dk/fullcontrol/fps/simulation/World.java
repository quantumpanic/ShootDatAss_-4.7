package dk.fullcontrol.fps.simulation;

import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;

import com.gankers.shootdatass.ShootDatAss;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Random;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;


// The main World server model. Contains players, items, and all the other needed world objects
public class World {
	private static final int maxSpawnedItems = 6; // Maximum items that can be spawned at once

	// World bounds - to create random transforms
	public static final double minX = -25;
	public static final double maxX = 25;
	public static final double minZ = -25;
	public static final double maxZ = 25;

	private static final int maxItemsOfSingleType = 3; // Maximum items of the particular type that can be present on the scene

	private static Random rnd = new Random();

	private int itemId = 0; // Item id counter - to generate unique ids
	private int bulletId = 0; // Item id counter - to generate unique ids
	private int crateId = 0; // Item id counter - to generate unique ids

	private ShootDatAss extension; // Reference to the server extension

	// Players
	private List<CombatPlayer> players = new ArrayList<CombatPlayer>();

	// Projectiles
	private List<Projectile> bullets = new ArrayList<Projectile>();

	// Items
	private List<Item> items = new ArrayList<Item>();
	
	// Items
	private List<Crate> crates = new ArrayList<Crate>();
	
	private class WorldRunner implements Runnable
    {
        public void run()
        {
        	World.this.WorldCycle();
        }
    }
	ScheduledFuture<?> worldScheduler;
	
	public World(ShootDatAss extension) {
		this.extension = extension;
		rnd.setSeed((new Date()).getTime());
		
		SmartFoxServer sfs = SmartFoxServer.getInstance();
		worldScheduler = sfs.getTaskScheduler().scheduleAtFixedRate(new WorldRunner(), 200, 200, TimeUnit.MILLISECONDS);
	}
	
	public void WorldCycle()
	{
		for (CombatPlayer pl : players)
		{
			// check collision
			// update stats
			// 
		}

		for (Projectile pr : bullets)
		{
			// move projectiles
			// check collision
			// check expiration
			pr.MoveForward();
		}

		for (Item i : items)
		{
			// check expiration
			//
		}
	}

	public List<CombatPlayer> getPlayers() {
		return players;
	}

	// Spawning new items
	public void spawnBullet(User user, ISFSObject data) {
		CombatPlayer player = getPlayer(user);
		Projectile item = new Projectile(player, bulletId++, data);
		bullets.add(item);
		extension.clientInstantiateBullet(item);
		extension.trace("Spawned bullet at: ");
	}

	// Spawning new items
	public void spawnItems() {
		int itemsCount = rnd.nextInt(maxSpawnedItems);

		int healthItemsCount = itemsCount / 2;
		int hc = 0;
		extension.trace("Spawn " + itemsCount + " items.");

		for (int i = 0; i < itemsCount; i++) {
			ItemType itemType = (hc++ < healthItemsCount) ? ItemType.HealthPack : ItemType.Ammo;
			if (hasMaxItems(itemType)) {
				continue;
			}

			Item item = new Item(itemId++, Transform.randomWorld(), itemType);
			items.add(item);
			extension.clientInstantiateItem(item);
		}
	}
	
	// initialize the new crates on the map
	public void initCrates()
	{
		int itemsCount = rnd.nextInt(maxSpawnedItems);

		int healthItemsCount = itemsCount / 2;
		int hc = 0;
		
		// ^ keep this to distribute evenly weapons and powerups to players
		// also good for rare weapon or the like

		for (int i = 0; i < itemsCount; i++) {
			ItemType itemType = (hc++ < healthItemsCount) ? ItemType.HealthPack : ItemType.Ammo;
			if (hasMaxItems(itemType)) {
				continue;
			}

			Item item = new Item(itemId++, Transform.randomWorld(), itemType);
			items.add(item);
			extension.clientInstantiateItem(item);
		}
	}

	// Respawning all crates on the map
	public void spawnCrates() {
		for (int i = 0; i < crates.size(); i++) {
			// try to spawn each crate
			// conditions for crate to spawn:
			// crate not broken
			// item from broken crate not picked up
			// cannot be a player within some screen distance nearby
			
			
		}
	}

	private boolean hasMaxItems(ItemType itemType) {
		int counter = 0;

		for (Item item : items) {
			if (item.getItemType() == itemType) {
				counter++;
			}
		}

		return counter > maxItemsOfSingleType;
	}

	// Add new player if he doesn't exist, or resurrect him if he already added
	public boolean addOrRespawnPlayer(User user, ISFSObject data) {
		CombatPlayer player = getPlayer(user);

		if (player == null) {
			
			player = new CombatPlayer(user, data);
			players.add(player);
			extension.clientInstantiatePlayer(player);
			return true;
		}
		else {
			player.resurrect();
			extension.clientInstantiatePlayer(player);
			return true;
		}
	}

	// Trying to move player. If the new transform is not valid, returns null
	public Transform movePlayer(User u, Transform newTransform) {
		CombatPlayer player = getPlayer(u);

		if (player.isDead()) {
			return player.getTransform();
		}

		if (isValidNewTransform(player, newTransform)) {
			player.getTransform().load(newTransform);

			checkItem(player, newTransform);

			return newTransform;
		}

		return null;
	}

	// Check the player intersection with item - to pick it up
	private void checkItem(CombatPlayer player, Transform newTransform) {
		for (Object itemObj : items.toArray()) {
			Item item = (Item) itemObj;
			if (item.isClose(newTransform)) {
				try {
					useItem(player, item);
				}
				catch (Throwable e) {
					extension.trace("Exception using item " + e.getMessage());
				}
				return;
			}
		}
	}

	// Applying the item effect and removing the item from World
	private void useItem(CombatPlayer player, Item item) {
		if (item.getItemType() == ItemType.Ammo) {
			if (player.hasMaxAmmoInReserve()) {
				return;
			}

			player.addAmmoToReserve(20);
			extension.clientUpdateAmmo(player);
		}
		else if (item.getItemType() == ItemType.HealthPack) {
			if (player.hasMaxHealth()) {
				return;
			}

			player.addHealth(CombatPlayer.maxHealth / 3);
			extension.clientUpdateHealth(player);
		}

		extension.clientRemoveItem(item, player);
		items.remove(item);
	}

	public Transform getTransform(User u) {
		CombatPlayer player = getPlayer(u);
		return player.getTransform();
	}

	private boolean isValidNewTransform(CombatPlayer player,
	                                    Transform newTransform) {

		// Check if the given transform is valid in terms of collisions, speed hacks etc
		// In this example, the server will always accept a new transform from the client

		return true;
	}

	// Gets the player corresponding to the specified SFS user
	private CombatPlayer getPlayer(User u) {
		for (CombatPlayer player : players) {
			if (player.getSfsUser().getId() == u.getId()) {
				return player;
			}
		}

		return null;
	}

	// Trying to move player. If the new transform is not valid, returns null
	public Transform moveBullet(User u, Transform newTransform) {
		CombatPlayer player = getPlayer(u);

		if (player.isDead()) {
			return player.getTransform();
		}

		if (isValidNewTransform(player, newTransform)) {
			player.getTransform().load(newTransform);

			checkItem(player, newTransform);

			return newTransform;
		}

		return null;
	}

	// Process the shot from client
	public void processShot(User shooter, String id, int rotation) {
		// here, make the remote player face direction and try fire weapon
		// on player, keep trying to shoot as requests from local player comes in at intervals
		CombatPlayer player = getPlayer(shooter);
		if (player.isDead()) {
			return;
		}
		player.orderShoot();
		
		// check if fired from valid position
		// check if state is valid to fire (shoot rate, disarmed, etc)

		player.getWeapon().shoot();

		extension.clientUpdateAmmo(player);
		extension.clientEnemyShotFired(player);
	}

	// Client reports a bullet hitting another player
	public void bulletCollide(User hitter, int bulletId, int hit) {
		Projectile bullet = null;
		CombatPlayer target = null;
		
		for (CombatPlayer player: players)
		{
			if (player.getSfsUser() == hitter) // finds the shooter
			{
				bullet = player.getBullet(bulletId); // finds the shooter's bullet
			}
			else if (player.getSfsUser().getId() == hit) // finds the target
			{
				target = player;
			}
		}
		
		if (bullet == null) return; // bullet not found
		if (target == null) return; // target not found

		boolean res = checkHit(target, bullet);
		if (res)
		{
			// check what happens with the bullet here
			playerHit(target,getPlayer(hitter));
		}
	}

	// Client reports a bullet hitting a crate
	public void bulletCollide(int bulletId, int crateId) {
		Projectile bullet = null;
		Crate crate = null;
		
		for (Projectile b: bullets)
		{
			if (b.id == bulletId)
			{
				bullet = b;
			}
		}
		
		for (Crate c: crates)
		{
			if (c.id == crateId)
			{
				crate = c;
			}
		}
		
		if (bullet == null) return; // bullet not found
		if (crate == null) return; // target not found

		boolean res = checkHit(crate, bullet);
		if (res)
		{
			// destroy the crate, put it on cooldown, and spawn an item
			
		}
	}
	
	private boolean checkHit(CombatPlayer player, Projectile bullet) {
		Transform playerTrns = player.getTransform();
		
		return bullet.isClose(playerTrns);
	}
	
	private boolean checkHit(Crate crate, Projectile bullet) {
		Transform playerTrns = crate.getTransform();
		
		return bullet.isClose(playerTrns);
	}

	// Performing reload
	public void processReload(User fromUser) {
		CombatPlayer player = getPlayer(fromUser);
		if (player.isDead()) {
			return;
		}
		if (player.getAmmoReserve() == 0) {
			return;
		}
		if (!player.getWeapon().canReload()) {
			return;
		}

		player.reload();
		extension.clientReloaded(player);
		extension.clientUpdateAmmo(player);
	}

	private double normAngle(double a) {
		if (a >= 360) {
			return a - 360;
		}
		return a;
	}

	// Applying the hit to the player.
	// Processing the health and death
	private void playerHit(CombatPlayer fromPlayer, CombatPlayer pl) {
		pl.removeHealth(20);

		if (pl.isDead()) {
			fromPlayer.addKillToScore();  // Adding frag to the player if he killed the enemy
			extension.updatePlayerScore(fromPlayer);
			extension.clientKillPlayer(pl,fromPlayer);
		}
		else {
			// Updating the health of the hit enemy
			extension.clientUpdateHealth(pl);
		}
	}

	// When user lefts the room or disconnects - removing him from the players list 
	public void userLeft(User user) {
		CombatPlayer player = this.getPlayer(user);
		if (player == null) {
			return;
		}
		players.remove(player);
	}

}

package com.gankers.shootdatass;

import java.util.List;

import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.*;
import java.util.concurrent.ScheduledFuture;

import dk.fullcontrol.fps.handlers.BulletHitHandler;
import dk.fullcontrol.fps.handlers.GetTimeHandler;
import dk.fullcontrol.fps.handlers.OnUserGoneHandler;
import dk.fullcontrol.fps.handlers.ReloadHandler;
import dk.fullcontrol.fps.handlers.SendAnimHandler;
import dk.fullcontrol.fps.handlers.SendTransformHandler;
import dk.fullcontrol.fps.handlers.ShotHandler;
import dk.fullcontrol.fps.handlers.SpawnMeHandler;
import dk.fullcontrol.fps.simulation.CombatPlayer;
import dk.fullcontrol.fps.simulation.Item;
import dk.fullcontrol.fps.simulation.Projectile;
import dk.fullcontrol.fps.simulation.World;
import dk.fullcontrol.fps.utils.RoomHelper;
import dk.fullcontrol.fps.utils.UserHelper;


public class ShootDatAss extends SFSExtension{
	private World world; // Reference to World simulation model

	public World getWorld() {
		return world;
	}

	@Override
	public void init() {
		world = new World(this);  // Creating the world model

		// Subscribing the request handlers
		addRequestHandler("sendTransform", SendTransformHandler.class);
		addRequestHandler("sendAnim", SendAnimHandler.class);
		addRequestHandler("spawnMe", SpawnMeHandler.class);
		addRequestHandler("getTime", GetTimeHandler.class);
		addRequestHandler("shot", ShotHandler.class);
		addRequestHandler("bulletHit", BulletHitHandler.class);
		addRequestHandler("reload", ReloadHandler.class);
		addRequestHandler("QuickPlayWait", QuickPlayWait.class);
		addRequestHandler("QuickPlayStart", QuickPlayStart.class);

		addEventHandler(SFSEventType.USER_DISCONNECT, OnUserGoneHandler.class);
		addEventHandler(SFSEventType.USER_LEAVE_ROOM, OnUserGoneHandler.class);
		addEventHandler(SFSEventType.USER_LOGOUT, OnUserGoneHandler.class);

		trace("FPS extension initialized");
	}

	public void clientInstantiatePlayer(CombatPlayer player) {
		clientInstantiatePlayer(player, null);
	}

	//Send the player instantiation message to all the clients or to a specified user only
	public void clientInstantiatePlayer(CombatPlayer player, User targetUser) {
		ISFSObject data = new SFSObject();

		player.toSFSObject(data);
		Room currentRoom = RoomHelper.getCurrentRoom(this);
		if (targetUser == null) {
			// Sending to all the users
			List<User> userList = UserHelper.getRecipientsList(currentRoom);
			this.send("spawnPlayer", data, userList);
		}
		else {
			// Sending to the specified user
			this.send("spawnPlayer", data, targetUser);
		}
	}

	public void clientUpdateAmmo(CombatPlayer player) {
		// TODO Auto-generated method stub
		
	}

	public void clientUpdateHealth(CombatPlayer player) {
		// TODO Auto-generated method stub
		
	}

	public void clientRemoveItem(Item item, CombatPlayer player) {
		// TODO Auto-generated method stub
		
	}

	public void clientEnemyShotFired(CombatPlayer player) {
		// TODO Auto-generated method stub
		
	}

	public void clientReloaded(CombatPlayer player) {
		// TODO Auto-generated method stub
		
	}

	public void updatePlayerScore(CombatPlayer fromPlayer) {
		// TODO Auto-generated method stub
		
	}

	public void clientInstantiateItem(Item item) {
		// TODO Auto-generated method stub
		
	}

	public void clientInstantiateBullet(Projectile bullet) {
		// TODO Auto-generated method stub
		
	}

	public void clientKillPlayer(CombatPlayer pl, CombatPlayer killerPl) {
		ISFSObject data = new SFSObject();
		data.putInt("id", pl.getSfsUser().getId());
		data.putInt("killerId", killerPl.getSfsUser().getId());

		Room currentRoom = RoomHelper.getCurrentRoom(this);
		List<User> userList = UserHelper.getRecipientsList(currentRoom);
		this.send("killed", data, userList);
	}
}

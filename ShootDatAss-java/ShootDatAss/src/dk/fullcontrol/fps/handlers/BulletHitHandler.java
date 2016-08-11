package dk.fullcontrol.fps.handlers;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import dk.fullcontrol.fps.utils.RoomHelper;

//Player's local bullet hit something. check with corresponding remote bullet in world
public class BulletHitHandler extends BaseClientRequestHandler {

	@Override
	public void handleClientRequest(User u, ISFSObject data) {
		int bullet = data.getInt("bullet");
		int target = data.getInt("target");
		RoomHelper.getWorld(this).bulletCollide(u,bullet,target);
	}
}
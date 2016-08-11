package com.gankers.shootdatass;

import java.util.List;

import com.smartfoxserver.v2.entities.User;

public class UserListHelper {
	
	public static List<User> GetUserList(User user) {
		return user.getLastJoinedRoom().getUserList();
	}

		
}

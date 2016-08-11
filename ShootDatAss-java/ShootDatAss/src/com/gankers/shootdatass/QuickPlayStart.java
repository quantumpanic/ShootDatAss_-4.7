package com.gankers.shootdatass;


import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class QuickPlayStart extends BaseClientRequestHandler{
	
	@Override
	public void handleClientRequest(User user, ISFSObject objIn) {
		
		send("QuickPlayStart", objIn, UserListHelper.GetUserList(user));
		
	}
}

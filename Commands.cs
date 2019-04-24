//[---------------------------------------------------------]
//[                                                         ]
//[       System Management Version 2 Build 120617A         ]
//[                       Commands.cs                       ]
//[                                                         ]
//[---------------------------------------------------------]


//**************************//
//  Mute    /////////////////
//************************//

function serverCmdMute(%client,%user,%time,%reason)
{	
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		//
		// -A_ALL- stands for Admin All
		// It mutes anyone who isn't an admin or moderator
		//
		if(%user $= "-A_ALL-")
		{
			if($SM::ANP_Mute == true)
			{
				messageClient(%client,'',"\c2You cannot toggle AOC because it is already enabled. Say /unMute -A_ALL- to toggle it.");
				return;
			}
			if(%time $= "")
				return;
			else
			{
				if(isInteger(%time) == false)
				{
					messageClient(%client,'',"\c2You cannot mute someone with a time of \"\c3" @ %time @ "\c2\".");
					return;
				}
				if(%time < 30 && %time != -1 || %time > 3600)
				{
					messageClient(%client,'',"\c2You cannot mute someone for less than 30 seconds or more than 60 minutes. (3600 sec.)");
					return;
				}
				else if(isInteger(%time))
				{
					$SM::MuteTime = %time;
					decAllMuteTime(%time);
					
					%cg = nameToID("ClientGroup");
					for(%i = 0; %i < %cg.getCount(); %i++)
					{
						%cls = %cg.getObject(%i);
						if(%cls.isAdmin == false)
						{
							if(%cls.isModerator == false)
							{
								%cls.isMuted = false;
								%cls.MuteTime = false;
								cancel(%cls.MuteSchedule);
							}
							else
								continue;
						}
						continue;
					}
					
					if(%time == -1)
						messageAll('MsgAdminForce',"<font:impact:28>\c3" @ %client.name @ "\c2 has permanently muted all non-authoritive personnel\c2! \c3(Admin-Only-Chat ENABLED)");
					else
						messageAll('MsgAdminForce',"<font:impact:28>\c3" @ %client.name @ "\c2 has muted all non-authoritive personnel for \c3" @ %time @ " seconds\c2! \c3(Admin-Only-Chat ENABLED)");
					
					warn("Admin Log - Command Mute" NL "All Non-AP" NL "Time: " @ %time @ " Seconds" NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
					SM_logAdminEvent("Mute","All Non-AP","Time: " @ %time SPC "Seconds","Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
					$SM::ANP_Mute = true;
					return;
				}
			}
		}
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
		
			if(%user.isMuted == true)
				return;
			if(%user.getBLID() == getNumKeyID())
			{
				messageClient(%client,'',"\c3System Management\c6: You cannot mute the Host of the server. A notification has been sent to the Host.");
				messageClient(%user,'',"\c3System Management\c6: " @ %client.name @ " (BLID: " @ %client.getBLID() @ ") has tried to mute you.");
				return;
			}
			else
			{
				if(%time $= "")
					return;
				else
				{
					if(isInteger(%time) == false)
					{
						messageClient(%client,'',"\c2You cannot mute someone with a time of \"\c3" @ %time @ "\c2\".");
						return;
					}
					if(%time < 30 && %time != -1 || %time > 3600)
					{
						messageClient(%client,'',"\c2You cannot mute someone for less than 30 seconds or more than 60 minutes. (3600 sec.)");
						return;
					}
					else if(isInteger(%time))
					{
						%user.isMuted = true;
						%user.MuteTime = %time;
						%user.decMuteTime(%time);
						if(%time == -1 && %reason $= "")
						{
							%reason = "N/A";
							messageAllExcept(%user,'MsgAdminForce',"\c3" @ %client.name @ " \c2has permanently muted\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ")");
							messageClient(%user,'MsgAdminForce',"\c3" @ %client.name @ " \c2has permanently muted\c3 you\c2.");
						}
						else if(%time == -1 && %reason !$= "")
						{
							messageAllExcept(%user,'MsgAdminForce',"\c3" @ %client.name @ " \c2has permanently muted\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") - \"" @ %reason @ "\"");
							messageClient(%user,'MsgAdminForce',"\c3" @ %client.name @ " \c2has permanently muted\c3 you \c2- \"" @ %reason @ "\"");
						}
						else if(%reason !$= "" && %time != -1)
						{
							messageAllExcept(%user,'',"\c3" @ %client.name @ " \c2has muted\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") for \c3" @ %time @ " seconds \c2- \"" @ %reason @ "\"");
							messageClient(%user,'',"\c3" @ %client.name @ " \c2has muted you for \c3" @ %time @ " seconds \c2- \"" @ %reason @ "\"");
						}
						else if(%time != -1)
						{
							%reason = "N/A";
							messageAllExcept(%user,'',"\c3" @ %client.name @ " \c2has muted\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") for \c3" @ %time @ " seconds\c2.");
							messageClient(%user,'',"\c3" @ %client.name @ " \c2has muted you for \c3" @ %time @ " seconds\c2.");
						}
						warn("Admin Log - Command Mute" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() @ " Time: " @ %time @ " Seconds" NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
						SM_logAdminEvent("Mute","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Time: " @ %time SPC "Seconds","Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
					}
				}
			}
		}
	}
}

function serverCmdUnMute(%client,%user,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		//
		// -A_ALL- stands for Admin All
		// It unmutes anyone who isn't an admin or moderator
		//
		if(%user $= "-A_ALL-")
		{
			if($SM::ANP_Mute == false)
			{
				messageClient(%client,'',"\c2You cannot toggle AOC because it is already disabled. Say /Mute -A_ALL- to toggle it.");
				return;
			}
			
			$SM::MuteTime = false;
			cancel($SM::MuteSchedule);
			%cg = nameToID("ClientGroup");
			for(%i = 0; %i < %cg.getCount(); %i++)
			{
				%cls = %cg.getObject(%i);
				if(%cls.isAdmin == false)
				{
					if(%cls.isModerator == false)
					{
						%cls.isMuted = false;
						%cls.MuteTime = false;
						cancel(%cls.MuteSchedule);
					}
					else
						continue;
				}
				continue;
			}

			messageAll('MsgAdminForce',"<font:impact:28>\c3" @ %client.name @ "\c2 has unmuted all non-authoritive personnel\c2! \c3(Admin-Only-Chat DISABLED)");
			warn("Admin Log - Command unMute" NL "All Non-AP" NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
			SM_logAdminEvent("Mute","All Non-AP","Time: " @ %time SPC "Seconds","Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
			$SM::ANP_Mute = false;
			return;
		}
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
		
			if(%user.isMuted == false)
				return;
			else
			{
				%user.isMuted = false;
				%user.MuteTime = false;
				cancel(%user.MuteSchedule);
				if(%reason !$= "")
				{
					messageAllExcept(%user,'',"\c3" @ %client.name @ " \c2has unmuted\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") - \"" @ %reason @ "\"");
					messageClient(%user,'',"\c3" @ %client.name @ " \c2has unmuted you - \"" @ %reason @ "\"");
				}
				else
				{
					%reason = "N/A";
					messageAllExcept(%user,'',"\c3" @ %client.name @ " \c2has unmuted\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ").");
					messageClient(%user,'',"\c3" @ %client.name @ " \c2has unmuted you.");
				}
				
				warn("Admin Log - Command unMute" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
				SM_logAdminEvent("unMute","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
			}
		}
	}
}

function decAllMuteTime(%time)
{
	cancel($SM::MuteSchedule);
	if(%time == -1)
		return;
	else
	{
		if(%time >= 1 || $SM::MuteTime != %time)
		{
			%time -= 1;
			$SM::MuteTime = %time;
			%cg = nameToID("ClientGroup");
			for(%i = 0; %i < %cg.getCount(); %i++)
			{
				%client = %cg.getObject(%i);
				if(%client.isAdmin == false)
				{
					if(%client.isModerator == false)
						%client.bottomPrint("\c2Mute Time Left: \c3" @ %time @ " \c2seconds.",1,0);
					else
						continue;
				}
				else
					continue;
			}
			if(%time <= 0)
			{
				$SM::MuteTime = false;
				$SM::ANP_Mute = false;
				messageAll('MsgAdminForce',"\c3Admin-Only-Chat\c2 is now disabled, all non-authoritive personnel can talk now.");
				messageClient(%client,'',"\c2Your given mute has expired.");
			}
			else
				$SM::MuteSchedule = schedule(1000,0,"decAllMuteTime",%time);
		}
	}
	$SM::MuteSchedule = schedule(1000,0,"decAllMuteTime",%time);
}

function gameConnection::decMuteTime(%client,%time)
{
	cancel(%client.MuteSchedule);
	if(%client.isMuted == false)
		return;
	else
	{
		if(%time == -1 || %client.MuteTime != %time)
			return;
		else
		{
			if(%time >= 1)
			{
				//%sec = 1000;
				//%min = %sec * 60;
				//%hour = %min * 60;
				//day = %hour * 24;
				//%week = %day * 7;
				%time -= 1;
				%client.MuteTime = %time;
				%client.bottomPrint("\c2Mute Time Left: \c3" @ %time @ " \c2seconds.",1,0);
				if(%time <= 0)
				{
					%client.MuteTime = false;
					%client.isMuted = false;
					messageClient(%client,'',"\c2Your given mute has expired.");
					if(isFile("config/server/System Management/Logs/Muted Users/" @ %client.getBLID() @ ".txt") == true)
						fileDelete("config/server/System Management/Logs/Muted Users/" @ %client.getBLID() @ ".txt");
				}
				else
					%client.MuteSchedule = %client.schedule(1000,"decMuteTime",%time);
			}
		}
	}
}

//**************************//
//  Kick w/R     ////////////
// Overwritten    //////////
// the command to  ////////
// fit better needs.//////
//*********************//
package SM_KickRban
{
	function serverCmdKick(%client,%user,%reason)
	{
		//This is actually a pretty cool way of doing a command like this, especially for alternate
		//authoritive access.
		
		// Let's overwrite the command first
		// User 1 2 & 3 -type plus PCmds algorithm check added
		// 
		//getWord 0 = isAdmin : getWord 1 = canUsePCmds
		if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
		{
			if(%user !$= "")
			{
				//cool algorithm so you can still use the kick button from the admin gui
				//%userA is client object by name
				//%userB is client object by blid
				//%userC is client object by player object
				//%userD is client object
				%userA = findclientbyname(%user);
				if(isObject(%userA) == false || %userA == 0)
				{
					%userB = findclientbyBL_ID(%user);
					if(isObject(%userB) == false || %userB == 0)
					{
						%userC = %user.client;
						if(isObject(%userC) == false || %userC == 0)
						{
							%userD = %user;
							if(isObject(%userD) == false || %userD == 0)
								return;
							else
								%user = %userD;
						}
						else
							%user = %userC;
					}
					else
						%user = %userB;
				}
				else
					%user = %userA;
				
				//-----
				// Check a person's authoritive position
				if(%user.getBLID() == getNumKeyID())
				{
					messageClient(%client,'',"\c3System Management\c6: You cannot kick the Host of the server. A notification has been sent to the Host.");
					messageClient(%user,'',"\c3System Management\c6: " @ %client.name @ " (BLID: " @ %client.getBLID() @ ") has tried to kick you.");
					return;
				}
				else if(%user.isSuperAdmin == true && %client.getBLID() != getNumKeyID())
				{
					messageClient(%client,'',"\c3System Management\c6: You cannot kick a Super Admin from the server. A notification has been sent to the Host and Super Admin.");
					messageClient(%user,'',"\c3System Management\c6: " @ %client.name @ " (BLID: " @ %client.getBLID() @ ") has tried to kick you.");
					return;
				}
				if(%reason !$= "")
				{
					%user.delete("You have been kicked by " @ %client.name @ " (BLID: " @ %client.getBLID() @ ")" NL "Reason: " @ %reason);
					messageAll('MsgAdminForce',"\c3" @ %client.name @ " \c2has kicked\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") - \"" @ %reason @ "\"");
				}
				else
				{
					%reason = "N/A";
					%user.delete("You have been kicked by " @ %client.name @ " (BLID: " @ %client.getBLID() @ ")");
					messageAll('MsgAdminForce',"\c3" @ %client.name @ " \c2has kicked\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ")");
				}
				
				//Log the action
				warn("Admin Log - Command Kick" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
				SM_logAdminEvent("Kick","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
			}
			else
				return;
		}
		else if(getWord(%client.canUsePCmds(),1) == false)
			messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
	}
	
	//Added a PCmds check
	function serverCmdBan(%client,%name,%blid,%time,%reason)
	{
		if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
			parent::serverCmdBan(%client,%name,%blid,%time,%reason);
		else
			messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
	}
};
activatePackage(SM_KickRban);

//**************************//
//  Freeze    ///////////////
//************************//

function serverCmdFreeze(%client,%user,%time,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user $= "-A_ALL-")
		{
			if(%time $= "")
				return;
			else
			{
				if(isInteger(%time) == false)
				{
					messageClient(%client,'',"\c2You cannot freeze someone with a time of \"\c3" @ %time @ "\c2\".");
					return;
				}
				if(%time < 10 && %time != -1 || %time > 120)
				{
					messageClient(%client,'',"\c2You cannot freeze someone for less than 10 seconds or over 120 seconds.");
					return;
				}
				else if(isInteger(%time))
				{
					%clGroup = ClientGroup;
					for(%i = 0; %i < %clGroup.getCount(); %i++)
					{
						%clGObj = %clGroup.getObject(%i);
						%clGPlayer = %clGObj.player;
						if(isObject(%clGPlayer))
						{
							%clGObj.oldPlayerDatablock = %clGPlayer.getDatablock().getID();
							%clGPlayer.changeDatablock(PlayerSMFrozenArmor.getID());
							%clGObj.oldPlayer = %clGPlayer.getID();
						}
						
						%camera = %clGObj.camera;
						//%camera.setFlyMode();
						%camera.setmode("corpse",%clGPlayer);
						%clGObj.setControlObject(%camera);
						%clGObj.isFrozen = true;
						%clGObj.FreezeTime = %time;
						%clGObj.decFreezeTime(%time);
					}
					messageAll('MsgAdminForce',"<font:impact:28>\c3" @ %client.name @ "\c2 has frozen everyone!");
					warn("Admin Log - Command Freeze" NL "Everyone" NL "Time: " @ %time @ " Seconds" NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
					SM_logAdminEvent("Freeze","Everyone","Time: " @ %time SPC "Seconds","Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
				}
			}
		}
		else if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
				
			if(%time $= "")
				return;
			else
			{
				if(isInteger(%time) == false)
				{
					messageClient(%client,'',"\c2You cannot freeze someone with a time of \"\c3" @ %time @ "\c2\".");
					return;
				}
				else if(%time < 10 && %time != -1 || %time > 120)
				{
					messageClient(%client,'',"\c2You cannot freeze someone for less than 10 seconds or over 120 seconds. (2 min.)");
					return;
				}
				else
				{
					%userPlayer = %user.player;
					if(isObject(%userPlayer))
					{
						%user.oldPlayerDatablock = %userPlayer.getDatablock().getID();
						%userPlayer.changeDatablock(PlayerSMFrozenArmor.getID());
						%user.oldPlayer = %userPlayer.getID();
					}
					
					%camera = %user.camera;
					//%camera.setFlyMode();
					%camera.setmode("corpse",%userPlayer);
					%user.setControlObject(%camera);
					%user.isFrozen = true;
					%user.FreezeTime = %time;
					%user.decFreezeTime(%time);
					messageAll('MsgAdminForce',"<font:impact:24>\c3" @ %client.name @ "\c2 has frozen \c3" @ %user.name @ "\c2!");
					warn("Admin Log - Command Freeze" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() @ " Time: " @ %time @ " Seconds" NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
					SM_logAdminEvent("Freeze","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Time: " @ %time SPC "Seconds","Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
				}
			}
		}
	}
}

function serverCmdUnFreeze(%client,%user,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user $= "-ALL-")
		{
			%clGroup = ClientGroup;
			for(%i = 0; %i < %clGroup.getCount(); %i++)
			{
				%clGObj = clGroup.getObject(%i);
				%clGPlayer = %clGObj.oldPlayer.getID();
				%clGObj.isFrozen = false;
				cancel(%clGObj.FreezeSchedule);
				%clGObj.FreezeTime = 0;
				if(!isObject(%clGPlayer))
				{
					%clGObj.instantRespawn();
					%clGPlayer = %clGObj.player;
				}
				else
					%clGPlayer.changeDatablock(%clGObj.oldPlayerDatablock);
					
				%camera = %clGObj.camera;
				//%camera.setFlyMode();
				%camera.setmode("Observer",%clGPlayer);
				%clGObj.setControlObject(%clGPlayer);
			}
			messageAll('MsgAdminForce',"<font:impact:24>\c3" @ %client.name @ "\c2 has un-frozen everyone!");
		}
		else if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(%user.isFrozen == false)
				return;
			
			%userPlayer = %user.oldPlayer.getID();
			cancel(%user.FreezeSchedule);
			%user.FreezeTime = 0;
			%user.isFrozen = false;
			if(!isObject(%userPlayer))
			{
				%user.instantRespawn();
				%userPlayer = %user.player;
			}
			else
				%userPlayer.changeDatablock(%user.oldPlayerDatablock);
			
			%camera = %clGObj.camera;
			//%camera.setFlyMode();
			camera.setmode("Observer",%clGPlayer);
			%user.setControlObject(%userPlayer);
			messageAll('MsgAdminForce',"<font:impact:24>\c3" @ %client.name @ "\c2 has un-frozen \c3" @ %user.name @ "\c2!");
		}
	}
}

function gameConnection::decFreezeTime(%client,%time)
{
	cancel(%client.FreezeSchedule);
	if(%client.isFrozen == false)
		return;
	else
	{
		if(%time == -1 || %client.FreezeTime != %time)
			return;
		else
		{
			if(%time >= 1)
			{
				//%sec = 1000;
				//%min = %sec * 60;
				//%hour = %min * 60;
				//day = %hour * 24;
				//%week = %day * 7;
				%time -= 1;
				%client.FreezeTime = %time;
				%client.bottomPrint("\c2Freeze Time Left: \c3" @ %time @ " \c2seconds.",1,0);
				if(%time <= 0)
				{
					%client.FreezeTime = 0;
					%client.isFrozen = false;
					%clPlayer = %client.oldPlayer.getID();
					%clPlayer.changeDatablock(%client.oldPlayerDatablock);
					if(!isObject(%clPlayer))
					{
						%user.instantRespawn();
						%clPlayer = %client.player;
					}
					//%camera = %clGObj.camera;
					//%camera.setFlyMode();
					//%camera.mode = "Observer";
					%client.setControlObject(%clPlayer);
					messageClient(%client,'',"\c2Your given freeze has expired.");
				}
				else
					%client.FreezeSchedule = %client.schedule(1000,"decFreezeTime",%time);
			}
		}
	}
}

package SM_FreezeFix
{
	function serverCmdSuicide(%client,%a)
	{
		if(!%client.isFrozen)
			parent::serverCmdSuicide(%client,%a);
	}
	
	function GameConnection::SpawnPlayer(%client)
	{
		if(!%client.isFrozen)
			parent::SpawnPlayer(%client);
	}
	
	function PlayerSMFrozenArmor::onTrigger(%this,%obj,%slot,%state)
	{
		return;
	}
	
	//function Armor::onTrigger(%this,%obj,%slot,%state)
	//{
	//	if(!%obj.client.isFrozen)
	//		parent::onTrigger(%this,%obj,%slot,%state);
	//}

	function serverCmdUseTool(%client,%slot)
	{
		%player = %client.player;
		if(!isObject(%player) || %player.getDatablock().getID() != PlayerSMFrozenArmor.getID())
			parent::serverCmdUseTool(%client,%slot);
	}
};
activatePackage(SM_FreezeFix);

//**************************//
//  ForceKill    ////////////
//************************//

function serverCmdForceKill(%client,%user,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(isObject(%user.player) == true)
			{
				%user.player.kill();
				if(%reason !$= "")
					messageAll('MsgAdminForce',"\c3" @ %client.name @ " \c2has ForceKilled\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") - \"" @ %reason @ "\"");
				else
				{
					%reason = "N/A";
					messageAll('MsgAdminForce',"\c3" @ %client.name @ " \c2has ForceKilled\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ")");
				}
			}
			else
				return;
			warn("Admin Log - Command ForceKill" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
			SM_logAdminEvent("ForceKill","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
		}
		else
			return;
	}
	else if(getWord(%client.canUsePCmds(),1) == false)
		messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
}

//**************************//
//  ForceSpawn    ///////////
//************************//

function serverCmdForceSpawn(%client,%user,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(%reason !$= "")
			{
				%user.instantrespawn();
				messageAll('MsgAdminForce',"\c3" @ %client.name @ " \c2has ForceSpawned\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ") - \"" @ %reason @ "\"");
			}
			else
			{
				%reason = "N/A";
				%user.instantrespawn();
				messageAll('MsgAdminForce',"\c3" @ %client.name @ " \c2has ForceSpawned\c3 " @ %user.name @ "\c2 (BLID: " @ %user.getBLID() @ ")");
			}
			
			warn("Admin Log - Command ForceSpawn" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Reason: " @ %reason NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
			SM_logAdminEvent("ForceSpawn","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Reason: " @ %reason,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
		}
		else
			return;
	}
	else if(getWord(%client.canUsePCmds(),1) == false)
		messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
}

//**************************//
//  Warn    /////////////////
//************************//

function serverCmdWarn(%client,%user,%msg,%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9,%a10,%a11,%a12)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(%msg !$= "")
			{
				messageClient(%client,'',"\c2Your warning message has been sent.");
				messageClient(%user,'',"\c2You have received a warning from Admin/Mod \c3" @ %client.name @ " \c2(BLID: " @ %client.getBLID() @ ").");
				if(%a1 $= "")
					messageClient(%user,'',"\c3WARNING\c2: " @ %msg);
				else
				{
					%msg = %msg SPC %a1 SPC %a2 SPC %a3 SPC %a4 SPC %a5 SPC %a6 SPC %a7 SPC %a8 SPC %a9 SPC %a10 SPC %a11 SPC %a12;
					messageClient(%user,'',"\c3WARNING\c2: " @ %msg);
				}
			}
			warn("Admin Log - Command Warn" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Reason: " @ %msg NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
			SM_logAdminEvent("Warn","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Reason: " @ %msg,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
		}
	}
}

//**************************//
//  Set Health    ///////////
//************************//

function serverCmdSetHealth(%client,%user,%amount,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(isObject(%user.player) == true && isInteger(%amount) == true)
			{
				%user.player.sethealth(%amount);
				messageAllExcept(%user,'',"\c3" @ %client.name @ "has set \c3" @ %user.name @ "\'s \c2health to \c3" @ mCeil(%user.player.getDatablock().maxDamage - %user.player.getDamageLevel()) @ "\c2.");
				messageClient(%user,'',"\c3" @ %client.name @ " \c2has set your health to \c3" @ mCeil(%user.player.getDatablock().maxDamage - %user.player.getDamageLevel()) @ "\c2.");
				//mCeil(%name.player.getDatablock().maxDamage-%name.player.getDamageLevel());
			}
			else
				return;
			warn("Admin Log - Command SetHealth" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Amount To: " @ %amount NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
			SM_logAdminEvent("SetHealth","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Amount To: " @ %amount,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
		}
	}
	else if(getWord(%client.canUsePCmds(),1) == false)
		messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
}

//**************************//
//  Set Size    /////////////
//************************//

function serverCmdSetSize(%client,%user,%sizeX,%sizeY,%sizeZ)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(isObject(%user.player) == true)
			{
				if(%sizeX !$= "" && isInteger(getSubStr(%sizeX,0,1)) == true && %sizeY !$= "" && isInteger(getSubStr(%sizeY,0,1)) == true && %sizeZ !$= "" && isInteger(getSubStr(%sizeZ,0,1)) == true)
				{
					%user.player.setPlayerScale(%sizeX SPC %sizeY SPC %sizeZ);
					warn("Admin Log - Command SetSize" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Size X: " @ %sizeX NL "Size Y: " @ %sizeY NL "Size Z: " @ %sizeX NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
					SM_logAdminEvent("SetSize","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Size X: " @ %sizeX,"Size Y: " @ %sizeY,"Size Z: " @ %sizeZ,"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
					messageClient(%user,'',"\c3" @ %client.name @ " \c2has set your size to\c3 X " @ %sizeX @ " Y " @ %sizeY @ " Z " @ %sizeZ);
				}
			}
		}
		else
			return;
	}
	else if(getWord(%client.canUsePCmds(),1) == false)
		messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
}

//**************************//
//  Set Datablock    ////////
//************************//

function serverCmdSetDatablock(%client,%user,%datablock,%reason)
{
	if(getWord(%client.canUsePCmds(),0) == true && getWord(%client.canUsePCmds(),1) == true || %client.getBLID() == getNumKeyID())
	{
		if(%user !$= "")
		{
			//cool algorithm so you can still use the kick button from the admin gui
			//%userA is client object by name
			//%userB is client object by blid
			//%userC is client object by player object
			//%userD is client object
			%userA = findclientbyname(%user);
			if(isObject(%userA) == false || %userA == 0)
			{
				%userB = findclientbyBL_ID(%user);
				if(isObject(%userB) == false || %userB == 0)
				{
					%userC = %user.client;
					if(isObject(%userC) == false || %userC == 0)
					{
						%userD = %user;
						if(isObject(%userD) == false || %userD == 0)
							return;
						else
							%user = %userD;
					}
					else
						%user = %userC;
				}
				else
					%user = %userB;
			}
			else
				%user = %userA;
			
			if(isObject(%user.player) == true && isObject(%datablock) == true && %datablock.getName() !$= "")
			{
				%datablock = %datablock.getID();
				%user.player.changeDatablock(%datablock);
				messageAllExcept(%user,'',"\c3" @ %client.name @ "has set \c3" @ %user.name @ "\'s \c2datablock to \c3" @ %datablock.uiname @ "\c2.");
				messageClient(%user,'',"\c3" @ %client.name @ " \c2has set your datablock to \c3" @ %datablock.uiname @ "\c2.");
				//mCeil(%name.player.getDatablock().maxDamage-%name.player.getDamageLevel());
			}
			else
				return;
			warn("Admin Log - Command SetDatablock" NL "Name: " @ %user.name @ " BLID: " @ %user.getBLID() NL "Datablock: " @ %datablock.getName() NL "Admin/Mod: " @ %client.name @ " BLID: " @ %client.getBLID());
			SM_logAdminEvent("SetDatablock","Name: " @ %user.name,"BLID: " @ %user.getBLID(),"Datablock: " @ %datablock.getName(),"Admin/Mod: " @ %client.name,"BLID: " @ %client.getBLID());
		}
	}
	else if(getWord(%client.canUsePCmds(),1) == false)
		messageClient(%client,'',"\c3System Management\c6: You have unsufficient privileges to use this command.");
}

function serverCmdAPM(%client)
{
	%client.isUsingAPM = true;
}

function serverCmdChangeBGIDColor(%client,%brickgroup,%firstColorID,%secondColorID)
{
	if(%client.isAdmin == false)
		return;
	
	if(isObject("brickgroup_" @ %brickgroup))
	{
		//finish later
	}
}

datablock PlayerData(PlayerSMFrozenArmor : PlayerStandardArmor)
{
	thirdPersonOnly = 1;

	runForce = 0;
	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;
	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;
	drag = 10;

	jumpForce = 0;
	jumpEnergyDrain = 0;
	minJumpEnergy = 0;
	canJet = 0;
	jetEnergyDrain = 0;
	minJetEnergy = 0;

	runSurfaceAngle  = 0;
	jumpSurfaceAngle = 0;

	uiName = "SM Frozen Player";
};

talk("Commands.cs - Success");
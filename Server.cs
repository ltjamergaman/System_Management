$SM::Version = "Version 2 BUILD 120617A";
$SM::Directory[1] = "config/server/System Management/class.cs";
$SM::Directory[2] = "config/server/System Management/Logs/registered.cs";
$SM::Directory[3] = "config/server/System Management/Vars/registered.cs";
$SM::Directory[4] = "config/client/System Management/class.cs";
$SM::Directory[5] = "config/client/System Management/Logs/registered.cs";

$SM::FilterCW_Enabled = false;
$SM::FilteredWords::Word[1] = "n00b";
$SM::FilteredWords::Word[2] = "noob";
$SM::FilteredWords::Word[3] = "jamergayman";
$SM::FilteredWords::Word[4] = "lt";
$SM::FilteredWords::Word[5] = "jamer";
$SM::FilteredWords::Count = 5;
$SM::ReplacementWords::Word[1] = "diddler";
$SM::ReplacementWords::Word[2] = "fiddler";
$SM::ReplacementWords::Word[3] = "riddler";
$SM::ReplacementWords::Word[4] = "liddler";
$SM::ReplacementWords::Word[5] = "giddler";
$SM::ReplacementWords::Count = 5;

function exSM()
{
	exec("./server.cs");
}

exec("./commands.cs");
exec("./Data Transfer.cs");
//exec("./client.cs");


function SM_RegisterDirectories()
{
	%file = new FileObject();
	%file.openForWrite($SM::Directory[1]);
	%file.writeLine("---Write BEGIN---" NL $SM::Version);
	%file.close();
	%file.delete();
	
	%file = new FileObject();
	%file.openForWrite($SM::Directory[2]);
	%file.writeLine("---Write BEGIN---" NL "Registered by SM_RegisterDirectories()");
	%file.close();
	%file.delete();
	
	%file = new FileObject();
	%file.openForWrite($SM::Directory[3]);
	%file.writeLine("---Write BEGIN---" NL "Registered by SM_RegisterDirectories()");
	%file.close();
	%file.delete();
	
	%file = new FileObject();
	%file.openForWrite($SM::Directory[4]);
	%file.writeLine("---Write BEGIN---" NL $SM::Version);
	%file.close();
	%file.delete();
	
	%file = new FileObject();
	%file.openForWrite($SM::Directory[5]);
	%file.writeLine("---Write BEGIN---" NL "Registered by SM_RegisterDirectories()");
	%file.close();
	%file.delete();
}

SM_RegisterDirectories();

function isInteger(%string)
{
	%search = "- 0 1 2 3 4 5 6 7 8 9";
	for(%i = 0; %i < getWordCount(%search); %i++)
	{
		%string = strReplace(%string,getWord(%search,%i),"");
	}
	if(%string $= "")
		return true;
	return false;
}

function notifyAdmins(%msg,%a,%b)
{
	for(%i = 0; %i < clientGroup.getCount(); %i++)
	{
		%adminClients = clientGroup.getObject(%i);
		if(%adminClients.isAdmin || %adminClients.isModerator)
			messageClient(%adminClients,'',"\c3ADMIN NOTE\c2: " @ %msg SPC "\c0" @ %a SPC "\c1" @ %b);
	}
}

function SM_logAdminEvent(%a,%b,%c,%d,%e,%f,%g,%h,%i)
{
	if(%a $= "" || %b $= "")
		return;
	if(%c $= "")
	{
		%c = -1;
	}
	if(%d $= "")
	{
		%d = -1;
	}
	if(%e $= "")
	{
		%e = -1;
	}
	if(%f $= "")
	{
		%f = -1;
	}
	if(%g $= "")
	{
		%g = -1;
	}
	if(%h $= "")
	{
		%h = -1;
	}
	if(%i $= "")
	{
		%i = -1;
	}
	%date = getWord(getDateTime(),0);
	%time = getWord(getDateTime(),1);
	%file = new FileObject();
	%filepath = "config/server/System Management/Admin Events Logs/" @ getSubStr(%date,0,2) @"-" @ getSubStr(%date,3,2) @ "-" @ getSubStr(%date,6,2) @ ".txt";
	%file.openForAppend(%filepath);
	%file.writeLine("[" @ %time @ "]");
	%file.writeLine("|-" SPC %a);
	%file.writeLine("|--" SPC %b);
	%file.writeLine("|---" SPC %c);
	%file.writeLine("|----" SPC %d);
	%file.writeLine("|-----" SPC %e);
	%file.writeLine("|------" SPC %f);
	%file.writeLine("|-------" SPC %g);
	%file.writeLine("|--------" SPC %h);
	%file.writeLine("|---------" SPC %i);
	%file.writeLine(":-|- NEW ENTRY BELOW -|-:");
	%file.writeLine("--------------------------------------------------------------------");
	%file.close();
	%file.delete();
}

//This function is a simplistic way to handle security of functions.
//"PCmds" - "Privileged Commands" : Handles the ability of a person using authoritive commands.
//%client - client obj, %oR - overriding val.
function GameConnection::canUsePCmds(%client,%oR)
{
	if(%oR == true)
	{
		%client.canUsePCmds = true;
		return true;
	}
	else if(%oR == false)
	{
		%client.canUsePCmds = false;
		return false;
	}
	
	//First, we have to check if the client is of authority.
	//Second, we have to check if the client can use "PCmds".
	//If both are true, then the %val variable returns "1 1" when this command is called.
	//If the client can't use "PCmds", then it returns "1 0".
	//If both are false, then the %val variable returns "0 0" when this command is called.
	//A client cannot use "PCmds" without having authoritive command, it wouldn't justify "being more secure".	
	if(%client.isAdmin == true || %client.isModerator == true)
		if(%client.canUsePCmds == true)
			%val = true SPC true;
		else
			%val = true SPC false;
	else
		%val = false SPC false;

	return %val;
}

package SM_ServerClientFuncs
{
	function GameConnection::autoAdminCheck(%client)
	{
		%r = parent::autoAdminCheck(%client);
		//talk(%a SPC %b SPC %c);
		//%client.schedule(100,"chatMessage","\c3This server is running Lt. Jamergaman's System Management add-on. " @ $SM::Version);
		if(isFile("config/server/System Management/Logs/Muted Users/" @ %client.getBLID() @ ".txt") == true)
		{
			%file = new FileObject();
			%file.openForRead("config/server/System Management/Logs/Muted Users/" @ %client.getBLID() @ ".txt");
			%line = getWord(%file.readLine(),2);
			if(%line $= "0")
			{
				%client.isMuted = false;
				%file.close();
				%file.delete();
				
			}
			else if(%line $= "")
			{
				%client.isMuted = false;
				%file.close();
				%file.delete();
			}
			else
			{
				%client.MuteTime = %line;
				%client.isMuted = true;
				%client.decMuteTime(%client.MuteTime);
				%file.close();
				%file.delete();
			}
		}
		else
			%client.isMuted = false;
		
		SM_checkCoHost(%client);
		SM_checkModerator(%client);
		
		if(%client.isAdmin == true || %client.isModerator == true)
			%client.canUsePCmds = true;
		else
			%client.canUsePCmds = false;
		return %r;
	}

	function GameConnection::onClientEnterGame(%client)
	{
		parent::onClientEnterGame(%client);
	}

	function GameConnection::onClientLeaveGame(%client)
	{
		if(%client.isMuted == true)
		{
			cancel(%client.MuteSchedule);
			%file = new FileObject();
			%file.openForWrite("config/server/System Management/Logs/Muted Users/" @ %client.getBLID() @ ".txt");
			%file.writeLine("Time Left: " @ %client.MuteTime);
			%file.close();
			%file.delete();
		}
		
		commandToClient(%client,'receiveSMULP');
		parent::onClientLeaveGame(%client);
	}
	
	function serverCmdMessageSent(%client,%msg)
	{
		if($SM::FilterCW_Enabled == true)
			%msg = filterCussWords(%msg);
		
		//this is a mess lmao dont worry about it tho it works you dick
		if(%client.isAdmin == true && %client.isSuperAdmin == false && %client.isModerator == false && %client.isCoHost == false && %client.bl_id != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c0Ad\c31\c6]";
			else
				%prefix = "\c6[\c0Ad\c30\c6]";
		}
		if(%client.isModerator == true && %client.isAdmin == false && %client.isSuperAdmin == false && %client.isCoHost == false && %client.getBLID() != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c4Mo\c31\c6]";
			else
				%prefix = "\c6[\c4Mo\c31\c6]";
		}
		if(%client.isSuperAdmin == true && %client.isAdmin == true && %client.isModerator == false && %client.isCoHost == false && %client.getBLID() != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c2SAd\c31\c6]";
			else
				%prefix = "\c6[\c2SAd\c31\c6]";
		}
		if(%client.isCoHost == true && %client.isSuperAdmin == true && %client.isAdmin == true && %client.isModerator == false && %client.getBLID() != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c5CHo\c31\c6]";
			else
				%prefix = "\c6[\c5CHo\c31\c6]";
		}
		if(%client.getBLID() == getNumKeyID())
			%prefix = "\c6[\c5Ho\c6]";
		
		%client.clanPrefix = %prefix SPC "";
		
		if(%client.isMuted == true)
		{
			if($SM::ANP_Mute == true)
			{
				messageClient(%client,'',"\c2You cannot talk because an admin muted all non-authoritve personnel.");
				return;
			}
			if(%client.muteTime == -1)
			{
				messageClient(%client,'',"\c2You cannot talk because you are \c3permanently \c2muted. Send a PM to an Admin using \c3/APM\c2.");
				return;
			}

			messageClient(%client,'',"\c2You cannot talk because you are muted. You have " @ %client.MuteTime @ " seconds remaining.");
			return;
		}
		
		parent::serverCmdMessageSent(%client,%msg);
	}
	
	function serverCmdTeamMessageSent(%client,%msg)
	{
		if($SM::FilterCW_Enabled == true)
			%msg = filterCussWords(%msg);
		
		if(%client.isAdmin == true && %client.isSuperAdmin == false && %client.isModerator == false && %client.isCoHost == false && %client.bl_id != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c0Ad\c31\c6]";
			else
				%prefix = "\c6[\c0Ad\c30\c6]";
		}
		if(%client.isModerator == true && %client.isAdmin == false && %client.isSuperAdmin == false && %client.isCoHost == false && %client.getBLID() != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c4Mo\c31\c6]";
			else
				%prefix = "\c6[\c4Mo\c31\c6]";
		}
		if(%client.isSuperAdmin == true && %client.isAdmin == true && %client.isModerator == false && %client.isCoHost == false && %client.getBLID() != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c2SAd\c31\c6]";
			else
				%prefix = "\c6[\c2SAd\c31\c6]";
		}
		if(%client.isCoHost == true && %client.isSuperAdmin == true && %client.isAdmin == true && %client.isModerator == false && %client.getBLID() != getNumKeyID())
		{
			if(%client.canUsePCmds == true)
				%prefix = "\c6[\c5CHo\c31\c6]";
			else
				%prefix = "\c6[\c5CHo\c31\c6]";
		}
		if(%client.getBLID() == getNumKeyID())
			%prefix = "\c6[\c5Ho\c6]";
		
		%client.clanPrefix = %prefix SPC "";
		
		if(%client.isMuted == true)
		{
			if($SM::ANP_Mute == true)
			{
				messageClient(%client,'',"\c2You cannot talk because an admin muted all non-authoritve personnel.");
				return;
			}
			if(%client.muteTime == -1)
			{
				messageClient(%client,'',"\c2You cannot talk because you are \c3permanently \c2muted. Send a PM to an Admin using \c3/APM\c2.");
				return;
			}

			messageClient(%client,'',"\c2You cannot talk because you are muted. You have " @ %client.MuteTime @ " seconds remaining.");
			return;
		}
		
		parent::serverCmdTeamMessageSent(%client,%msg);
	}
};
activatePackage(SM_ServerClientFuncs);

package SM_CoHost
{
    function SM_checkCoHost(%client)
    {
        %list = $Pref::Server::CoHost;
        %bl_id = %client.BL_ID;
        if(hasItemOnList(%list,%bl_id))
        {
            messageAll('MsgAdminForce',"\c2" @ %client.name @ " has become Super Admin (Co-Host)");
            %client.isCoHost = true;
            %client.isSuperAdmin = true;
            %client.isAdmin = true;
			%client.isModerator = false;
            %client.sendPlayerListUpdate();
            commandtoclient(%client,'setAdminLevel',2);
        }
    }
    
    function serverCmdSetCoHost(%client,%name)
    {
        if(%client.BL_ID == getNumKeyID())
        {
            %Victim = findclientbyname(%name);
            %VictimID = findclientbyname(%name).getBLID();
            %VictimName = findclientbyname(%name).getPlayerName();
            %Victim.isCoHost = true;
            %Victim.isSuperAdmin = true;
            %Victim.isAdmin = true;
			%Victim.isModerator = false;
            %Victim.sendPlayerListUpdate();
            %client.sendPlayerListUpdate();
            commandtoclient(%Victim,'setAdminLevel',2);
            messageAll('MsgAdminForce',"\c2" @ %VictimName @ " has become Super Admin (Co-Host - Manual)");
        }
    }

	function serverCmdSetCHBLID(%client,%blid)
	{
		if(%client.BL_ID == getNumKeyID())
		{
			if(%blid !$= "" && isInteger(%blid) == true)
			{
				$Pref::Server::CoHost = %blid;
				messageClient(%client,'',"\c3System Management\c6: BL_ID \c2" @ %blid @ "\c6 set to the Auto Co-Host Check.");
				for(%i = 0; %i <= ClientGroup.getCount(); %i++)
				{
					%clientB = ClientGroup.getObject(%i);
					if(%clientB.getBLID() == %blid)
					{
						%Victim.isCoHost = true;
						%Victim.isSuperAdmin = true;
						%Victim.isAdmin = true;
						%Victim.isModerator = false;
						%Victim.sendPlayerListUpdate();
						%client.sendPlayerListUpdate();
						commandtoclient(%Victim,'setAdminLevel',2);
						messageAll('MsgAdminForce',"\c2" @ %VictimName @ " has become Super Admin (Co-Host - Auto)");
					}
					return;
				}
			}
		}
	}
};
activatepackage(SM_CoHost);

package SM_Moderator
{
    function SM_checkModerator(%client)
    {
        %list = $Pref::Server::autoModeratorList;
        %bl_id = %client.BL_ID;
        if(hasItemOnList(%list,%bl_id))
        {
            messageAll('MsgAdminForce',"\c2" @ %client.name @ " has become Moderator (Auto)");
			%client.isCoHost = false;
			%client.isSuperAdmin = false;
            %client.isAdmin = false;
            %client.isModerator = true;
            %client.sendPlayerListUpdate();
            commandToClient(%client,'setAdminLevel',0);
        }
		else
			%client.isModerator = false;
    }
	
	function SM_AddToModeratorList(%ID)
	{
		if(%ID $= "")
			return;
		else
		{
			if($Pref::Server::autoModeratorList $= "")
				$Pref::Server::autoModeratorList = %ID;
			else
				$Pref::Server::autoModeratorList = $Pref::Server::autoModeratorList SPC %ID;
			warn("SM_AddToModeratorList() - ID " @ %ID @ " has been added to List \"autoModeratorList\".");
		}
	}
	
	function serverCmdSetModAuto(%client,%user)
	{
		if(%client.isAdmin == true && %client.canUsePCmds == true)
		{
			if(%user !$= "")
			{
				//cool algorithm
				//%userA is client object
				//%userB is client object by blid
				//%user(c) is client object by name
				%userA = findclientbyname(%user.name);
				if(isObject(%userA) == false)
				{
					%userB = findclientbyBL_ID(%user);
					if(isObject(%userB) == false)
					{
						if(isObject(%user) == false)
							return;
						else
							%user = findclientbyname(%user);
					}
					else
						%user = %userB;
				}
				else
					%user = %userA;
				
				if(%user.isAdmin == true)
					return;
				else
				{
					%user.isModerator = true;
					%user.canUsePCmds = true;
					commandToClient(%user,'SetAdminLevel',0);
					%client.sendPlayerListUpdate();
					%user.sendPlayerListUpdate();
					messageAll('MsgAdminForce',"\c2" @ %user.name @ " has become Moderator (Auto) - Added by \c3" @ %client.name @ "\c2.");
					SM_AddToModeratorList(%user.getBLID());
				}
			}
		}
	}
	
	function serverCmdSetMod(%client,%user)
	{
		if(%client.isAdmin == true && %client.canUsePCmds == true)
		{
			if(%user !$= "")
			{
				//cool algorithm
				//%userA is client object
				//%userB is client object by blid
				//%user(c) is client object by name
				%userA = findclientbyname(%user.name);
				if(isObject(%userA) == false)
				{
					%userB = findclientbyBL_ID(%user);
					if(isObject(%userB) == false)
					{
						if(isObject(%user) == false)
							return;
						else
							%user = findclientbyname(%user);
					}
					else
						%user = %userB;
				}
				else
					%user = %userA;
				
				if(%user.isAdmin == true)
					return;
				else
				{
					%user.isModerator = true;
					commandToClient(%user,'SetAdminLevel',0);
					%client.sendPlayerListUpdate();
					%user.sendPlayerListUpdate();
					messageAll('MsgAdminForce',"\c2" @ %user.name @ " has become Moderator (Manual) - Added by \c3" @ %client.name @ "\c2.");
				}
			}
		}
	}
};
activatepackage(SM_Moderator);

$SM::Announcements::Enabled = 0;
$SM::Announcements::SchedTime = 90000; //90 seconds : 1 1/2 minutes
$SM::Announcements::Announcement[1] = "Admins, you are not allowed to change the map settings.";
$SM::Announcements::Announcement[2] = "Disrespect is not allowed here.";
$SM::Announcements::Announcement[3] = "If you found an issue with another player, please consult an admin or moderator about the issue or just use the /report command.";


function SM_getAnnouncementCount()
{
	for(%i = 1; $SM::Announcements::Announcement[%i] !$= ""; %i++)
	{
		//nothing
	}

	return %i - 1;
}

$SM::Announcements::Count = SM_getAnnouncementCount();

function SM_announce(%togCheck)
{
	SM_cancelAnnounce();
	if($SM::Announcements::Enabled == true)
	{
		if(%togCheck == true)
		{
			$SM::Announcements::Next_Num = 1;
			$SM::Announcements::Next = $SM::Announcements::Announcement[1];
			SM_announce(0);
		}
		else
		{
			announce("\c5Announcement\c6: " @ $SM::Announcements::Next);
			$SM::Announcements::Prev_Num = $SM::Announcements::Next_Num;
			$SM::Announcements::Prev = $SM::Announcements::Announcement[$SM::Announcements::Prev_Num];
			$SM::Announcements::Next_Num++;
			$SM::Announcements::Next = $SM::Announcements::Announcement[$SM::Announcements::Next_Num];
			if($SM::Announcements::Next_Num > $SM::Announcements::Count)
			{
				$SM::Announcements::Next_Num = 1;
				$SM::Announcements::Next = $SM::Announcements::Announcement[1];
			}
			$SM::Announcements::Schedule = schedule($SM::Announcements::SchedTime,0,"SM_announce",0);
		}
	}
}

function SM_cancelAnnounce()
{
	cancel($SM::Announcements::Schedule);
}

function SM_TogAnnounce()
{
	if($SM::Announcements::Enabled == true)
	{
		cancel($SM::Announcements::Schedule);
		messageAll('MsgAdminForce',"\c3System Management\c6: Announcements have been turned \c0off\c6.");
		$SM::Announcements::Enabled = false;
	}
	else if($SM::Announcements::Enabled == false)
	{
		$SM::Announcements::Enabled = true;
		messageAll('MsgAdminForce',"\c3System Management\c6: Announcements have been turned \c2on\c6.");
		SM_announce(1);
	}
}

function SM_newAncmnt(%text)
{
	%count = SM_getAnnouncementCount() + 1;
	$SM::Announcements::Announcement[%count] = %text;
	talk("A new announcement has been made.");
	announce("\c5Announcement\c6: " @ $SM::Announcements::Announcement[%count]);
}

function filterCussWords(%message)
{
	if($SM::FilterCW_Enabled == false)
		return;
	
	%wordCount = getWordCount(%message);
	
	for(%i = 0; %i < %wordCount; %i++)
	{
		%word = getWord(%message,%i);
		for(%a = 1; %a <= $SM::FilteredWords::Count; %a++)
		{
			if(%word $= $SM::FilteredWords::Word[%a])
				%word = $SM::ReplacementWords::Word[getRandom(1,$SM::ReplacementWords::Count)];
			else
				%word = %word;
		}
		%blah = %blah SPC %word;
	}
	
	%message = ltrim(%blah);
}

function serverCmdDediLoad(%fileName,%owner)
{
	announce("\c3System Management\c6: ATTEMPTING DEDILOAD -> FILE \c2" @ %filename @ "\c6.");
	serverDirectSaveFileLoad(%fileName,3,"",%owner);
}

//function checkForCussWords(%word)
//{
//	for(%i = 1; %i <= $SM::FilteredWords::Count; %i++)
//	{
//		if(%word $= $SM::FilteredWords::Word[%i])
//			%word = $SM::ReplacementWords::Word[getRandom(1,$SM::ReplacementWords::Count)];
//		else
//			%word = %word;
//	}
//}

//function SM_StpTCPServ(%name,%port)
//{
//	new TCPObject(getWord(%name,1));
//	
	//some other shit here
//}

talk("Server.cs - Success");
if(!isObject("SM_HelpDlg"))
	exec("./interface/SM_HelpDlg.gui");

canvas.schedule(100, "pushDialog", SM_HelpDlg);

function SM_LoadSMC()
{
	if($ADMINGUIPLUSPLUS_loaded == true)
		return;
	adminGui.delete();
	exec("./interface/admingui.gui");
	echo("SMC :- Loading");
}

function SM_unLoadSMC()
{
	if($ADMINGUIPLUSPLUS_loaded != true)
		return;
	adminGui.delete();
	exec($OLDADMINGUI);
	echo("SMC :- UnLoading");
}

package SM_unload
{
	function disconnectedCleanup()
	{
		parent::disconnectedCleanup();
		SM_unLoadSMC();
	}
};
//temp
adminGui.save("config/client/System Management/Temp/adminGui.gui");
$OLDADMINGUI = "config/client/System Management/Temp/adminGui.gui";

package SM_Client
{
	function onMissionDownloadPhase1(%a)
	{
		clientCmdSendSMHandshake();
		return parent::onMissionDownloadPhase1(%a);
	}
	
	function adminGui::onWake(%this,%a,%b,%c)
	{
		parent::onWake(%this,%a,%b,%c);
		adminGui_Reason.setText("");
	}

	function SM_doAction(%func,%obj,%a,%b,%c,%d,%e)
	{
		switch$(%func)
		{
			//warn
			case "1":
			commandToServer('Warn',%obj,%a);

			//forcespawn
			case "2":
			commandToServer('ForceSpawn',%obj,%a);

			//forcekill
			case "3":
			commandToServer('ForceKill',%obj,%a);

			//mute
			case "4":
			commandToServer('Mute',%obj,%a,%b);

			//unmute
			case "5":
			commandToServer('UnMute',%obj,%a);

			//setsize
			case "6":
			commandToServer('SetSize',%obj,%a,%b,%c);

			//sethealth
			case "7":
			commandToServer('SetHealth',%obj,%a,%b);

			//setdatablock
			case "8":
			commandToServer('SetDatablock',%obj,%a,%b);

			//freeze
			case "9":
			commandToServer('Freeze',%obj,%a,%b);

			//unfreeze
			case "10":
			commandToServer('unFreeze',%obj,%a);
			
			//freeze all
			case "11":
			commandToServer('Freeze',%obj,%a,%b);
			
			//toggle aoc (mute all non admins)
			case "12":
			commandToServer('Mute',%obj,%a,%b);
			
			case "13":
			commandToServer('Fetch',%obj,%a,%b);
			
			case "14":
			commandToServer('Find',%obj,%a,%b);
		}
	}

	// 1 == WARN
	// 2 == FORCESPAWN
	// 3 == FORCEKILL
	// 4 == MUTE
	// 5 == UNMUTE
	// 6 == SETSIZE
	// 7 == SETHEALTH
	// 8 == SETDATABLOCK
	// 9 == FREEZE
	// 10 == UNFREEZE
	// 11 == FREEZEALL
	// 12 == TOGGLEAOC (MUTE ALL NON ADMINS)

	function adminGui::Warn(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%reason = adminGui_Reason.getText();
		SM_doAction(1,%objID,%reason);
	}
	
	function adminGui::ForceSpawn(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%reason = adminGui_Reason.getText();
		SM_doAction(2,%objID,%reason);
	}
	
	function adminGui::KickPlayer(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%reason = adminGui_Reason.getText();
		SM_doAction(9,%objID,%reason);
	}
	
	function adminGui::ForceKill(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%reason = adminGui_Reason.getText();
		SM_doAction(3,%objID,%reason);
	}
	
	function adminGui::Mute(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%time = getWord(%words,0);
		%words = removeWord(%words,0);
		%reason = %words;
		SM_doAction(4,%objID,%time,%reason);
	}
	
	function adminGui::UnMute(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%reason = %words;
		SM_doAction(5,%objID,%reason);
	}
	
	function adminGui::SetSize(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%x = getWord(%words,0);
		%y = getWord(%words,1);
		%z = getWord(%words,2);
		SM_doAction(6,%objID,%x,%y,%z);
	}
	
	function adminGui::SetHealth(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%amount = getWord(%words,0);
		%words = removeWord(%words,0);
		%reason = %words;
		SM_doAction(7,%objID,%amount,%reason);
	}
	
	function adminGui::SetDatablock(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%datablock = getWord(%words,0);
		%words = removeWord(%words,0);
		%reason = %words;
		SM_doAction(8,%objID,%datablock,%reason);
	}
	
	function adminGui::Freeze(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%time = getWord(%words,0);
		%words = removeWord(%words,0);
		%reason = %words;
		SM_doAction(9,%objID,%time,%reason);
	}
	
	function adminGui::UnFreeze(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%reason = %words;
		SM_doAction(10,%objID,%reason);
	}
	
	function adminGui::FreezeAll(%this)
	{
		%objID = "-ALL-";
		%words = adminGui_Reason.getText();
		%reason = %words;
		SM_doAction(11,%objID,%reason);
	}
	
	function adminGui::ToggleAOC(%this)
	{
		%objID = "-A_ALL-";
		%words = adminGui_Reason.getText();
		%time = getWord(%words,0);
		%words = removeWord(%words,0);
		%reason = %words;
		SM_doAction(12,%objID,%time,%reason);
	}
	
	function adminGui::Fetch(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%reason = %words;
		SM_doAction(13,%objID,%reason);
	}
	
	function adminGui::Find(%this)
	{
		%objID = lstAdminPlayerList.getSelectedID();
		%words = adminGui_Reason.getText();
		%reason = %words;
		SM_doAction(14,%objID,%reason);
	}
	
	function adminGui::OpenHelpDlg(%this)
	{
		canvas.pushDialog(adminGuiHelpDlg);
	}
};
activatePackage(SM_Client);

function clientCmdreceiveSMULP()
{
	echo("SMC :- Received SMC UnLoad Packet...");
	SM_unLoadSMC();
}

function SM_HandshakeFailed()
{
	echo("SMC :- Handshake Failed.");
	newChatHUD_addLine("\c0ERROR\c6: This server isn't using \c5System_Management\c6.");
}

function clientCmdSendSMHandshake()
{
	echo("SMC :- Sending Handshake...");
	commandToServer('receiveSMHandshake');
	$SM::Client::HandshakeTimeout = schedule(1000,0,'SM_HandshakeFailed');
}

function clientCmdSMHandshakeSuccess()
{
	echo("SMC :- Handshake Success!");
	cancel($SM::Client::HandshakeTimeout);
	SM_LoadSMC();
	$ADMINGUIPLUSPLUS_loaded = 1;
	newChatHUD_addLine("\c6This server is using \c5System_Management\c6 by \c3Lt. Jamergaman\c6/\c3tkepahama\c6.");
}
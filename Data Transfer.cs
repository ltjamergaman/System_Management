function serverCmdReceiveSMHandshake(%client)
{
	echo("SM ::-- Received Handshake Packet...");
	echo("SM ::-- Sending Handshake Success Packet...");
	commandToClient(%client,'SMHandshakeSuccess');
	%client.sendPlayerListUpdate();
}
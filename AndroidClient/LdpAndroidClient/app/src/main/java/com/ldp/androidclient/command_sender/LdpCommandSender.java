package com.ldp.androidclient.command_sender;


import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpCommandSender {
    private LdpClient clientHandler;
    private LdpProtocolPacketFactory packetFactory;
    public LdpCommandSender() {
        clientHandler = LdpClient.getInstance();
        packetFactory = new LdpProtocolPacketFactory();
    }

    public void sendCommand(CommandType type) {
        LdpCommand command = packetFactory.setLdpCommand(type);
        LdpPacket commandPacket = packetFactory.buildPacket(command);
        clientHandler.getSendingChannel().send(commandPacket);
    }
}

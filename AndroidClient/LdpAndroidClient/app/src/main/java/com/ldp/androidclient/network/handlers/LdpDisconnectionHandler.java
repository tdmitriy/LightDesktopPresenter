package com.ldp.androidclient.network.handlers;

import android.util.Log;

import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpDisconnectionHandler implements ILdpPacketHandler {
    private LdpClient clientHandler;

    public LdpDisconnectionHandler() {
        clientHandler = LdpClient.getInstance();
    }

    @Override
    public void handle(LdpProtocol.LdpPacket packet) {
        switch (packet.getType()) {
            case DISCONNECT_REQUEST:
                switch (packet.getDisconnectRequest().getType()) {
                    case FROM_SERVER:
                        clientHandler.disconnect();
                        break;
                }
                break;
        }
    }

    @Override
    public void dispose() {
        clientHandler = null;
    }
}

package com.ldp.androidclient.network.handlers;

import android.util.Log;

import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpDisconnectHandler implements ILdpPacketHandler {
    private static final String TAG = "LdpDisconnectHandler";
    private LdpClient clientHandler = LdpClient.getInstance();
    @Override
    public void handle(LdpProtocol.LdpPacket packet) {
        switch (packet.getType()) {
            case DISCONNECT_REQUEST:
                clientHandler.disconnect();
                break;
        }
    }

    @Override
    public void dispose() {
        clientHandler = null;
    }
}

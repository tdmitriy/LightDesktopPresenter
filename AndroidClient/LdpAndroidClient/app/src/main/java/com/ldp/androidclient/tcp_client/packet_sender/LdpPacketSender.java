package com.ldp.androidclient.tcp_client.packet_sender;

import android.util.Log;

import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.ILdpClient;
import com.ldp.androidclient.tcp_client.LdpClient;

import java.io.IOException;

public class LdpPacketSender {
    private static final String TAG = "LdpPacketSender";
    private LdpClient clientHandle;

    public LdpPacketSender() {
        clientHandle = LdpClient.getInstance();
    }

    public void send(LdpProtocol.LdpPacket packet) {
        try {
            packet.writeDelimitedTo(clientHandle.getChannel().getOutputStream());
        } catch (IOException e) {
            clientHandle.disconnect();
            Log.e(TAG, e.getMessage());
        }
    }
}

package com.ldp.androidclient.tcp_client.packet_sender;

import android.util.Log;

import com.ldp.androidclient.disposable.ILdpDisposable;
import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.ILdpClient;
import com.ldp.androidclient.tcp_client.LdpClient;

import java.io.IOException;

public class LdpPacketSender implements ILdpDisposable {
    private static final String TAG = "LdpPacketSender";
    private LdpClient clientHandler;

    public LdpPacketSender() {
        clientHandler = LdpClient.getInstance();
    }

    public void send(LdpProtocol.LdpPacket packet) {
        try {
            packet.writeDelimitedTo(clientHandler.getSocketChannel().getOutputStream());
            //Log.i(TAG, "Sending packet: " + packet.getType());
        } catch (IOException e) {
            clientHandler = null;
            Log.i(TAG, "" + e.getMessage());
        }
    }

    @Override
    public void dispose() {
        clientHandler = null;
    }
}

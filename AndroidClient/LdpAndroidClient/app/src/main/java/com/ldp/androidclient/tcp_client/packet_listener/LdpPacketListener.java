package com.ldp.androidclient.tcp_client.packet_listener;

import android.util.Log;

import com.ldp.androidclient.network.packet_handler.LdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.tcp_client.ILdpClient;
import com.ldp.androidclient.tcp_client.LdpClient;

import java.io.IOException;

public class LdpPacketListener extends LdpPacketHandler implements Runnable {
    private static final String TAG = "LdpClientPacketListener";
    private final LdpClient clientHandle;
    private boolean thread_working = false;

    public LdpPacketListener() {
        clientHandle = LdpClient.getInstance();
        thread_working = true;
    }

    @Override
    protected void handle() {
        try {
            LdpPacket packet = LdpPacket.parseDelimitedFrom(clientHandle.getChannel().getInputStream());

            //base
            notifyToAllListeners(packet);
        } catch (IOException e) {
            thread_working = false;
            clientHandle.disconnect();
            Log.e(TAG,  e.getMessage());
        }
    }

    @Override
    public void run() {
        while(thread_working) {
            handle();
        }
        Log.d(TAG, "Exiting working thread.");
    }
}

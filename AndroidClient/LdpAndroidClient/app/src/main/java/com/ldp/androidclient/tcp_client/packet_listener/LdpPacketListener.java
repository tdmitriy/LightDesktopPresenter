package com.ldp.androidclient.tcp_client.packet_listener;

import android.util.Log;

import com.ldp.androidclient.network.LdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
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

    public boolean isThreadAlive() {
        return thread_working;
    }

    public void stopWorkingThread() {
        thread_working = false;
    }

    @Override
    protected void handle() {
        try {
            LdpPacket packet = LdpPacket.parseDelimitedFrom(clientHandle.getSocketChannel()
                    .getInputStream());

            //base
            notifyToAllListeners(packet);
        } catch (IOException e) {
            thread_working = false;
            clientHandle.disconnect();
            Log.i(TAG,  "" + e.getMessage());
        }
    }

    @Override
    public void run() {
        while(thread_working) {
            handle();
        }
        Log.i(TAG, "Exiting working thread.");
    }
}

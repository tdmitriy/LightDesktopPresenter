package com.ldp.androidclient.tcp_client.packet_listener;

import android.util.Log;

import com.ldp.androidclient.network.LdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.tcp_client.LdpClient;

import java.io.IOException;

public class LdpPacketListener extends LdpPacketHandler implements Runnable {
    private static final String TAG = "LdpClientPacketListener";
    private LdpClient clientHandler;
    private boolean thread_working = false;

    public LdpPacketListener() {
        clientHandler = LdpClient.getInstance();
        thread_working = true;
    }

    @Override
    protected void handle() {
        try {
            LdpPacket packet = LdpPacket.parseDelimitedFrom(clientHandler.getSocketChannel()
                    .getInputStream());

            //base
            notifyToAllListeners(packet);
        } catch (IOException e) {
            thread_working = false;
            clientHandler.disconnect();
            //Log.i(TAG,  "" + e);
        } catch (Exception ex) {
            thread_working = false;
            clientHandler.disconnect();
            //Log.i(TAG,  "" + ex);
        }
    }

    @Override
    public void dispose() {
        super.dispose();
        thread_working = false;
        clientHandler = null;
    }

    @Override
    public void run() {
        while(thread_working) {
            handle();
        }
        Log.i(TAG, "Exiting packetListening thread.");
    }
}

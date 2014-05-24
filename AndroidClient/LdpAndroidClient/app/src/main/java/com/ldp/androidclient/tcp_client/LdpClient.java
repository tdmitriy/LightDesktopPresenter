package com.ldp.androidclient.tcp_client;

import android.content.Context;
import android.util.Log;
import com.ldp.androidclient.network.packet_handler.LdpConnectionHandler;
import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.packet_listener.LdpPacketListener;
import com.ldp.androidclient.tcp_client.packet_sender.LdpPacketSender;
import com.ldp.androidclient.utils.controls.LdpMessageBox;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.io.IOException;
import java.net.Socket;

// Singleton pattern impl

public class LdpClient implements ILdpClient {
    private static final String TAG = "LdpClient error: ";
    private static LdpClient instance;
    private static Context context;
    private Socket channel;

    private LdpPacketListener packetListener;
    private LdpPacketSender packetSender;

    private LdpConnectionPreferences prefs;
    private LdpProtocol.ConnectionType type;

    public static void initSingleInstance(Context context) {
        if(instance == null) {
            instance = new LdpClient();
            LdpClient.context = context;
        }
    }

    public synchronized static LdpClient getInstance() {
        return instance;
    }

    private LdpClient() {

    }


    public void initSettings(LdpConnectionPreferences prefs, LdpProtocol.ConnectionType type) {
        this.prefs = prefs;
        this.type = type;
    }
    @Override
    public void connect(String ipAddress, int port) {
        try {
            disconnect();
            channel = new Socket(ipAddress, port);
            addClientHandlers();
        } catch (IOException e) {
            disconnect();
            Log.e(TAG, e.getMessage());
            LdpMessageBox.show(context, "Unable to connect to: " + ipAddress,
                    LdpMessageBox.DialogType.ERROR);
        }
    }


    @Override
    public void disconnect() {
        if (channel != null && channel.isConnected()) {
            try {
                channel.close();
                channel = null;
            } catch (IOException e) {
                Log.e(TAG, e.getMessage());
            }
        }

        if (packetListener != null && packetSender != null) {
            packetListener = null;
            packetSender = null;
        }
    }


    private void addClientHandlers() {
        packetSender = new LdpPacketSender();
        packetListener = new LdpPacketListener();

        packetListener.addListener(new LdpConnectionHandler(context, prefs, type));
    }

    @Override
    public LdpPacketListener getPacketListener() {
        return packetListener;
    }

    @Override
    public LdpPacketSender getPacketSender() {
        return packetSender;
    }

    @Override
    public Socket getChannel() {
        return channel;
    }
}

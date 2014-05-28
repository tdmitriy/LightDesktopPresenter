package com.ldp.androidclient.tcp_client;

import android.os.AsyncTask;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.tcp_client.packet_listener.LdpPacketListener;
import com.ldp.androidclient.tcp_client.packet_sender.LdpPacketSender;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.net.Socket;

public abstract class LdpClientBase extends LdpClientInitializator implements ILdpClient {
    private static final int DEFAULT_PORT = 9998;

    @Override
    public void initSettings(LdpConnectionPreferences prefs, ConnectionType type) {
        super.initSettings(prefs, type);
    }

    @Override
    public boolean connect(int port) {
        return super.connect(port);
    }

    @Override
    public boolean connect() {
        return connect(DEFAULT_PORT);
    }


    @Override
    public void disconnect() {
        super.disconnect();
    }

    @Override
    public String getClientIpAddress() {
        return getSocketChannel().getLocalAddress().toString();
    }

    @Override
    public LdpPacketListener getListenerChannel() {
        return super.getListenerChannel();
    }

    @Override
    public LdpPacketSender getSendingChannel() {
        return super.getSendingChannel();
    }

    @Override
    public Socket getSocketChannel() {
        return super.getSocketChannel();
    }



}

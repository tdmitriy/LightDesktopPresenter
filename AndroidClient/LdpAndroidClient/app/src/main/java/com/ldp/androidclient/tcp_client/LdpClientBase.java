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
        super.initClientSettings(prefs, type);
    }

    @Override
    public boolean connect(int port) {
        return super.connectToServer(port);
    }

    @Override
    public boolean connect() {
        return super.connectToServer(DEFAULT_PORT);
    }


    @Override
    public void disconnect() {
        super.disconnectFromServer();
    }

    @Override
    public String getClientIpAddress() {
        return getSocketChannel().getLocalAddress().toString();
    }

    @Override
    public LdpPacketListener getListenerChannel() {
        return super.getClientListenerChannel();
    }

    @Override
    public LdpPacketSender getSendingChannel() {
        return super.getClientSendingChannel();
    }

    @Override
    public Socket getSocketChannel() {
        return super.getClientSocketChannel();
    }



}

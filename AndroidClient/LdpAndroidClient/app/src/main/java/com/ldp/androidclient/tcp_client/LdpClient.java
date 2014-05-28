package com.ldp.androidclient.tcp_client;

import android.content.Context;
import android.util.Log;

import com.ldp.androidclient.network.handlers.LdpConnectHandler;
import com.ldp.androidclient.network.handlers.LdpDisconnectHandler;
import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.packet_listener.LdpPacketListener;
import com.ldp.androidclient.tcp_client.packet_sender.LdpPacketSender;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.io.IOException;
import java.net.Socket;

// Singleton pattern impl
public class LdpClient extends LdpClientBase {
    private static LdpClient instance;

    public static void initSingleInstance() {
        if (instance == null) {
            instance = new LdpClient();
        }
    }

    public synchronized static LdpClient getInstance() {
        return instance;
    }

    private LdpClient() {
    }
}

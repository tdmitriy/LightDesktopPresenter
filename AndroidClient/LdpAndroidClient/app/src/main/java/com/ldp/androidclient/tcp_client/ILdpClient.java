package com.ldp.androidclient.tcp_client;

import android.app.Activity;
import android.content.Context;

import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

public interface ILdpClient extends ILdpClientHandlers {
    void initSettings(LdpConnectionPreferences prefs,
                      LdpProtocol.ConnectionType type);

    boolean connect(int port);

    boolean connect();

    void disconnect();
}

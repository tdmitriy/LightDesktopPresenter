package com.ldp.androidclient.tcp_client;

public interface ILdpClient extends ILdpClientHandlers {
    void connect(String ipAddress, int port);
    void disconnect();
}

package com.ldp.androidclient.tcp_client;

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

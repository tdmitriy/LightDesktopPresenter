package com.ldp.androidclient.tcp_client;

import android.util.Log;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.network.handlers.LdpAuthHandler;
import com.ldp.androidclient.network.handlers.LdpDisconnectionHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.packet_listener.LdpPacketListener;
import com.ldp.androidclient.tcp_client.packet_sender.LdpPacketSender;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;

public abstract class LdpClientInitializator {
    private static final String TAG = "LdpClientInitializator";
    private static final int CONNECTION_TIMEOUT = 6000;

    private LdpPacketListener packetListener;
    private LdpPacketSender packetSender;
    private Socket channel;

    private LdpAuthHandler authHandler;
    private LdpDisconnectionHandler disconnectionHandler;

    private LdpConnectionPreferences prefs;
    private ConnectionType type;
    private Thread packetListenerThread;

    private boolean settingsInitialized = false;

    protected void initClientSettings(LdpConnectionPreferences prefs, ConnectionType type) {
        this.prefs = prefs;
        this.type = type;
        settingsInitialized = true;
    }

    protected boolean connectToServer(int port) {
        String ipAddress;
        try {
            if (!settingsInitialized) {
                Log.i(TAG, "initSettings is not initialized.");
                LdpConnectionProgressDialog.dismiss();
                return false;
            }
            ipAddress = prefs.getIPAddress();
            SocketAddress serverAddress = new InetSocketAddress(ipAddress, port);

            channel = new Socket();
            channel.connect(serverAddress, CONNECTION_TIMEOUT);
            addClientHandlers();
            //LdpConnectionProgressDialog.dismiss();
            return true;
        } catch (IOException e) {
            Log.i(TAG, "" + e.getMessage());
            showErrorDialog(ErrorType.NO_CONNECTION);
            disconnectFromServer();
            return false;
        }
    }

    private void addClientHandlers() {
        packetSender = new LdpPacketSender();
        packetListener = new LdpPacketListener();

        if(packetListenerThread != null) {
            packetListenerThread.interrupt();
        }
        packetListenerThread = new Thread(packetListener);
        packetListenerThread.start();

        authHandler = new LdpAuthHandler(type);
        disconnectionHandler = new LdpDisconnectionHandler();

        packetListener.addListener(authHandler);
        packetListener.addListener(disconnectionHandler);

        try {
            Thread.sleep(500);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    private void sendDisconnectionPacket() {
        try {
            if (packetSender != null) {
                LdpProtocolPacketFactory factory = new LdpProtocolPacketFactory();
                LdpDisconnectRequest request = factory.setDisconnectRequest(DisconnectionType.FROM_SERVER);
                LdpPacket disconnPacket = factory.buildPacket(request);
                packetSender.send(disconnPacket);
                Log.i(TAG, "sendDisconnectionPacket called.");
            }

        } catch (Exception ex) {
            Log.i(TAG, "sendDisconnectionPacket error.");
        }
    }

    protected void disconnectFromServer() {
        sendDisconnectionPacket();
        if (channel != null) {
            try {
                channel.close();
            } catch (Exception e) {
                Log.i(TAG, e.getMessage());
            }
        }

        if (packetListener != null) {
            packetListener.dispose();
            packetListener = null;
        }

        if (packetSender != null) {
            packetSender.dispose();
            packetSender = null;
        }

        channel = null;
    }

    private void showErrorDialog(final ErrorType type) {
        LdpActivityMain.getMainActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                switch (type) {
                    case DISCONNECTED:
                        LdpMessageBox.show(LdpActivityMain.getContext(), "Disconnected", LdpMessageBox.DialogType.ERROR);
                        break;
                    case NO_CONNECTION:
                        LdpConnectionProgressDialog.dismiss();
                        LdpMessageBox.show(LdpActivityMain.getContext(), "Unable to connect to: " + prefs.getIPAddress(),
                                LdpMessageBox.DialogType.ERROR);
                        break;

                }
            }
        });

    }

    protected Socket getClientSocketChannel() {
        return channel;
    }

    protected LdpPacketListener getClientListenerChannel() {
        return packetListener;
    }

    protected LdpPacketSender getClientSendingChannel() {
        return packetSender;
    }

    private enum ErrorType {
        DISCONNECTED,
        NO_CONNECTION
    }
}

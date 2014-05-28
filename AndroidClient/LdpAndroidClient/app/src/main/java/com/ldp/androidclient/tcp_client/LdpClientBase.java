package com.ldp.androidclient.tcp_client;

import android.app.Activity;
import android.util.Log;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.disposable.ILdpDisposable;
import com.ldp.androidclient.network.handlers.LdpConnectHandler;
import com.ldp.androidclient.network.handlers.LdpDisconnectHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.packet_listener.LdpPacketListener;
import com.ldp.androidclient.tcp_client.packet_sender.LdpPacketSender;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;

public abstract class LdpClientBase implements ILdpClient, ILdpDisposable {
    private static final String TAG = "LdpClientBase error: ";
    public static final int CONNECTION_TIMEOUT = 6000;
    private static final int DEFAULT_PORT = 9998;

    private LdpPacketListener packetListener;
    private LdpPacketSender packetSender;
    private Socket channel;

    private LdpConnectionPreferences prefs;
    private ConnectionType type;

    private boolean settingsInitialized = false;
    private Thread packetListenerThread;

    @Override
    public void initSettings(LdpConnectionPreferences prefs, ConnectionType type) {

        this.prefs = prefs;
        this.type = type;
        settingsInitialized = true;
    }

    @Override
    public boolean connect(int port) {
        String ipAddress;
        try {
            disconnect();
            if (!settingsInitialized) {
                Log.i(TAG, "initSettings is not initialized.");
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
            //showErrorDialog(ErrorType.NO_CONNECTION);
            disconnect();
            return false;
        }
    }

    @Override
    public boolean connect() {
        return connect(DEFAULT_PORT);
    }

    private void addClientHandlers() {
        packetSender = new LdpPacketSender();
        packetListener = new LdpPacketListener();

        packetListener.addListener(new LdpConnectHandler(type));
        packetListener.addListener(new LdpDisconnectHandler());

        packetListenerThread = new Thread(packetListener);
        packetListenerThread.start();
    }

    @Override
    public void disconnect() {
        if (channel != null) {
            try {
                sendDisconnectionPacket();
                //showErrorDialog(ErrorType.DISCONNECTED);
                channel.close();
                channel = null;
            } catch (Exception e) {
                Log.i(TAG, e.getMessage());
            }
        }

        if (packetListener != null) {
            packetListener.removeListeners();
            packetListener.stopWorkingThread();
            if (packetListenerThread != null && packetListenerThread.isAlive())
                packetListenerThread.interrupt();
        }

        packetListener = null;
        packetSender = null;
    }


    private void sendDisconnectionPacket() {
        try {
            if (getSendingChannel() != null) {
                LdpProtocolPacketFactory factory = new LdpProtocolPacketFactory();
                LdpDisconnectRequest request = factory.setDisconnectRequest(DisconnectionType.FROM_SERVER);
                LdpPacket disconPacket = factory.buildPacket(request);
                getSendingChannel().send(disconPacket);
            }
        } catch (Exception ex) {
            Log.i(TAG, "sendDisconnectionPacket error.");
        }
    }

    private void showErrorDialog(final ErrorType type) {
        /*LdpActivityMain.getContext().runOnUiThread(new Runnable() {
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
        });*/
    }


    private enum ErrorType {
        DISCONNECTED,
        NO_CONNECTION
    }

    @Override
    public LdpPacketListener getListenerChannel() {
        return packetListener;
    }

    @Override
    public LdpPacketSender getSendingChannel() {
        return packetSender;
    }

    @Override
    public Socket getSocketChannel() {
        return channel;
    }

    @Override
    public void dispose() {
        if (packetListener != null)
            packetListener.dispose();
        if(packetListenerThread != null)
            packetListenerThread.interrupt();
        packetListener = null;
        packetSender = null;
        channel = null;
        prefs = null;

    }

}

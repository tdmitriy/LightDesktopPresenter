package com.ldp.androidclient.network.handlers;

import android.util.Log;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.utils.LdpClientInfo;

public class LdpAuthHandler implements ILdpPacketHandler {
    private static final String TAG = "LdpAuthHandler";
    private LdpProtocolPacketFactory packetFactory;
    private LdpClient clientHandler;
    private LdpClientInfoRequest clientInfoRequest;
    private LdpPreparableInfoRequest preparableInfoRequest;
    private LdpPacket packet;
    private static final int DELEY = 150;

    private LdpPreparableInfoHandler preparableInfoHandler;

    private ConnectionType type;

    public LdpAuthHandler(ConnectionType type) {
        clientHandler = LdpClient.getInstance();
        this.type = type;
        packetFactory = new LdpProtocolPacketFactory();
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case AUTH_RESPONSE:
                LdpAuthResponse authResponse = packet.getAuthResponse();
                processAuthResponse(authResponse.getIsSuccess());
                break;
        }
    }

    private void processAuthResponse(boolean success) {
        Log.i(TAG, "processAuthResponse: " + success);
        if (success) {
            showDialogInfo(InfoType.AUTH_SUCCESS);
            showDialogInfo(InfoType.SEND_CLIENT_INFO);

            sendClientInfoRequest();
            showDialogInfo(InfoType.SEND_PREPARABLE_INFO);

            preparableInfoHandler = new LdpPreparableInfoHandler();
            clientHandler.getListenerChannel().addListener(preparableInfoHandler);

            sendPreparableDesktopInfoRequest();

            showDialogInfo(InfoType.WAIT_PREPARABLE_INFO);

            clientHandler.getListenerChannel().removeListener(this);
        } else {
            showDialogInfo(InfoType.WRONG_PASSWORD);
            Log.i(TAG, "WRONG_PASSWORD");
            clientHandler.disconnect();
            Log.i(TAG,  "Send clientHandler.disconnect()");
        }
    }

    private void sendClientInfoRequest() {
        clientInfoRequest = packetFactory.setClientInfoRequest(
                clientHandler.getClientIpAddress(),
                LdpClientInfo.getOs(),
                LdpClientInfo.getDeviceName()
        );

        packet = null;
        packet = packetFactory.buildPacket(clientInfoRequest);
        clientHandler.getSendingChannel().send(packet);
    }

    private void sendPreparableDesktopInfoRequest() {
        preparableInfoRequest = packetFactory.setPreparableInfoRequest(type);
        packet = null;
        packet = packetFactory.buildPacket(preparableInfoRequest);
        clientHandler.getSendingChannel().send(packet);
    }

    private void sendPreparableVolumeInfoRequest() {
        // TODO sendPreparableVolumeInfoRequest
    }

    private void showDialogInfo(final InfoType infoType) {
        LdpActivityMain.getMainActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                switch (infoType) {
                    case AUTH_SUCCESS:
                        LdpConnectionProgressDialog.changeMessage("Auth success..");
                        deley();
                        break;
                    case SEND_CLIENT_INFO:
                        LdpConnectionProgressDialog.changeMessage("Sending client info..");
                        deley();
                        break;
                    case SEND_PREPARABLE_INFO:
                        LdpConnectionProgressDialog.changeMessage("Sending preparable info..");
                        deley();
                        break;
                    case WAIT_PREPARABLE_INFO:
                        LdpConnectionProgressDialog.changeMessage("Waiting preparable info..");
                        deley();
                        break;
                    case WRONG_PASSWORD:
                        LdpMessageBox.show(LdpActivityMain.getContext(), "Wrong password", LdpMessageBox.DialogType.ERROR);
                        LdpConnectionProgressDialog.dismiss();
                        break;
                }
            }
        });
    }

    private void deley() {
        try {
            Thread.sleep(DELEY);
        } catch (InterruptedException ignored) {
        }
    }

    private enum InfoType {
        AUTH_SUCCESS,
        SEND_CLIENT_INFO,
        SEND_PREPARABLE_INFO,
        WAIT_PREPARABLE_INFO,

        WRONG_PASSWORD,

        RECEIVED_PREPARABLE_INFO
    }

    @Override
    public void dispose() {
        packetFactory = null;
        clientHandler = null;
        clientInfoRequest = null;
        preparableInfoRequest = null;
    }
}

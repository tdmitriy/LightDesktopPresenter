package com.ldp.androidclient.network.handlers;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.activities.LdpActivityRemoteDesktop;
import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.utils.LdpClientInfo;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;


public class LdpConnectHandler implements ILdpPacketHandler {
    private static final String TAG = "LdpConnectHandler";
    private LdpProtocolPacketFactory packetFactory;
    private LdpClient clientHandler;
    private final ConnectionType type;
    private Context context;

    public LdpConnectHandler(ConnectionType type) {
        this.context = LdpActivityMain.getContext();
        clientHandler = LdpClient.getInstance();
        this.type = type;
        packetFactory = new LdpProtocolPacketFactory();
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case AUTH_RESPONSE:
                processAuthResponse(packet);
                break;
            case PREPARABLE_INFO_RESPONSE:
                processPreparableResponse(packet);
                break;
        }
    }

    private void processAuthResponse(LdpPacket packet) {
        LdpAuthResponse authResponse = packet.getAuthResponse();

        boolean isSuccess = authResponse.getIsSuccess();
        Log.i(TAG, "processAuthResponse: " + isSuccess);
        if (isSuccess) {
            //showDialogInfo(InfoType.AUTH_SUCCESS);
            //showDialogInfo(InfoType.SEND_CLIENT_INFO);

            LdpClientInfoRequest clientInfo =
                    packetFactory.setClientInfoRequest(clientHandler
                                    .getSocketChannel()
                                    .getLocalAddress().toString(),
                            LdpClientInfo.getOs(),
                            LdpClientInfo.getDeviceName()
                    );

            LdpPacket clientInfoRequest = packetFactory.buildPacket(clientInfo);
            clientHandler.getSendingChannel().send(clientInfoRequest);

            //showDialogInfo(InfoType.SEND_PREPARABLE_INFO);

            LdpPreparableInfoRequest request = packetFactory.setPreparableInfoRequest(type);
            LdpPacket preparablePacket = packetFactory.buildPacket(request);
            clientHandler.getSendingChannel().send(preparablePacket);


            //showDialogInfo(InfoType.WAIT_PREPARABLE_INFO);
        } else {
            showDialogInfo(InfoType.WRONG_PASSWORD);
            Log.i(TAG, "WRONG_PASSWORD");
            clientHandler.disconnect();
        }
    }

    private void processPreparableResponse(LdpPacket packet) {

        LdpPreparableInfoResponse preparableResponse = packet.getPreparableInfoResponse();
        int desktopWidth = preparableResponse.getScreenWidth();
        int desktopHeight = preparableResponse.getScreenHeight();
        Log.i(TAG, "processAuthResponse: width=" + desktopWidth + " height=" + desktopHeight);

        //showDialogInfo(InfoType.RECEIVED_PREPARABLE_INFO);

        try {
            Intent intent = new Intent(context, LdpActivityRemoteDesktop.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.putExtra("RmDesktopWidth", desktopWidth);
            intent.putExtra("RmDesktopHeight", desktopHeight);
            context.startActivity(intent);
            Log.i(TAG, "Starting activity LdpActivityRemoteDesktop");
            clientHandler.getListenerChannel().removeListener(this);
        } catch (Exception ex) {
            Log.i(TAG, "starting LdpActivityRemoteDesktop error: " + ex.getMessage());
            clientHandler.disconnect();
        }
    }

    private void showDialogInfo(final InfoType infoType) {
        /*context.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                switch (infoType) {
                    case AUTH_SUCCESS:
                        LdpConnectionProgressDialog.changeMessage("Auth success..");
                        break;
                    case SEND_CLIENT_INFO:
                        LdpConnectionProgressDialog.changeMessage("Sending client info..");
                        break;
                    case SEND_PREPARABLE_INFO:
                        LdpConnectionProgressDialog.changeMessage("Sending preparable info..");
                        break;
                    case WAIT_PREPARABLE_INFO:
                        LdpConnectionProgressDialog.changeMessage("Waiting preparable info..");
                        break;
                    case WRONG_PASSWORD:
                        LdpMessageBox.show(context, "Wrong password", LdpMessageBox.DialogType.ERROR);
                        break;
                    case RECEIVED_PREPARABLE_INFO:
                        LdpConnectionProgressDialog.changeMessage("Preparable info received..");
                        LdpConnectionProgressDialog.dismiss();
                        break;
                }
            }
        });*/
    }

    @Override
    public void dispose() {
        clientHandler = null;
        packetFactory = null;
        clientHandler = null;
        context = null;
    }

    private enum InfoType {
        AUTH_SUCCESS,
        SEND_CLIENT_INFO,
        SEND_PREPARABLE_INFO,
        WAIT_PREPARABLE_INFO,

        WRONG_PASSWORD,

        RECEIVED_PREPARABLE_INFO
    }
}

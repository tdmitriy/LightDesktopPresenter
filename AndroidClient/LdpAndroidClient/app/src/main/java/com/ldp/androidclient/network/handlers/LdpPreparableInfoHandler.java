package com.ldp.androidclient.network.handlers;

import android.content.Intent;
import android.util.Log;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.activities.LdpActivityRemoteDesktop;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpPreparableInfoHandler implements ILdpPacketHandler {
    private static final String TAG = "LdpPreparableInfoHandler";

    private LdpClient clientHandler;
    private LdpProtocolPacketFactory packetFactory;
    private LdpPreparableInfoResponse preparableInfoResponse;

    public LdpPreparableInfoHandler() {

        clientHandler = LdpClient.getInstance();
        packetFactory = new LdpProtocolPacketFactory();
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case PREPARABLE_INFO_RESPONSE:
                processPreparableDesktopInfoResponse(packet);
                break;
        }
    }

    private void processPreparableDesktopInfoResponse(LdpPacket packet) {
        preparableInfoResponse = packet.getPreparableInfoResponse();
        int desktopWidth = preparableInfoResponse.getScreenWidth();
        int desktopHeight = preparableInfoResponse.getScreenHeight();
        Log.i(TAG, "processAuthResponse: width=" + desktopWidth + " height=" + desktopHeight);

        //showDialogInfo(InfoType.RECEIVED_PREPARABLE_INFO);

        try {
            Intent intent = new Intent(LdpActivityMain.getContext(), LdpActivityRemoteDesktop.class);
            //intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.putExtra("RmDesktopWidth", desktopWidth);
            intent.putExtra("RmDesktopHeight", desktopHeight);
            LdpActivityMain.getContext().startActivity(intent);
            Log.i(TAG, "Starting activity LdpActivityRemoteDesktop");
            showDialogInfo(InfoType.RECEIVED_PREPARABLE_INFO);
            clientHandler.getListenerChannel().removeListener(this);
        } catch (Exception ex) {
            Log.i(TAG, "starting LdpActivityRemoteDesktop error: " + ex.getMessage());
            clientHandler.disconnect();
            Log.i(TAG,  "Send clientHandler.disconnect()");
        }
    }

    private void processPreparableVolumeInfoResponse(LdpPacket packet) {
        // TODO processPreparableVolumeInfoResponse
    }

    private void showDialogInfo(final InfoType infoType) {
        LdpActivityMain.getMainActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                switch (infoType) {
                    case RECEIVED_PREPARABLE_INFO:
                        LdpConnectionProgressDialog.changeMessage("Preparable info received..");
                        LdpConnectionProgressDialog.dismiss();
                        break;
                }
            }
        });
    }

    private enum InfoType {
        RECEIVED_PREPARABLE_INFO
    }

    @Override
    public void dispose() {
        clientHandler = null;
        packetFactory = null;
        preparableInfoResponse = null;
    }
}

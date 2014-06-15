package com.ldp.androidclient.network.handlers;

import android.content.Intent;
import android.util.Log;

import com.ldp.androidclient.activities.LdpActivityMain;
import com.ldp.androidclient.activities.LdpActivityRemoteDesktop;
import com.ldp.androidclient.activities.LdpActivityVolumeControl;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpPreparableInfoHandler implements ILdpPacketHandler {
    private static final String TAG = "LdpPreparableInfoHandler";

    private LdpClient clientHandler;
    private LdpPreparableDesktopInfoResponse preparableInfoResponse;
    private LdpPreparableVolumeInfoResponse preparableVolumeInfoResponse;

    public LdpPreparableInfoHandler() {
        clientHandler = LdpClient.getInstance();
        clientHandler.getListenerChannel().addListener(this);
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case PREPARABLE_DESKTOP_INFO_RESPONSE:
                processPreparableDesktopInfoResponse(packet);
                break;
            case PREPARABLE_VOLUME_INFO_RESPONSE:
                processPreparableVolumeInfoResponse(packet);
                break;
        }
    }

    private void processPreparableDesktopInfoResponse(LdpPacket packet) {
        preparableInfoResponse = packet.getPreparableDesktopResponse();
        int desktopWidth = preparableInfoResponse.getScreenWidth();
        int desktopHeight = preparableInfoResponse.getScreenHeight();
        try {
            Intent intent = new Intent(LdpActivityMain.getContext(), LdpActivityRemoteDesktop.class);
            intent.putExtra("RmDesktopWidth", desktopWidth);
            intent.putExtra("RmDesktopHeight", desktopHeight);
            LdpActivityMain.getContext().startActivity(intent);

            showDialogInfo(InfoType.RECEIVED_PREPARABLE_INFO);
            clientHandler.getListenerChannel().removeListener(this);
            Log.i(TAG, "Starting activity LdpActivityRemoteDesktop");
        } catch (Exception ex) {
            Log.i(TAG, "starting LdpActivityRemoteDesktop error: " + ex.getMessage());
            clientHandler.disconnect();
        }
    }

    private void processPreparableVolumeInfoResponse(LdpPacket packet) {
        preparableVolumeInfoResponse = packet.getPreparableVolumeInfoResponse();
        int volume = preparableVolumeInfoResponse.getVolume();
        boolean mute = preparableVolumeInfoResponse.getIsMute();

        try {
            Intent intent = new Intent(LdpActivityMain.getContext(), LdpActivityVolumeControl.class);
            intent.putExtra("Volume", volume);
            intent.putExtra("Mute", mute);
            LdpActivityMain.getContext().startActivity(intent);

            showDialogInfo(InfoType.RECEIVED_PREPARABLE_INFO);
            clientHandler.getListenerChannel().removeListener(this);
            Log.i(TAG, "Starting activity LdpActivityVolumeControl");
        } catch (Exception ex) {
            Log.i(TAG, "starting LdpActivityVolumeControl error: " + ex.getMessage());
            clientHandler.disconnect();
        }
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
        preparableInfoResponse = null;
    }
}

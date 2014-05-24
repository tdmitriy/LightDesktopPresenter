package com.ldp.androidclient.authentication;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.widget.ListAdapter;
import com.ldp.androidclient.R;
import com.ldp.androidclient.network.packet_handler.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.utils.controls.LdpConnectionTypeAdapter;
import com.ldp.androidclient.utils.controls.LdpMessageBox;

public class LdpAuthResponseHandler implements ILdpPacketHandler {
    private final ListAdapter adapter;

    private final LdpProtocolPacketFactory packetFactory;
    private final Context context;
    private ConnectionType type;

    public LdpAuthResponseHandler(Context context) {
        this.context = context;
        packetFactory = new LdpProtocolPacketFactory();
        String[] items = new String[]{"Remote desktop control", "Remote volume control"};
        Integer[] icons = new Integer[]{R.drawable.remote_pc_icon, R.drawable.remote_volume_icon};
        adapter = new LdpConnectionTypeAdapter(context, items, icons);
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case AUTH_RESPONSE:
                LdpAuthResponse authResponse = packet.getAuthResponse();
                boolean isSuccess = authResponse.getIsSuccess();
                if (isSuccess) {
                    showConnectionTypeSelector();
                } else {
                    LdpMessageBox.show(context, "Wrong password", LdpMessageBox.DialogType.ERROR);
                }
                break;
        }
    }

    private void sendPreparableInfoRequest(ConnectionType type) {
        final LdpPreparableInfoRequest request = packetFactory
                .setPreparableInfoRequest(type);
        final LdpPacket response;
        switch (type) {
            case REMOTE_DESKTOP_CONTROL:
                response = packetFactory.buildPacket(request);
                //LdpPacketSender.send(response);
                break;
            case REMOTE_VOLUME_CONTROL:
                response = packetFactory.buildPacket(request);
                //LdpPacketSender.send(response);
                break;
        }
    }

    private void showConnectionTypeSelector() {
        new AlertDialog.Builder(context).setTitle("Select the type of control:")
                .setAdapter(adapter, new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int item) {
                        switch (item) {
                            case 0: //remote desktop
                                type = ConnectionType.REMOTE_DESKTOP_CONTROL;
                                sendPreparableInfoRequest(type);
                                break;
                            case 1: // remote volume
                                type = ConnectionType.REMOTE_VOLUME_CONTROL;
                                sendPreparableInfoRequest(type);
                                break;
                        }
                    }
                }).show();
    }
}

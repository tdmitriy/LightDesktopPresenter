package com.ldp.androidclient.connection_handler;

import android.content.Context;
import com.ldp.androidclient.network.packet_handler.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.utils.LdpClientInfo;
import com.ldp.androidclient.utils.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.utils.controls.LdpMessageBox;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;


public class LdpConnectionHandler implements ILdpPacketHandler {
    private final LdpProtocolPacketFactory packetFactory;
    private final LdpConnectionPreferences prefs;
    private final LdpClient clientHandler;
    private final Context context;
    private final ConnectionType type;
    private static final int WAITING_DELEY = 100;


    public LdpConnectionHandler(Context context, LdpConnectionPreferences prefs,
                                ConnectionType type) {
        clientHandler = LdpClient.getInstance();
        this.context = context;
        this.type = type;
        this.prefs = prefs;
        packetFactory = new LdpProtocolPacketFactory();
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case AUTH_RESPONSE:
                LdpAuthResponse authResponse = packet.getAuthResponse();
                boolean isSuccess = authResponse.getIsSuccess();
                if (isSuccess) {
                    LdpConnectionProgressDialog.changeMessage("Auth success..");
                    LdpConnectionProgressDialog.changeMessage("Sending client info..");
                    LdpClientInfoRequest clientInfo =
                            packetFactory.setClientInfoRequest(clientHandler
                                            .getSocketChannel()
                                            .getLocalAddress().toString(),
                                    LdpClientInfo.getOs(),
                                    LdpClientInfo.getDeviceName()
                            );


                    LdpPacket clientInfoRequest = packetFactory.buildPacket(clientInfo);
                    clientHandler.getSendingChannel().send(clientInfoRequest);

                    LdpConnectionProgressDialog.changeMessage("Sending preparable info..");
                    LdpPreparableInfoRequest request = packetFactory.setPreparableInfoRequest(type);
                    LdpPacket preparablePacket = packetFactory.buildPacket(request);
                    clientHandler.getSendingChannel().send(preparablePacket);


                    LdpConnectionProgressDialog.changeMessage("Waiting preparable info..");
                } else {
                    LdpMessageBox.show(context, "Wrong password", LdpMessageBox.DialogType.ERROR);
                }
                break;
            case PREPARABLE_INFO_RESPONSE:
                LdpPreparableInfoResponse preparableResponse = packet.getPreparableInfoResponse();
                int desktoWidth = preparableResponse.getScreenWidth();
                int desktopHeight = preparableResponse.getScreenHeight();

                LdpConnectionProgressDialog.changeMessage("Preparable info received..");
                LdpConnectionProgressDialog.dismiss();

                // TODO pass this 2 parameters to intent and start activity

                break;
        }
    }
}

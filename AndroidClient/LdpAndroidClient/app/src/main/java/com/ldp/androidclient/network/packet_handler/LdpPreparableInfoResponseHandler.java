package com.ldp.androidclient.network.packet_handler;

import android.content.Context;
import android.content.Intent;

import com.ldp.androidclient.protocol.LdpProtocol;

import java.io.Serializable;

public class LdpPreparableInfoResponseHandler implements ILdpPacketHandler, Serializable {
    private final Context context;
    public LdpPreparableInfoResponseHandler(Context context) {
        this.context = context;
    }
    @Override
    public void handle(LdpProtocol.LdpPacket packet) {
        switch (packet.getType()) {
            case PREPARABLE_INFO_RESPONSE:
                // TODO start new Activity (screen or volume)
                //Intent screenIntent = new Intent();
                break;
        }
    }
}

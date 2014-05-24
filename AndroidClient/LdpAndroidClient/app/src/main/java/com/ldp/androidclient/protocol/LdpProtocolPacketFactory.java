package com.ldp.androidclient.protocol;

import com.ldp.androidclient.protocol.LdpProtocol.*;

public class LdpProtocolPacketFactory {

    public LdpAuthRequest setAuthRequest(String password) {
        return LdpAuthRequest.newBuilder()
                .setPassword(password)
                .build();
    }

    public LdpClientInfoRequest setClientInfoRequest(String ip, String os, String devName) {
        return LdpClientInfoRequest.newBuilder()
                .setIP(ip)
                .setOS(os)
                .setDeviceName(devName)
                .build();
    }

    public LdpPreparableInfoRequest setPreparableInfoRequest(ConnectionType type) {
        return LdpPreparableInfoRequest.newBuilder()
                .setType(type)
                .build();
    }

    public LdpDisconnectRequest setDisconnectRequest(DisconnectionType type) {
        return LdpDisconnectRequest.newBuilder()
                .setType(type)
                .build();
    }

    public LdpPacket buildPacket(LdpAuthRequest builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.AUTH_REQUEST)
                .setAuthRequest(builder)
                .build();
    }

    public LdpPacket buildPacket(LdpClientInfoRequest builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.CLIENT_INFO_REQUEST)
                .setClientInfoRequest(builder)
                .build();
    }

    public LdpPacket buildPacket(LdpPreparableInfoRequest builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.PREPARABLE_INFO_REQUEST)
                .setPreparableInfoRequest(builder)
                .build();
    }

    public LdpPacket buildPacket(LdpDisconnectRequest builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.DISCONNECT_REQUEST)
                .setDisconnectRequest(builder)
                .build();
    }
}

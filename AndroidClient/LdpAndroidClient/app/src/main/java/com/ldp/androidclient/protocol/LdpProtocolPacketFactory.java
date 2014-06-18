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

    public LdpScreenRequest setScreenRequest() {
        return LdpScreenRequest.newBuilder().build();
    }

    public LdpMouseInfoResponse setMouseInfoResponse(int x, int y, MouseType type) {
        return LdpMouseInfoResponse.newBuilder()
                .setType(type)
                .setX(x)
                .setY(y)
                .build();
    }

    public LdpVolumeInfoResponse setVolumeInfoResponse(int volume) {
        return LdpVolumeInfoResponse.newBuilder()
                .setType(VolumeInfoType.VOLUME)
                .setVolume(volume)
                .build();
    }

    public LdpVolumeInfoResponse setVolumeInfoResponse(boolean mute) {
        return LdpVolumeInfoResponse.newBuilder()
                .setType(VolumeInfoType.MUTE)
                .setIsMute(mute)
                .build();
    }

    public LdpKeyboardInfoResponse setKeyboardInfoResponse(String text, KeyboardKey type) {
        return LdpKeyboardInfoResponse.newBuilder()
                .setType(type)
                .setKey(text)
                .build();
    }

    public LdpCommand setLdpCommand(CommandType type) {
        return LdpCommand.newBuilder()
                .setType(type)
                .build();
    }

    public LdpPacket buildPacket(LdpScreenRequest screenRequest) {
        return LdpPacket.newBuilder()
                .setType(PacketType.SCREEN_REQUEST)
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

    public LdpPacket buildPacket(LdpMouseInfoResponse builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.MOUSE_INFO_RESPONSE)
                .setMouseInfoResponse(builder)
                .build();
    }

    public LdpPacket buildPacket(LdpVolumeInfoResponse builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.VOLUME_INFO_RESPONSE)
                .setVolumeInfoResponse(builder)
                .build();
    }

    public LdpPacket buildPacket(LdpKeyboardInfoResponse builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.KEYBOARD_INFO_RESPONSE)
                .setKeyboardInfoResponse(builder)
                .build();
    }

    public LdpPacket buildPacket(LdpCommand builder) {
        return LdpPacket.newBuilder()
                .setType(PacketType.COMMAND)
                .setCommand(builder)
                .build();
    }
}

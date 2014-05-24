package com.ldp.androidclient.network.packet_handler;

import com.ldp.androidclient.protocol.LdpProtocol.*;

public interface ILdpPacketHandler {
    void handle(LdpPacket packet);
}

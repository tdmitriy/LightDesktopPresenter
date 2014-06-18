package com.ldp.androidclient.network;

import com.ldp.androidclient.disposable.ILdpDisposable;
import com.ldp.androidclient.protocol.LdpProtocol.*;

public interface ILdpPacketHandler extends ILdpDisposable {
    void handle(LdpPacket packet);
}

package com.ldp.androidclient.network;


import com.ldp.androidclient.protocol.*;

public interface ILdpObservable {
    void addListener(ILdpPacketHandler listener);
    void removeListener(ILdpPacketHandler listener);
    void removeListeners();
    void notifyToAllListeners(LdpProtocol.LdpPacket packet);
}

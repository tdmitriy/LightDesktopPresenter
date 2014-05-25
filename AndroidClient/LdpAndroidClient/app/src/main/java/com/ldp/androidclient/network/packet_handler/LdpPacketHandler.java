package com.ldp.androidclient.network.packet_handler;


import com.ldp.androidclient.protocol.*;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.concurrent.CopyOnWriteArrayList;

abstract public class LdpPacketHandler implements ILdpObservable {
    private static final Object MUTEX = new Object();

    protected abstract void handle();

    private CopyOnWriteArrayList<ILdpPacketHandler> listeners;

    public LdpPacketHandler() {
        listeners = new CopyOnWriteArrayList<ILdpPacketHandler>();
    }

    @Override
    public void addListener(ILdpPacketHandler listener) {
        synchronized (MUTEX) {
            if (!listeners.contains(listener))
                listeners.add(listener);
        }
    }

    @Override
    public void notifyToAllListeners(LdpProtocol.LdpPacket packet) {
        if (listeners != null && listeners.size() != 0) {
            Iterator<ILdpPacketHandler> iterator = listeners.iterator();
            while (iterator.hasNext()) {
                ILdpPacketHandler lis = iterator.next();
                lis.handle(packet);
            }
        }
    }

    @Override
    public void removeListener(ILdpPacketHandler listener) {
        synchronized (MUTEX) {
            if (listeners.contains(listener))
                listeners.remove(listener);
        }
    }

    @Override
    public void removeListeners() {
        synchronized (MUTEX) {
            if (listeners != null) {
                listeners.clear();
                listeners = null;
            }
        }
    }
}

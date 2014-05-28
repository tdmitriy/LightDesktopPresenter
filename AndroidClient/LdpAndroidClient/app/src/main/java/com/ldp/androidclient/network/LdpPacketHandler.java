package com.ldp.androidclient.network;


import android.util.Log;

import com.ldp.androidclient.disposable.ILdpDisposable;
import com.ldp.androidclient.protocol.*;

import java.util.Iterator;
import java.util.concurrent.CopyOnWriteArrayList;

abstract public class LdpPacketHandler implements ILdpObservable, ILdpDisposable {
    private static final String TAG = "LdpPacketHandler";

    protected abstract void handle();

    private CopyOnWriteArrayList<ILdpPacketHandler> listeners;

    public LdpPacketHandler() {
        listeners = new CopyOnWriteArrayList<ILdpPacketHandler>();
    }

    @Override
    public void addListener(ILdpPacketHandler listener) {
        if (!listeners.contains(listener))
            listeners.add(listener);
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
        if (listeners.contains(listener)) {
            listener.dispose();
            listeners.remove(listener);
            Log.i(TAG, "removed listener: " + listener.toString());
        }
    }

    @Override
    public void removeListeners() {
        if (listeners != null && listeners.size() != 0) {
            Iterator<ILdpPacketHandler> iterator = listeners.iterator();
            while (iterator.hasNext()) {
                ILdpPacketHandler lis = iterator.next();
                lis.dispose();
            }
            listeners.clear();
            listeners = null;
        }
    }

    @Override
    public void dispose() {
        removeListeners();
        listeners = null;
    }
}

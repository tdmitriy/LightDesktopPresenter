package com.ldp.androidclient.tcp_client;

import com.ldp.androidclient.tcp_client.packet_listener.LdpPacketListener;
import com.ldp.androidclient.tcp_client.packet_sender.LdpPacketSender;

import java.net.Socket;

public interface ILdpClientHandlers {
    LdpPacketListener getPacketListener();
    LdpPacketSender getPacketSender();
    Socket getChannel();
}

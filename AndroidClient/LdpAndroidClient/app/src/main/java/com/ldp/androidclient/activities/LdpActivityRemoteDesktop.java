package com.ldp.androidclient.activities;

import android.app.Activity;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.os.StrictMode;
import android.view.ViewGroup;

import com.ldp.androidclient.network.packet_handler.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol;
import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.utils.controls.image_view.LdpRemoteImageView;

public class LdpActivityRemoteDesktop extends Activity implements ILdpPacketHandler {

    private LdpClient clientHandler = LdpClient.getInstance();

    private LdpRemoteImageView remoteImageView;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        permitNetworkThreadPolicy();

    }

    private void initializeContent(Bitmap background) {
        remoteImageView = new LdpRemoteImageView(this);
        int backgroundW = background.getWidth();
        int backgroundH = background.getHeight();
        remoteImageView.setLayoutParams(new ViewGroup.LayoutParams(backgroundW, backgroundH));
        remoteImageView.setImageBitmap(background);

        //cursorView = new MCursorView(this, bmp, imageView, client);
        //cursorView.setLayoutParams(new ViewGroup.LayoutParams(bmp.getWidth(), bmp.getHeight()));

        ViewGroup root = (ViewGroup) findViewById(android.R.id.content);
        root.addView(remoteImageView);
        //root.addView(cursorView);
    }

    //to avoid NetworkOnMainThreadException
    private void permitNetworkThreadPolicy() {
        if (android.os.Build.VERSION.SDK_INT > 9) {
            StrictMode.ThreadPolicy policy =
                    new StrictMode.ThreadPolicy.Builder().permitAll().build();
            StrictMode.setThreadPolicy(policy);
        }
    }

    private void sendScreenImageRequest() {

    }

    @Override
    public void handle(LdpProtocol.LdpPacket packet) {

    }
}

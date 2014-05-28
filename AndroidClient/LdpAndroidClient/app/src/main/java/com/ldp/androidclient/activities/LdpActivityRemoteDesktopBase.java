package com.ldp.androidclient.activities;

import android.app.Activity;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.os.StrictMode;
import android.view.ViewGroup;

import com.ldp.androidclient.controls.image_view.LdpRemoteImageView;

public class LdpActivityRemoteDesktopBase extends Activity {

    protected LdpRemoteImageView remoteImageView;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    protected void initializeContent(Bitmap background) {
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
}

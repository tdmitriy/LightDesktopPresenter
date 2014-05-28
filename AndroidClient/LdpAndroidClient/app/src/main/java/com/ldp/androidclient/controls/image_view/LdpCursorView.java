package com.ldp.androidclient.controls.image_view;

import android.content.Context;
import android.widget.ImageView;

public class LdpCursorView extends ImageView implements ICursorView {

    private float cursorX, cursorY;
    private float x_prev, y_prev;
    private int screenW, screenH;
    private int canvasWidth, canvasHeight;

    private int positionX = 0;
    private int positionY = 0;

    private LdpRemoteImageView remoteImageView;
    private Context context;

    public LdpCursorView(Context context) {
        super(context);
    }

    @Override
    public float getCursroX() {
        return 0;
    }

    @Override
    public float getCursorY() {
        return 0;
    }
}

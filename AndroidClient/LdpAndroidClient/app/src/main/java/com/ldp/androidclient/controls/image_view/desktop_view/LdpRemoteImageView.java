package com.ldp.androidclient.controls.image_view.desktop_view;

import android.content.Context;
import android.graphics.Canvas;
import android.util.AttributeSet;
import android.widget.ImageView;

public class LdpRemoteImageView extends ImageView implements IContentScrolling {


    public LdpRemoteImageView(Context context) {
        super(context);
    }

    public LdpRemoteImageView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public LdpRemoteImageView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public void scrollContentTo(int x, int y) {
        scrollTo(x, y);
    }
}

package com.ldp.androidclient.controls.image_view.desktop_view;

import android.content.Context;
import android.widget.ImageView;

public class LdpRemoteImageView extends ImageView implements IContentScrolling {
    public LdpRemoteImageView(Context context) {
        super(context);
    }

    @Override
    public void scrollContentTo(int x, int y) {
        scrollTo(x, y);
    }
}

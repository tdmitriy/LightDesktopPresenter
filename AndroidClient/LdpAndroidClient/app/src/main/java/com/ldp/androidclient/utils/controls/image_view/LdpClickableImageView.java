package com.ldp.androidclient.utils.controls.image_view;

import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.ImageView;

public class LdpClickableImageView extends ImageView {
    public LdpClickableImageView(Context context) {
        super(context);
    }

    public LdpClickableImageView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public LdpClickableImageView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public void setPressed(boolean pressed) {
        // If the parent is pressed, do not set to pressed.
        if (pressed && ((View) getParent()).isPressed()) {
            return;
        }
        super.setPressed(pressed);
    }
}

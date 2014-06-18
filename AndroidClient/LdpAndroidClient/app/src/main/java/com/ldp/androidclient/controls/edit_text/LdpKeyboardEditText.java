package com.ldp.androidclient.controls.edit_text;

import android.content.Context;
import android.util.AttributeSet;
import android.view.KeyEvent;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputConnection;
import android.view.inputmethod.InputConnectionWrapper;
import android.widget.EditText;

public class LdpKeyboardEditText extends EditText {

    public LdpKeyboardEditText(Context context) {
        super(context);
    }

    public LdpKeyboardEditText(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public LdpKeyboardEditText(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public InputConnection onCreateInputConnection(EditorInfo outAttrs) {
        return new LdpInputConnection(super.onCreateInputConnection(outAttrs), true);
    }

    private class LdpInputConnection extends InputConnectionWrapper {

        public LdpInputConnection(InputConnection target, boolean mutable) {
            super(target, mutable);
        }

        @Override
        public boolean sendKeyEvent(KeyEvent event) {
            return super.sendKeyEvent(event);
        }
    }
}

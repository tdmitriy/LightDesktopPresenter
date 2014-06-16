package com.ldp.androidclient.activities;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import com.ldp.androidclient.R;
import com.ldp.androidclient.controls.image_view.cursor_view.LdpCursorView;
import com.ldp.androidclient.controls.image_view.desktop_view.LdpRemoteImageView;

public class LdpActivityRemoteDesktopBase extends Activity {

    protected LdpRemoteImageView remoteImageView;
    protected LdpCursorView cursorView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        // hide statusbar of Android
        // could also be done later
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.remote_desktop_menu, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.show_keyboard_setting:
                showKeyboard();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    protected void initializeContent(Bitmap background) {
        remoteImageView = new LdpRemoteImageView(this);
        int backgroundW = background.getWidth();
        int backgroundH = background.getHeight();
        remoteImageView.setImageBitmap(background);

        remoteImageView.setLayoutParams(new ViewGroup.LayoutParams(backgroundW, backgroundH));
        remoteImageView.setImageBitmap(background);

        cursorView = new LdpCursorView(this, backgroundW, backgroundH, remoteImageView);
        cursorView.setLayoutParams(new ViewGroup.LayoutParams(backgroundW, backgroundH));

        /*Button settingsButton = new Button(this);
        settingsButton.setText("Settings");
        RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(
                RelativeLayout.LayoutParams.WRAP_CONTENT,
                RelativeLayout.LayoutParams.WRAP_CONTENT);
        settingsButton.setGravity(Gravity.BOTTOM);
        settingsButton.setLayoutParams(params);*/


        ViewGroup root = (ViewGroup) findViewById(android.R.id.content);
        root.addView(remoteImageView);
        root.addView(cursorView);
        //root.addView(settingsButton);
    }

    private void showKeyboard() {
        InputMethodManager keyboard =
                (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
        if (keyboard != null) {
            keyboard.toggleSoftInput(0, InputMethodManager.SHOW_IMPLICIT);
        }
    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        int keyAction = event.getAction();
        if(keyAction == KeyEvent.ACTION_DOWN)
        {
            char pressedKey = (char) event.getUnicodeChar();
            Log.i("Key", "" + pressedKey);
        }

       /* switch  (event.getKeyCode()) {
            case  KeyEvent.KEYCODE_D:
                return  true;
            case  KeyEvent.KEYCODE_HOME:
                return  true;
        }*/
        return super.dispatchKeyEvent(event);
    }
}

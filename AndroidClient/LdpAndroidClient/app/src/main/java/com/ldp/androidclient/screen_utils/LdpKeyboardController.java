package com.ldp.androidclient.screen_utils;

import android.content.Context;
import android.text.Editable;
import android.text.InputType;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;

import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpKeyboardController implements View.OnFocusChangeListener, TextWatcher {

    private static final String TAG = "LdpKeyboardController";
    private LdpClient clientHandler;
    private LdpProtocolPacketFactory packetFactory;
    private EditText editText;

    private static InputMethodManager imm;
    private boolean firstTimeLoad = true;

    public LdpKeyboardController(Context context, EditText editText) {
        this.editText = editText;

        this.editText.addTextChangedListener(this);
        //this.editText.setOnFocusChangeListener(this);
        //this.editText.setFocusable(false);

        imm = (InputMethodManager)
                context.getSystemService(Context.INPUT_METHOD_SERVICE);

        closeKeyboard();
        clientHandler = LdpClient.getInstance();
        packetFactory = new LdpProtocolPacketFactory();
    }


    public void showKeyboard() {
        editText.setFocusable(true);
        hideKeyboard();
        editText.requestFocusFromTouch();
        openKeyboard();
    }

    public void hideKeyboard() {
        editText.clearFocus();
        imm.restartInput(editText);
    }

    private void openKeyboard() {
        imm.showSoftInput(editText, InputMethodManager.SHOW_IMPLICIT);
    }

    public void closeKeyboard() {
        imm.hideSoftInputFromWindow(editText.getWindowToken(),
                InputMethodManager.HIDE_NOT_ALWAYS);
    }


    public void sendKeyboardInfoPacket(String text, KeyboardKey type) {
        LdpKeyboardInfoResponse response = packetFactory.setKeyboardInfoResponse(text, type);
        LdpPacket packetResponse = packetFactory.buildPacket(response);
        clientHandler.getSendingChannel().send(packetResponse);
    }

    @Override
    public void onFocusChange(View v, boolean hasFocus) {
        if (hasFocus) {
            if (!firstTimeLoad)
                openKeyboard();
            firstTimeLoad = false;
        }
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void afterTextChanged(Editable s) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {
        if (!editText.getText().toString().equals("")) {
            if (s.length() == 1) {
                Log.i("", s.toString());
                sendKeyboardInfoPacket(s.toString(), KeyboardKey.KEY_TEXT);
            } else {
                Log.i("Voice: ", s.toString());
            }
            editText.setText("");
        }
    }
}

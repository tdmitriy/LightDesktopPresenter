package com.ldp.androidclient.activities;


import android.app.Activity;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.os.StrictMode;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.widget.ListView;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.controls.LdpConnectionProgressDialog;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

public class LdpActivityMain extends LdpActivityMainInterface {
    private static final String TAG = "LdpActivityMain";
    private LdpClient clientHandler;
    private LdpProtocolPacketFactory packetFactory;
    private AlertDialog connectionTypeDialog;
    private static Context context;
    private static Activity mainActivity;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        clientHandler = LdpClient.getInstance();
        packetFactory = new LdpProtocolPacketFactory();
        permitNetworkThreadPolicy();
        context = this;
        mainActivity = this;
    }

    //to avoid NetworkOnMainThreadException
    private void permitNetworkThreadPolicy() {
        if (android.os.Build.VERSION.SDK_INT > 9) {
            StrictMode.ThreadPolicy policy =
                    new StrictMode.ThreadPolicy.Builder().permitAll().build();
            StrictMode.setThreadPolicy(policy);
        }
    }

    public static Context getContext() {
        return context;
    }

    public static Activity getMainActivity() {
        return mainActivity;
    }

    @Override
    protected void onListItemClick(ListView list, View v, int position, long id) {
        selectedPrefs = (LdpConnectionPreferences)
                list.getItemAtPosition(position);
        //connect(getSelectedPrefs(), ConnectionType.REMOTE_DESKTOP_CONTROL);
        showConnectionTypeDialog();

    }

    private void showConnectionTypeDialog() {
        connectionTypeDialog = new AlertDialog.Builder(this)
                .setTitle("Select type of connection")
                .setNegativeButton("Cancel", null)
                .setCancelable(false)
                .setAdapter(listViewConnectionTypePopulator, connectionClickListener)
                .show();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();

    }

    private void connect(LdpConnectionPreferences prefs, ConnectionType type) {
        clientHandler.initSettings(prefs, type);

        LdpClientThread connectionThread = new LdpClientThread();
        connectionThread.execute();
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {
            clientHandler.disconnect();
            Log.i(TAG, "Send clientHandler.disconnect()");
            return super.onKeyDown(keyCode, event);
        }
        return super.onKeyDown(keyCode, event);
    }

    private DialogInterface.OnClickListener connectionClickListener = new DialogInterface.OnClickListener() {
        @Override
        public void onClick(DialogInterface dialog, int which) {
            switch (which) {
                case 0:
                    connect(getSelectedPrefs(), ConnectionType.REMOTE_DESKTOP_CONTROL);
                    break;
                case 1:
                    connect(getSelectedPrefs(), ConnectionType.REMOTE_VOLUME_CONTROL);
                    break;
            }
        }
    };

    private final class LdpClientThread extends AsyncTask<Void, Void, Void> {
        boolean success = false;

        @Override
        protected void onPreExecute() {
            LdpConnectionProgressDialog.show(LdpActivityMain.getContext(),
                    "Connecting", "Please wait..");
        }

        @Override
        protected Void doInBackground(Void... params) {
            success = clientHandler.connect();
            if (success)
                sendAuthRequest();
            return null;
        }

        private void sendAuthRequest() {
            LdpConnectionPreferences prefs = getSelectedPrefs();
            String password = prefs.getPassword();
            Log.i(TAG, "Sending password=" + password);
            LdpAuthRequest authRequest = packetFactory.setAuthRequest(password);
            LdpPacket packet = packetFactory.buildPacket(authRequest);
            clientHandler.getSendingChannel().send(packet);
        }


        @Override
        protected void onPostExecute(Void aVoid) {
            Log.i(TAG, "Exiting AsyncTask connector.");
            super.onPostExecute(aVoid);
        }
    }
}

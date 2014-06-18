package com.ldp.androidclient.activities;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.TextView;

import com.ldp.androidclient.R;
import com.ldp.androidclient.controls.seek_arc.SeekArc;
import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpActivityVolumeControl extends Activity implements
        SeekArc.OnSeekArcChangeListener, CompoundButton.OnCheckedChangeListener, ILdpPacketHandler {

    private static final String TAG = "LdpActivityRemoteDesktop";
    private AlertDialog.Builder disconnectionDialog;

    private SeekArc volumeControl;
    private Switch muteControl;
    private TextView volumeProgress;

    private int volume;
    private boolean mute;

    private LdpClient clientHandler;
    private LdpPacket packetResponse;
    private LdpVolumeInfoResponse volumeInfoResponse;
    private LdpProtocolPacketFactory packetFactory;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_volume_control);
        initializeComponents();
    }

    private void initializeComponents() {
        clientHandler = LdpClient.getInstance();
        packetFactory = new LdpProtocolPacketFactory();

        clientHandler.getListenerChannel().addListener(this);

        volumeControl = (SeekArc) findViewById(R.id.volumeControl);
        volumeControl.setArcWidth(5);
        volumeControl.setProgressWidth(5);
        volumeControl.setRoundedEdges(true);
        volumeControl.setTouchInSide(true);
        volumeControl.invalidate();
        volumeControl.setOnSeekArcChangeListener(this);

        muteControl = (Switch) findViewById(R.id.muteControl);
        muteControl.setOnCheckedChangeListener(this);

        volumeProgress = (TextView) findViewById(R.id.lblVolumeProgress);

        Intent intent = getIntent();
        volume = intent.getExtras().getInt("Volume");
        mute = intent.getExtras().getBoolean("Mute");

        volumeControl.setProgress(volume);
        muteControl.setChecked(mute);
    }

    @Override
    public void onProgressChanged(SeekArc seekArc, int progress, boolean fromUser) {
        volumeProgress.setText(String.valueOf(progress));
    }

    @Override
    public void onStartTrackingTouch(SeekArc seekArc) {

    }

    @Override
    public void onStopTrackingTouch(SeekArc seekArc) {
        volume = seekArc.getProgress();
        sendVolumeInfoPacket(volume);
    }

    @Override
    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        volume = volumeControl.getProgress();
        mute = isChecked;
        sendVolumeInfoPacket(mute);
    }

    private void sendVolumeInfoPacket(int volume) {
        packetResponse = null;
        volumeInfoResponse = null;

        volumeInfoResponse = packetFactory.setVolumeInfoResponse(volume);
        packetResponse = packetFactory.buildPacket(volumeInfoResponse);
        clientHandler.getSendingChannel().send(packetResponse);
    }

    private void sendVolumeInfoPacket(boolean mute) {
        packetResponse = null;
        volumeInfoResponse = null;

        volumeInfoResponse = packetFactory.setVolumeInfoResponse(mute);
        packetResponse = packetFactory.buildPacket(volumeInfoResponse);
        clientHandler.getSendingChannel().send(packetResponse);
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case VOLUME_INFO_RESPONSE:
                switch (packet.getVolumeInfoResponse().getType()) {
                    case VOLUME:
                        setVolumeValueToControl(packet);
                        break;
                    case MUTE:
                        setMuteValueToControl(packet);
                        break;
                }
                break;
            case DISCONNECT_REQUEST:
                switch (packet.getDisconnectRequest().getType()) {
                    case FROM_VOLUME_THREAD:
                        showDisconnectionMessage();
                        break;
                }
                break;
        }
    }

    private void setVolumeValueToControl(LdpPacket packet) {
        volumeInfoResponse = packet.getVolumeInfoResponse();
        volume = volumeInfoResponse.getVolume();

        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                volumeControl.setProgress(volume);
            }
        });
    }

    private void setMuteValueToControl(LdpPacket packet) {
        volumeInfoResponse = packet.getVolumeInfoResponse();
        mute = volumeInfoResponse.getIsMute();

        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                muteControl.setChecked(mute);
            }
        });
    }

    private void showDisconnectionMessage() {
        try {
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    disconnectionDialog = new AlertDialog.Builder(LdpActivityVolumeControl.this);
                    disconnectionDialog.setCancelable(false);
                    disconnectionDialog.setTitle("Disconnected");
                    disconnectionDialog.setIcon(R.drawable.ic_action_error);
                    disconnectionDialog.setPositiveButton("Close", closeListener);
                    disconnectionDialog.show();
                }
            });

        } catch (Exception exc) {
            Log.i(TAG, "" + exc.getMessage());
            if (clientHandler != null)
                clientHandler.disconnect();
        }

    }

    private DialogInterface.OnClickListener closeListener =
            new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    switch (which) {
                        case DialogInterface.BUTTON_POSITIVE:
                            if (clientHandler != null) {
                                clientHandler.disconnect();
                            }
                            Log.i(TAG, "Server shutdown.");
                            finish();
                            break;
                    }
                }
            };

    private DialogInterface.OnClickListener disconnectListener =
            new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    switch (which) {
                        case DialogInterface.BUTTON_POSITIVE:
                            sendDisconnectRequestFromVolumeThread();
                            if (clientHandler != null) {
                                clientHandler.disconnect();
                                Log.i(TAG, "Send clientHandler.disconnect()");
                            }
                            finish();
                            break;
                        case DialogInterface.BUTTON_NEGATIVE:
                            break;
                    }
                }
            };

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {

            disconnectionDialog = new AlertDialog.Builder(this);
            disconnectionDialog.setCancelable(false);
            disconnectionDialog.setTitle("Close connection");
            disconnectionDialog.setMessage("Close connection and return to the main screen?");
            disconnectionDialog.setIcon(R.drawable.ic_action_warning);
            disconnectionDialog.setPositiveButton("Close", disconnectListener);
            disconnectionDialog.setNegativeButton("Cancel", disconnectListener);
            disconnectionDialog.show();
        }
        return super.onKeyDown(keyCode, event);
    }

    private void sendDisconnectRequestFromVolumeThread() {
        try {
            LdpDisconnectRequest request =
                    packetFactory.setDisconnectRequest(DisconnectionType.FROM_VOLUME_THREAD);
            LdpPacket disconnPacket = packetFactory.buildPacket(request);
            clientHandler.getSendingChannel().send(disconnPacket);
            Log.i(TAG, "sending disconnect request from volume thread.");
        } catch (Exception ignored) {
            finish();
        }
    }

    @Override
    public void dispose() {
        clientHandler = null;
        packetFactory = null;
        packetResponse = null;
        volumeInfoResponse = null;
    }
}

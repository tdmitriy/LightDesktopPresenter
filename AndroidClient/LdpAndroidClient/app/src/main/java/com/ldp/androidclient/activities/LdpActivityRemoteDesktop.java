package com.ldp.androidclient.activities;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.RectF;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;

import com.ldp.androidclient.R;
import com.ldp.androidclient.network.ILdpPacketHandler;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.screen_utils.LdpScreenScalingSettings;
import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.image_processing.LdpImageProcessor;

public class LdpActivityRemoteDesktop extends LdpActivityRemoteDesktopBase
        implements ILdpPacketHandler {

    private static final String TAG = "LdpActivityRemoteDesktop";
    private AlertDialog.Builder disconnectionDialog;

    private LdpClient clientHandler;
    private LdpScreenRequest screenRequest;
    private LdpScreenResponse screenResponse;
    private LdpPacket requestPacket;
    private LdpProtocolPacketFactory packetFactory;

    private boolean pausingScreenRequestSending = false;

    private LdpImageProcessor imageProcessor;

    private RectF scaledRect = new RectF(0, 0, 0, 0);
    private int left, top, right, bottom;
    private float scaleCoef;
    private Bitmap background;
    private Bitmap screen;

    private LdpScreenScalingSettings scalingSettings;
    private Canvas canvas;
    private Paint paint;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        clientHandler = LdpClient.getInstance();
        clientHandler.getListenerChannel().addListener(this);

        initializeObjects();
        prepareDrawableItems();
        Log.i(TAG, "Activity started: LdpActivityRemoteDesktop");
        sendScreenImageRequest();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

    private void initializeObjects() {
        try {
            packetFactory = new LdpProtocolPacketFactory();
            screenRequest = packetFactory.setScreenRequest();
            requestPacket = packetFactory.buildPacket(screenRequest);

            Intent intent = getIntent();
            int rmDesktopWidth = intent.getExtras().getInt("RmDesktopWidth");
            int rmDesktopHeight = intent.getExtras().getInt("RmDesktopHeight");
            scalingSettings = new LdpScreenScalingSettings(rmDesktopWidth, rmDesktopHeight);
            scaleCoef = scalingSettings.getScaleCoef();
            imageProcessor = new LdpImageProcessor(rmDesktopWidth, rmDesktopHeight, scaleCoef);
        } catch (Exception ex) {
            Log.i(TAG, "initializeObjects error: " + ex.getMessage());
            clientHandler.disconnect();
            finish();
        }

    }

    private void prepareDrawableItems() {
        try {
            background = Bitmap.createBitmap(scalingSettings.getCanvasWidth(),
                    scalingSettings.getCanvasHeight(), Bitmap.Config.ARGB_8888);

            canvas = new Canvas(background);
            initializeMainControls(background);

            paint = new Paint();
            paint.setStyle(Paint.Style.STROKE);
            paint.setAntiAlias(true);
            paint.setFilterBitmap(true);
            paint.setDither(true);
        } catch (Exception ex) {
            Log.i(TAG, "prepareDrawableItems error: " + ex.getMessage());
            clientHandler.disconnect();
            finish();
        }
    }

    private void sendScreenImageRequest() {
        if (!pausingScreenRequestSending)
            clientHandler.getSendingChannel().send(requestPacket);
    }

    @Override
    public void handle(LdpPacket packet) {
        switch (packet.getType()) {
            case SCREEN_RESPONSE:
                screenResponse = null;
                screenResponse = packet.getScreenResponse();
                imageProcessor.processRawData(screenResponse);
                drawScreenImage();

                sendScreenImageRequest();
                break;
            case DISCONNECT_REQUEST:
                switch (packet.getDisconnectRequest().getType()) {
                    case FROM_SCREEN_THREAD:
                        showDisconnectionMessage();
                        break;
                }
                break;
        }
    }

    private void drawScreenImage() {
        screen = null;
        screen = imageProcessor.getScaledScreen();
        scaledRect = imageProcessor.getScaledRect();
        canvas.drawBitmap(screen, null, scaledRect, paint);
        screen.recycle();

        left = (int) scaledRect.left;
        top = (int) scaledRect.top;
        right = (int) scaledRect.right;
        bottom = (int) scaledRect.bottom;

        remoteImageView.post(new Runnable() {
            @Override
            public void run() {
                remoteImageView.invalidate(left, top, right, bottom);
            }
        });
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {
            pausingScreenRequestSending = true;

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

    private void sendDisconnectRequestFromScreenThread() {
        try {
            pausingScreenRequestSending = true;
            LdpDisconnectRequest request =
                    packetFactory.setDisconnectRequest(DisconnectionType.FROM_SCREEN_THREAD);
            LdpPacket disconnPacket = packetFactory.buildPacket(request);
            clientHandler.getSendingChannel().send(disconnPacket);
            Log.i(TAG, "sending disconnect request from screen thread.");
        } catch (Exception ignored) {
            finish();
        }

    }


    private void showDisconnectionMessage() {
        try {
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    pausingScreenRequestSending = true;
                    disconnectionDialog = new AlertDialog.Builder(LdpActivityRemoteDesktop.this);
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
                            sendDisconnectRequestFromScreenThread();
                            if (clientHandler != null) {
                                clientHandler.disconnect();
                                Log.i(TAG, "Send clientHandler.disconnect()");
                            }
                            finish();
                            break;
                        case DialogInterface.BUTTON_NEGATIVE:
                            pausingScreenRequestSending = false;
                            sendScreenImageRequest();
                            break;
                    }
                }
            };

    @Override
    public void dispose() {
        pausingScreenRequestSending = false;
        clientHandler = null;
        screenRequest = null;
        screenResponse = null;
        requestPacket = null;
        packetFactory = null;
        scalingSettings = null;
        imageProcessor = null;
    }
}

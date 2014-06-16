package com.ldp.androidclient.controls.image_view.cursor_view;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.GestureDetector;
import android.view.MotionEvent;
import android.widget.ImageView;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.controls.image_view.desktop_view.LdpRemoteImageView;
import com.ldp.androidclient.image_processing.LdpImageProcessor;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.protocol.LdpProtocolPacketFactory;
import com.ldp.androidclient.tcp_client.LdpClient;

public class LdpCursorView extends ImageView implements ICursorView {

    private float canvasCursorX, canvasCursorY;
    private float desktopCursorX, desktopCursorY;
    private float x_prev, y_prev;
    private int screenW, screenH;
    private int canvasW, canvasH;

    private GestureDetector gestureDetector;
    private float coefX, coefY;
    private float rmdWidth, rmdHeight;

    private Bitmap cursor;

    private LdpRemoteImageView remoteImageView;
    private Context context;

    private Paint cursorPaint;

    private LdpClient clientHandler;
    private LdpProtocolPacketFactory packetFactory;
    private LdpMouseInfoResponse mouseInfoResponse;
    private LdpPacket packet;


    private boolean isDragging = false;

    public LdpCursorView(Context context) {
        super(context);
    }

    public LdpCursorView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public LdpCursorView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    public LdpCursorView(Context context, int canvasW, int canvasH, LdpRemoteImageView remoteImageView) {
        super(context);
        clientHandler = LdpClient.getInstance();
        packetFactory = new LdpProtocolPacketFactory();
        this.context = context;
        this.remoteImageView = remoteImageView;
        this.canvasW = canvasW;
        this.canvasH = canvasH;
        initializeComponents();
    }


    private void initializeComponents() {
        DisplayMetrics dm = getResources().getDisplayMetrics();
        screenW = dm.widthPixels;
        screenH = dm.heightPixels;

        canvasCursorX = screenW / 2;
        canvasCursorY = screenH / 2;

        gestureDetector = new GestureDetector(context, new GestureListener());
        rmdWidth = LdpImageProcessor.getRemoteDesktopWidth();
        rmdHeight = LdpImageProcessor.getRemoteDesktopHeight();
        calcCursorCoef();

        cursor = BitmapFactory.decodeResource(getResources(), R.drawable.cursor1);

        cursorPaint = new Paint();
        cursorPaint.setAntiAlias(true);
        cursorPaint.setFilterBitmap(true);
        cursorPaint.setDither(true);
    }

    private void calcCursorCoef() {
        if (rmdWidth != 0 && rmdHeight != 0) {
            coefX = rmdWidth / canvasW;
            coefY = rmdHeight / canvasH;
        }
    }

    private void computeScroll(float x, float y, MotionEvent event) {
        float new_x = canvasCursorX - (x - event.getX());
        if (new_x > 0 && new_x < canvasW) {
            canvasCursorX = new_x;
        }

        float new_y = canvasCursorY - (y - event.getY());
        if (new_y > 0 && new_y < canvasH) {
            canvasCursorY = new_y;
        }

        float offsetX;
        float offsetY;

        if (canvasCursorX < screenW / 2)
            offsetX = canvasCursorX;
        else if (canvasCursorX > canvasW - screenW / 2)
            offsetX = screenW - (canvasW - canvasCursorX);
        else
            offsetX = screenW / 2;

        if (canvasCursorY < screenH / 2)
            offsetY = canvasCursorY;
        else if (canvasCursorY > canvasH - screenH / 2)
            offsetY = screenH - (canvasH - canvasCursorY);
        else
            offsetY = screenH / 2;


        int scrollX = (int) (canvasCursorX - offsetX);
        int scrollY = (int) (canvasCursorY - offsetY);

        remoteImageView.scrollContentTo(scrollX, scrollY);
        scrollTo(scrollX, scrollY);
    }


    private static final long TIME_TO_MOVE_CURSOR = 90;
    private long startTimeToMove;
    private long durationMove;
    private boolean actionMoveCursor = false;

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        gestureDetector.onTouchEvent(event);
        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN:
                x_prev = event.getX();
                y_prev = event.getY();

                actionMoveCursor = true;
                startTimeToMove = System.currentTimeMillis();
                break;
            case MotionEvent.ACTION_UP:
                durationMove = System.currentTimeMillis() - startTimeToMove;
                actionMoveCursor = durationMove > TIME_TO_MOVE_CURSOR;
                break;
            case MotionEvent.ACTION_MOVE:

                durationMove = System.currentTimeMillis() - startTimeToMove;
                if (durationMove <= TIME_TO_MOVE_CURSOR) {
                    actionMoveCursor = false;
                } else {
                    actionMoveCursor = true;
                }


                if (actionMoveCursor) {
                    computeScroll(x_prev, y_prev, event);

                    invalidate();
                    x_prev = event.getX();
                    y_prev = event.getY();

                    desktopCursorX = canvasCursorX * coefX;
                    desktopCursorY = canvasCursorY * coefY;

                    sendCursorInfoPacket(MouseType.SET_CURSOR_POS);
                }
                break;
        }
        return true;
    }

    private void sendCursorInfoPacket(MouseType type) {
        mouseInfoResponse = null;
        packet = null;
        int x = getDesktopCursorX();
        int y = getDesktopCursorY();

        if (x < 0) x = 0;
        if (x > rmdWidth) x = (int) rmdWidth;

        if (y < 0) y = (int) rmdHeight;
        if (y > rmdHeight) y = (int) rmdHeight;

        mouseInfoResponse = packetFactory
                .setMouseInfoResponse(x, y, type);
        packet = packetFactory.buildPacket(mouseInfoResponse);
        clientHandler.getSendingChannel().send(packet);
    }

    @Override
    protected void onDraw(Canvas canvas) {
        canvas.drawBitmap(cursor, canvasCursorX, canvasCursorY, cursorPaint);
    }

    @Override
    public float getCanvasCursroX() {
        return canvasCursorX;
    }

    @Override
    public float getCanvasCursorY() {
        return canvasCursorY;
    }

    @Override
    public int getDesktopCursorX() {
        return (int) desktopCursorX;
    }

    @Override
    public int getDesktopCursorY() {
        return (int) desktopCursorY;
    }

    @Override
    public void setCanvasWidth(int width) {
        canvasW = width;
        calcCursorCoef();
    }

    @Override
    public void setCanvasHeight(int height) {
        canvasH = height;
        calcCursorCoef();
    }

    private final class GestureListener extends GestureDetector.SimpleOnGestureListener {

        private static final int TIME_TO_DOUBLE_TAP = 100;
        private long startDoubleTapTime = 0;
        private long resultDoubleTapTime = 0;

        @Override
        public boolean onSingleTapConfirmed(MotionEvent ev) {

            sendCursorInfoPacket(MouseType.LEFT_CLICK);
            return true;
        }

        @Override
        public void onLongPress(MotionEvent e) {
            if (!isDragging)
                sendCursorInfoPacket(MouseType.RIGHT_CLICK);
        }

        @Override
        public boolean onDoubleTapEvent(MotionEvent e) {
            switch (e.getAction()) {
                case MotionEvent.ACTION_UP:
                    resultDoubleTapTime = System.currentTimeMillis() - startDoubleTapTime;
                    if (resultDoubleTapTime <= TIME_TO_DOUBLE_TAP) {
                        //Log.i("", "EndDragging");
                        sendCursorInfoPacket(MouseType.LEFT_BTN_UP);
                        //Log.i("", "DoubleTap");
                        sendCursorInfoPacket(MouseType.LEFT_CLICK);
                        isDragging = false;
                    } else {
                        //Log.i("", "EndDragging");
                        sendCursorInfoPacket(MouseType.LEFT_BTN_UP);
                        isDragging = false;
                    }
                    break;

                case MotionEvent.ACTION_DOWN:
                    startDoubleTapTime = System.currentTimeMillis();
                    //Log.i("", "BeginDragging");
                    isDragging = true;
                    sendCursorInfoPacket(MouseType.LEFT_BTN_DOWN);
                    break;

            }
            return true;
        }
    }
}

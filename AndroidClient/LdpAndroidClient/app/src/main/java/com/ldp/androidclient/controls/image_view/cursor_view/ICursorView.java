package com.ldp.androidclient.controls.image_view.cursor_view;

public interface ICursorView {
    float getCanvasCursroX();
    float getCanvasCursorY();

    int getDesktopCursorX();
    int getDesktopCursorY();

    void setCanvasWidth(int width);
    void setCanvasHeight(int height);
}

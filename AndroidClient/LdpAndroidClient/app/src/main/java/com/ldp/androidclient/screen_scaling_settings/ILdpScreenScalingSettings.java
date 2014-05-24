package com.ldp.androidclient.screen_scaling_settings;

public interface ILdpScreenScalingSettings {
    int getDesktopWidth();
    int getDesktopHeight();

    void setDesktopWidth(int width);
    void setDesktopHeight(int height);

    float getWidthCoef();
    float getHeightCoef();

    float getScaleCoef();

    int getCanvasWidth();
    int getCanvasHeight();

}

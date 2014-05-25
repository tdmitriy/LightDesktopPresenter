package com.ldp.androidclient.screen_scaling_settings;

public interface ILdpScreenScalingSettings {
    int getDesktopWidth();
    int getDesktopHeight();

    float getWidthCoef();
    float getHeightCoef();

    float getScaleCoef();

    int getCanvasWidth();
    int getCanvasHeight();

}

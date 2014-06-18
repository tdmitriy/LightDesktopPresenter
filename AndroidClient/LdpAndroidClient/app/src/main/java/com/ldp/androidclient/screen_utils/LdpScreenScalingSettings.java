package com.ldp.androidclient.screen_utils;

import android.util.DisplayMetrics;

import com.ldp.androidclient.activities.LdpActivityMain;

public class LdpScreenScalingSettings implements ILdpScreenScalingSettings {
    private DisplayMetrics dm;
    private final float screenWidth;
    private final float screenHeight;

    private float rmDesktopWidth;
    private float rmDesktopHeight;

    private final float canvasWidth;

    private final float widthCoef;
    private final float heightCoef;
    private final float scaleCoef;

    public LdpScreenScalingSettings(int desktopWidth, int desktopHeight) {
        rmDesktopWidth = desktopWidth;
        rmDesktopHeight = desktopHeight;

        dm = LdpActivityMain.getContext().getResources().getDisplayMetrics();
        screenWidth = dm.widthPixels;
        screenHeight = dm.heightPixels;

        canvasWidth = Math.round(rmDesktopWidth * screenHeight / rmDesktopHeight);

        widthCoef = canvasWidth / rmDesktopWidth;
        heightCoef = screenHeight / rmDesktopHeight;
        scaleCoef = widthCoef < heightCoef ? widthCoef : heightCoef;
    }

    @Override
    public int getDesktopWidth() {
        return (int) rmDesktopWidth;
    }

    @Override
    public int getDesktopHeight() {
        return (int) rmDesktopHeight;
    }

    @Override
    public float getWidthCoef() {
        return widthCoef;
    }

    @Override
    public float getHeightCoef() {
        return heightCoef;
    }

    @Override
    public float getScaleCoef() {
        return scaleCoef;
    }

    @Override
    public int getCanvasWidth() {
        return (int) canvasWidth;
    }

    @Override
    public int getCanvasHeight() {
        return (int) screenHeight;
    }
}

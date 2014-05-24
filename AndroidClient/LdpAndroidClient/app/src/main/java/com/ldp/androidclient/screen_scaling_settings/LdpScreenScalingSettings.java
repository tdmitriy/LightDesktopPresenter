package com.ldp.androidclient.screen_scaling_settings;

import android.util.DisplayMetrics;

public class LdpScreenScalingSettings implements ILdpScreenScalingSettings {
    private DisplayMetrics dm;
    private final int screenWidth;
    private final int screenHeight;

    private int rmDesktopWidth;
    private int rmDesktopHeight;

    private final int canvasWidth;

    private final float widthCoef;
    private final float heightCoef;
    private final float scaleCoef;

    public LdpScreenScalingSettings() {
        dm = new DisplayMetrics();
        screenWidth = dm.widthPixels;
        screenHeight = dm.heightPixels;

        canvasWidth = Math.round(rmDesktopWidth * screenHeight / rmDesktopHeight);

        widthCoef = canvasWidth / rmDesktopWidth;
        heightCoef = screenHeight / rmDesktopHeight;
        scaleCoef = widthCoef < heightCoef ? widthCoef : heightCoef;
    }

    @Override
    public int getDesktopWidth() {
        return rmDesktopWidth;
    }

    @Override
    public int getDesktopHeight() {
        return rmDesktopHeight;
    }

    @Override
    public void setDesktopWidth(int width) {
        rmDesktopWidth = width;
    }

    @Override
    public void setDesktopHeight(int height) {
        rmDesktopHeight = height;
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
        return canvasWidth;
    }

    @Override
    public int getCanvasHeight() {
        return screenHeight;
    }
}

package com.ldp.androidclient.controls.image_view.image_processing;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.RectF;

import com.ldp.androidclient.protocol.LdpProtocol.*;

import net.jpountz.lz4.LZ4Factory;
import net.jpountz.lz4.LZ4FastDecompressor;

public class LdpImageProcessor {

    private LZ4Factory factory;
    private LZ4FastDecompressor decompressor;
    private BitmapFactory.Options options;

    private RectF scaledRect = new RectF(0, 0, 0, 0);
    private float left, top, right, bottom;
    private final int rmDesktopWidth, rmDesktopHeight;
    private final float scaleCoef;

    private Bitmap scaledScreen;
    private Bitmap origScreen;

    public LdpImageProcessor(int rmDesktopWidth, int rmDesktopHeight, float scaleCoef) {
        this.rmDesktopWidth = rmDesktopWidth;
        this.rmDesktopHeight = rmDesktopHeight;
        this.scaleCoef = scaleCoef;

        options = new BitmapFactory.Options();
        factory = LZ4Factory.fastestInstance();
        decompressor = factory.fastDecompressor();
    }


    public void processRawData(LdpScreenResponse response) {
        int baseLenght = response.getBaseLenght();
        byte[] compressedScreen = response.getCompressedScreen().toByteArray();
        byte[] decompressedScreen = decompressor.decompress(compressedScreen, baseLenght);
        setScaledRect(response.getRect());
        RectF rect = getScaledRect();
        origScreen = null;
        int rectW = (int) rect.width();
        int rectH = (int) rect.height();
        origScreen = decodeBitmap(decompressedScreen, rectW, rectH);
        scaledScreen = scaleScreen(origScreen, rectW, rectH);
        origScreen.recycle();
    }

    private Bitmap decodeBitmap(byte[] data, int reqWidth, int reqHeight) {
        options.inJustDecodeBounds = true;
        BitmapFactory.decodeByteArray(data, 0, data.length, options);

        options.inSampleSize = calculateInSampleSize(options, reqWidth, reqHeight);

        options.inDither = false;
        options.inPurgeable = true;
        options.inInputShareable = true;
        options.inJustDecodeBounds = false;
        options.inTempStorage = new byte[32 * 1024];
        options.inPreferredConfig = Bitmap.Config.RGB_565;
        options.inPreferQualityOverSpeed = true;

        return BitmapFactory.decodeByteArray(data, 0, data.length, options);
    }

    private int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
        // Raw height and width of image
        final int height = options.outHeight;
        final int width = options.outWidth;
        int inSampleSize = 1;

        if (height > reqHeight || width > reqWidth) {

            // Calculate ratios of height and width to requested height and width
            final int heightRatio = Math.round((float) height / (float) reqHeight);
            final int widthRatio = Math.round((float) width / (float) reqWidth);

            // Choose the smallest ratio as inSampleSize value, this will guarantee
            // a final image with both dimensions larger than or equal to the
            // requested height and width.
            inSampleSize = heightRatio < widthRatio ? heightRatio : widthRatio;
        }
        return inSampleSize;
    }

    public Bitmap getScaledScreen() {
        return scaledScreen;
    }

    public RectF getScaledRect() {
        return scaledRect;
    }

    private void setScaledRect(LdpRectangle rect) {
        left = rect.getLeft() * scaleCoef;
        top = rect.getTop() * scaleCoef;
        right = rect.getRight() * scaleCoef;
        bottom = rect.getBottom() * scaleCoef;

        scaledRect = new RectF(left, top, right, bottom);
    }

    private Bitmap scaleScreen(Bitmap bitmap, int newWidth, int newHeight) {
        int scaledWidth = (int) (newWidth * scaleCoef);
        int scaledHeight = (int) (newHeight * scaleCoef);
        return Bitmap.createScaledBitmap(bitmap, scaledWidth, scaledHeight, true);
    }
}

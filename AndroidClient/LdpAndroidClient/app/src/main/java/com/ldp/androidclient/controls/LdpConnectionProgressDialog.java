package com.ldp.androidclient.controls;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Context;
import android.os.Handler;
import android.os.Message;

public class LdpConnectionProgressDialog {
    private static ProgressDialog dialog;
    private static Context context;
    private static String dialogMessage;
    private static final long TIMEOUT = 8000;

    public static void show(Context context, String title, String message) {
        LdpConnectionProgressDialog.context = context;
        dialog = new ProgressDialog(context);
        dialog.setTitle(title);
        dialog.setMessage(message);
        dialog.setIndeterminate(true);
        dialog.setCancelable(false);
        dialog.show();
        //after CONNECTION_TIMEOUT call dismissDialog
        handler.postDelayed(dismissDialog, TIMEOUT);
    }

    public static void dismiss() {
        handler.removeCallbacks(dismissDialog);
        dialog.dismiss();
    }

    public static void changeMessage(String message) {
        dialogMessage = message;
        handler.sendEmptyMessage(0);
    }

    private static Runnable dismissDialog = new Runnable() {
        @Override
        public void run() {
            dialog.dismiss();
        }
    };

    private static Handler handler = new Handler() {
        @Override
        public void handleMessage(Message msg) {
            dialog.setMessage(dialogMessage);
        }
    };
}

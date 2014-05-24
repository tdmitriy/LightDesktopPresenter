package com.ldp.androidclient.utils.controls;

import android.app.Activity;
import android.app.ProgressDialog;
import android.os.Handler;

public class LdpConnectionProgressDialog {
    private static final long CONNECTION_TIMEOUT = 5000;
    private static ProgressDialog dialog;
    private static Handler handler = new Handler();
    private static Activity context;

    public static void show(Activity context, String title, String message) {
        LdpConnectionProgressDialog.context = context;
        dialog = new ProgressDialog(context);
        dialog.setTitle(title);
        dialog.setMessage(message);
        dialog.setIndeterminate(true);
        dialog.setCancelable(false);
        dialog.show();
        //after CONNECTION_TIMEOUT call dismissDialog
        handler.postDelayed(dismissDialog, CONNECTION_TIMEOUT);
    }

    public static void dismiss() {
        handler.removeCallbacks(dismissDialog);
        dialog.dismiss();
    }

    public static void changeMessage(final String message) {
        context.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                dialog.setMessage(message);
            }
        });
    }

    private static Runnable dismissDialog = new Runnable() {
        @Override
        public void run() {
            dialog.dismiss();
            LdpMessageBox.show(context, "Connection timeout.\nServer is unreachable.",
                    LdpMessageBox.DialogType.ERROR);
        }
    };
}

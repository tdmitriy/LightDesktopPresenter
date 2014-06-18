package com.ldp.androidclient.controls;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;

import com.ldp.androidclient.R;

public class LdpMessageBox {

    public static void show(Context context, String message, DialogType type) {
        String title;
        AlertDialog.Builder dialog = new AlertDialog.Builder(context);
        switch (type) {
            case ERROR:
                title = "Error";
                dialog.setTitle(title);
                dialog.setIcon(R.drawable.ic_action_error);
                break;
            case WARNING:
                title = "Warning";
                dialog.setTitle(title);
                dialog.setIcon(R.drawable.ic_action_warning);
                break;
        }
        dialog.setMessage(message);
        dialog.setPositiveButton("OK", null);
        dialog.setCancelable(true);
        dialog.create().show();
    }

    public static void showWithButtons(Context context,
                                       String title,
                                       String message,
                                       String btnOkText,
                                       String btnNoText,
                                       DialogType dlgType,
                                       ButtonType btnType,
                                       DialogInterface.OnClickListener listener) {
        AlertDialog.Builder dialog = new AlertDialog.Builder(context);
        switch (dlgType) {
            case ERROR:
                dialog.setIcon(R.drawable.ic_action_error);
                break;
            case WARNING:
                dialog.setIcon(R.drawable.ic_action_warning);
                break;
            case QUESTION:
                dialog.setIcon(R.drawable.ic_action_help);
                break;
        }

        switch (btnType) {
            case OK:
                dialog.setPositiveButton(btnOkText, listener);
                break;
            case NO:
                dialog.setNegativeButton(btnNoText, listener);
                break;
            case BOTH:
                dialog.setPositiveButton(btnOkText, listener);
                dialog.setNegativeButton(btnNoText, listener);
                break;
        }
        dialog.setTitle(title);
        dialog.setMessage(message);
        dialog.setCancelable(false);
        dialog.create().show();
    }

    public static void showListDialog(Context context, String title, int itemsId,
                                      DialogInterface.OnClickListener listener) {
        AlertDialog.Builder builder = new AlertDialog.Builder(context);
        builder.setTitle(title);
        builder.setItems(itemsId, listener);
        builder.create().show();
    }


    public enum DialogType {
        WARNING,
        ERROR,
        QUESTION
    }

    public enum ButtonType {
        OK,
        NO,
        BOTH
    }
}

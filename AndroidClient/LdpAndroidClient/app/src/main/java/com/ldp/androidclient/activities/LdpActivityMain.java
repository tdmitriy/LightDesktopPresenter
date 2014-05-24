package com.ldp.androidclient.activities;


import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.View;
import android.widget.ListView;
import android.widget.Toast;

import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

public class LdpActivityMain extends LdpActivityMainInterface {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    protected void onListItemClick(ListView list, View v, int position, long id) {
        selectedPrefs = (LdpConnectionPreferences)
                list.getItemAtPosition(position);
        showConnectionTypeDialog();

    }

    private void showConnectionTypeDialog() {
        new AlertDialog.Builder(this)
                .setTitle("Select type of connection")
                .setNegativeButton("Cancel", null)
                .setCancelable(false)
                .setAdapter(listViewConnectionTypePopulator, connectionClickListener)
                .show();
    }

    private DialogInterface.OnClickListener connectionClickListener = new DialogInterface.OnClickListener() {
        @Override
        public void onClick(DialogInterface dialog, int which) {
            switch (which) {
                case 0:
                    // TODO send auth request and connection type: remote desktop
                    Toast.makeText(getApplication(), "Desktop control", Toast.LENGTH_SHORT).show();
                    break;
                case 1:
                    // TODO send auth request and connection type: remote volume
                    Toast.makeText(getApplication(), "Volume control", Toast.LENGTH_SHORT).show();
                    break;
            }
        }
    };

}

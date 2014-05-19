package com.ldp.androidclient.activities;

import android.app.Activity;
import android.app.AlertDialog;
import android.os.Bundle;
import android.text.InputFilter;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.ldp.androidclient.R;
import com.ldp.androidclient.utils.LdpEditTextIPAddressChecker;
import com.ldp.androidclient.utils.controls.LdpMessageBox;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpObjectPreference;


public class ActivityNewConnection extends Activity implements View.OnClickListener {

    private static final String TAG = "ActivityNewConnection";
    private LdpObjectPreference objectPreference;
    private LdpComplexPreferences complexPrefenreces;

    private EditText txtIP;
    private EditText txtDisplayedName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_new_connection);

        objectPreference = (LdpObjectPreference) this.getApplication();
        complexPrefenreces = objectPreference.getComplexPreference();

        initialize();

    }

    private void initialize() {
        Button btnDone = (Button) findViewById(R.id.btnDone);
        btnDone.setOnClickListener(this);

        Button btnCancel = (Button) findViewById(R.id.btnCancel);
        btnCancel.setOnClickListener(this);

        txtIP = (EditText) findViewById(R.id.txtIPAddress);
        txtIP.setFilters(LdpEditTextIPAddressChecker.getIPFilter());

        txtDisplayedName = (EditText) findViewById(R.id.txtDisplayedName);
    }

    @Override
    public void onClick(View v) {
        String ip = txtIP.getText().toString();
        String dispName = txtDisplayedName.getText().toString();

        switch (v.getId()) {
            case R.id.btnDone:
                addPreferences(ip, dispName);
                break;
            case R.id.btnCancel:
                for (LdpConnectionPreferences pref : complexPrefenreces.getPreferences()) {
                    String ipp = pref.getIPAddress();
                    String name = pref.getDisplayedName();
                    boolean result = complexPrefenreces.remove(pref);
                    if (result)
                        Log.i(TAG, "removed: ip=" + ipp + ", name=" + name);
                }

                //finish();
                break;
            default:
                break;
        }
    }

    private void addPreferences(String ipAddress, String displayedName) {
        if (displayedName != null && !displayedName.isEmpty()) {
            boolean result = complexPrefenreces.addNewConnectionPreferences(ipAddress,
                    displayedName);
            if (!result)
                LdpMessageBox.show(this, "Current connection name is already exists.",
                        LdpMessageBox.DialogType.ERROR);
        } else {
            LdpMessageBox.show(this, "The fields cannot be empty.",
                    LdpMessageBox.DialogType.WARNING);
        }
    }
}

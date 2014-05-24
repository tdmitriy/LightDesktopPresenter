package com.ldp.androidclient.activities;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.NavUtils;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.utils.LdpEditTextIPAddressChecker;
import com.ldp.androidclient.utils.controls.LdpMessageBox;
import com.ldp.androidclient.base_application.LdpBaseApplication;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

@SuppressLint("AppCompatMethod")
public class LdpActivityNewConnectionPreferences extends Activity implements View.OnClickListener {

    private static final String TAG = "ActivityNewConnectionPreferences";
    private LdpBaseApplication objectPreference;
    private LdpComplexPreferences complexPrefenreces;

    private EditText txtIP;
    private EditText txtConnectionName;
    private EditText txtPassword;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_new_con_preferences);

        ActionBar actionBar = getActionBar();
        if (actionBar != null)
            actionBar.setDisplayHomeAsUpEnabled(true);

        initializeControls();
        initializeObjects();
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                NavUtils.navigateUpFromSameTask(this);
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private void initializeControls() {
        Button btnDone = (Button) findViewById(R.id.btnDone);
        btnDone.setOnClickListener(this);

        Button btnCancel = (Button) findViewById(R.id.btnCancel);
        btnCancel.setOnClickListener(this);

        txtIP = (EditText) findViewById(R.id.txtIPAddress);
        txtIP.setFilters(LdpEditTextIPAddressChecker.getIPFilter());

        txtConnectionName = (EditText) findViewById(R.id.txtConnectionName);
        txtPassword = (EditText) findViewById(R.id.txtPassword);
    }

    private void initializeObjects() {
        objectPreference = (LdpBaseApplication) this.getApplication();
        complexPrefenreces = objectPreference.getComplexPreference();
    }

    @Override
    public void onClick(View v) {
        String ipAddress = txtIP.getText().toString();
        String connectionName = txtConnectionName.getText().toString();
        String password = txtPassword.getText().toString();

        switch (v.getId()) {
            case R.id.btnDone:
                LdpConnectionPreferences prefs = new LdpConnectionPreferences();
                prefs.setIPAddress(ipAddress);
                prefs.setConnectionName(connectionName);
                prefs.setPassword(password);
                addPreferences(prefs);
                break;
            case R.id.btnCancel:
                finish();
                break;
            default:
                break;
        }
    }

    private void addPreferences(LdpConnectionPreferences prefs) {
        String connectionName = prefs.getConnectionName();
        String ipAddress = prefs.getIPAddress();
        if (connectionName != null &&
                !connectionName.isEmpty() &&
                ipAddress != null &&
                !ipAddress.isEmpty()) {
            boolean result = complexPrefenreces.addNewConnectionPreferences(prefs);
            if (result) {
                showMessage("Connection added: " + ipAddress);
                finish();
            }
            else
                LdpMessageBox.show(this, "Current connection name is already exists.",
                        LdpMessageBox.DialogType.ERROR);


        } else {
            LdpMessageBox.show(this, "The fields cannot be empty.",
                    LdpMessageBox.DialogType.WARNING);
        }
    }

    private void showMessage(String mess) {
        Toast.makeText(this, mess, Toast.LENGTH_LONG).show();
    }
}

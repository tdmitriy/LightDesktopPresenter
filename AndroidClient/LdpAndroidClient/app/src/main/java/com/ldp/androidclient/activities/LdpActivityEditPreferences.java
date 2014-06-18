package com.ldp.androidclient.activities;

import android.annotation.SuppressLint;
import android.app.ActionBar;
import android.app.Activity;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.NavUtils;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.controls.LdpMessageBox;
import com.ldp.androidclient.base_application.LdpBaseApplication;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

@SuppressLint("AppCompatMethod")
public class LdpActivityEditPreferences extends Activity implements View.OnClickListener,
        DialogInterface.OnClickListener {


    private static final String TAG = "LdpActivityEditPreferences";
    private LdpBaseApplication objectPreference;
    private LdpComplexPreferences complexPrefenreces;

    private EditText txtIp;
    private EditText txtConnectionName;
    private EditText txtPassword;


    private Button btnSave;
    private Button btnCancel;

    private static LdpConnectionPreferences newPreferences;
    private static LdpConnectionPreferences oldPreferences;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit_con_preferences);

        ActionBar actionBar = getActionBar();
        if (actionBar != null)
            actionBar.setDisplayHomeAsUpEnabled(true);

        initializeControls();
        initializeObjects();
    }

    private void initializeControls() {
        txtIp = (EditText) findViewById(R.id.txtIPAddress);
        txtConnectionName = (EditText) findViewById(R.id.txtConnectionName);
        txtPassword = (EditText) findViewById(R.id.txtPassword);

        btnSave = (Button) findViewById(R.id.btnSave);
        btnCancel = (Button) findViewById(R.id.btnCancel);

        btnSave.setOnClickListener(this);
        btnCancel.setOnClickListener(this);
    }

    private void initializeObjects() {
        Intent intent = getIntent();
        String ipAddress = intent.getExtras().getString("IpAddress");
        String connectionName = intent.getExtras().getString("ConnectionName");
        String password = intent.getExtras().getString("Password");
        oldPreferences = new LdpConnectionPreferences();
        oldPreferences.setIPAddress(ipAddress);
        oldPreferences.setConnectionName(connectionName);
        oldPreferences.setPassword(password);


        objectPreference = (LdpBaseApplication) this.getApplication();
        complexPrefenreces = objectPreference.getComplexPreference();

        txtIp.setText(ipAddress);
        txtConnectionName.setText(connectionName);
        txtPassword.setText(password);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.delete_connection, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                NavUtils.navigateUpFromSameTask(this);
                return true;
            case R.id.settings_delete_connection:
                showDeleteDialog();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.btnSave:
                String ipAddress = txtIp.getText().toString();
                String newConnectionName = txtConnectionName.getText().toString();
                String password = txtPassword.getText().toString();
                newPreferences = new LdpConnectionPreferences();
                newPreferences.setIPAddress(ipAddress);
                newPreferences.setConnectionName(newConnectionName);
                newPreferences.setPassword(password);

                savePreferences(oldPreferences, newPreferences);
                break;
            case R.id.btnCancel:
                finish();
                break;
        }
    }

    private void savePreferences(LdpConnectionPreferences oldPrefs,
                                 LdpConnectionPreferences newPrefs) {
        String connectionName = newPrefs.getConnectionName();
        String ipAddress = newPrefs.getIPAddress();
        if (connectionName != null &&
                !connectionName.isEmpty() &&
                ipAddress != null &&
                !ipAddress.isEmpty()) {
            boolean result = complexPrefenreces.editPreferences(oldPrefs, newPrefs);
            if (result) {
                showMessage("Settings successfully changed");
                finish();
            } else
                LdpMessageBox.show(this, "Current connection name is already exists.",
                        LdpMessageBox.DialogType.ERROR);


        } else {
            LdpMessageBox.show(this, "The fields cannot be empty.",
                    LdpMessageBox.DialogType.WARNING);
        }
    }

    private void showDeleteDialog() {
        LdpMessageBox.showWithButtons(this,
                "Confirmation",
                "Delete current connection: " + oldPreferences.getConnectionName() + "?",
                "Delete",
                "Cancel",
                LdpMessageBox.DialogType.QUESTION,
                LdpMessageBox.ButtonType.BOTH,
                this);
    }

    private void deleteCurrentPrefs() {
        complexPrefenreces.remove(oldPreferences);
        finish();
    }

    private void showMessage(String mess) {
        Toast.makeText(this, mess, Toast.LENGTH_LONG).show();
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {
        switch (which) {
            case DialogInterface.BUTTON_POSITIVE:
                deleteCurrentPrefs();
                break;
            case DialogInterface.BUTTON_NEGATIVE:
                break;
        }
    }
}

package com.ldp.androidclient.activities;

import android.app.Activity;
import android.os.Bundle;
import android.text.InputFilter;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.ldp.androidclient.R;
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
        txtIP.setFilters(checkIPAddressFilter());

        txtDisplayedName = (EditText) findViewById(R.id.txtDisplayedName);

    }

    @Override
    public void onClick(View v) {
        String ip = txtIP.getText().toString();
        String dispName = txtDisplayedName.getText().toString();

        switch (v.getId()) {
            case R.id.btnDone:
                if (dispName != null && !dispName.isEmpty())
                    addNewConnectionPreferences(ip, dispName);
                else {
                    // TODO show allert message
                }

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

    private void showMessage(String mess) {
        Toast.makeText(this, mess, Toast.LENGTH_LONG).show();
    }

    private void addNewConnectionPreferences(String ipAddress, String displayedName) {
        try {
            LdpConnectionPreferences preferences = new LdpConnectionPreferences();
            preferences.setIPAddress(ipAddress);
            preferences.setDisplayedName(displayedName);

            boolean prefExists = checkPreferencesIfExists(displayedName);

            if (!prefExists) {
                if (complexPrefenreces != null) {
                    complexPrefenreces.putObject(displayedName, preferences);
                    showMessage("Added new connection: " + ipAddress);
                    Log.i(TAG, "Added new preferences: " + displayedName);
                } else {
                    Log.e(TAG, "Preference is null");
                }
            } else {
                Log.e(TAG, "Preferences is already exists.");
            }

        } catch (Exception ex) {
            Log.e(TAG, "Add preferences error.\n" + ex.getMessage());
        }

    }


    private boolean checkPreferencesIfExists(String displayedName) {
        LdpConnectionPreferences preferences =
                complexPrefenreces.getObject(displayedName, LdpConnectionPreferences.class);
        return preferences != null;
    }


    private InputFilter[] checkIPAddressFilter() {
        InputFilter[] filters = new InputFilter[1];
        filters[0] = new InputFilter() {
            @Override
            public CharSequence filter(CharSequence source, int start, int end,
                                       android.text.Spanned dest, int dstart, int dend) {
                if (end > start) {
                    String destTxt = dest.toString();
                    String resultingTxt = destTxt.substring(0, dstart) + source.subSequence(start, end) + destTxt.substring(dend);
                    if (!resultingTxt.matches("^\\d{1,3}(\\.(\\d{1,3}(\\.(\\d{1,3}(\\.(\\d{1,3})?)?)?)?)?)?")) {
                        return "";
                    } else {
                        String[] splits = resultingTxt.split("\\.");
                        for (String split : splits) {
                            if (Integer.valueOf(split) > 255) {
                                return "";
                            }
                        }
                    }
                }
                return null;
            }
        };
        return filters;
    }

}

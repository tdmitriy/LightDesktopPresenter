package com.ldp.androidclient.base_application;

import android.app.Application;

import com.ldp.androidclient.tcp_client.LdpClient;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;

public class LdpBaseApplication extends Application {

    private static final String TAG = "LdpObjectPreference";
    private static final String SETTINGS_VALUE = "ConnectionSettings";
    private LdpComplexPreferences complexPrefenreces = null;


    @Override
    public void onCreate() {
        super.onCreate();
        //init singleton
        LdpClient.initSingleInstance(this);
        complexPrefenreces = LdpComplexPreferences.getComplexPreferences(getBaseContext(),
                SETTINGS_VALUE, MODE_PRIVATE);
        android.util.Log.i(TAG, "Preference loaded: " + SETTINGS_VALUE);
    }

    public LdpComplexPreferences getComplexPreference() {
        if (complexPrefenreces != null) {
            return complexPrefenreces;
        }
        return null;
    }
}

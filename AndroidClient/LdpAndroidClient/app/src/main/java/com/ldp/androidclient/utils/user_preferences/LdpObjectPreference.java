package com.ldp.androidclient.utils.user_preferences;

import android.app.Application;

public class LdpObjectPreference extends Application {

    private static final String TAG = "LdpObjectPreference";
    private static final String SETTINGS_VALUE = "ConnectionSettings";
    private LdpComplexPreferences complexPrefenreces = null;


    @Override
    public void onCreate() {
        super.onCreate();
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

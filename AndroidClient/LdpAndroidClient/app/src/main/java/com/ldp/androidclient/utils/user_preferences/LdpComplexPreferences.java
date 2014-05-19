package com.ldp.androidclient.utils.user_preferences;

import java.util.ArrayList;
import java.util.Map;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;


public class LdpComplexPreferences {

    private static final String TAG = "LdpComplexPreferences";
    private static LdpComplexPreferences complexPreferences;
    private final Context context;
    private final SharedPreferences preferences;
    private final SharedPreferences.Editor editor;
    private static Gson gson = new Gson();

    private LdpComplexPreferences(Context context, String namePreferences, int mode) {
        this.context = context;
        preferences = context.getSharedPreferences(namePreferences, mode);
        editor = preferences.edit();
    }

    public static LdpComplexPreferences getComplexPreferences(Context context,
                                                              String namePreferences, int mode) {
        if (complexPreferences == null) {
            complexPreferences = new LdpComplexPreferences(context,
                    namePreferences, mode);
        }
        return complexPreferences;
    }

    public void putObject(String key, Object object) {
        editor.putString(key, gson.toJson(object));
        commit();
    }

    private void commit() {
        editor.commit();
    }

    public boolean remove(LdpConnectionPreferences pref) {
        if (pref != null) {
            String key = pref.getDisplayedName();
            LdpConnectionPreferences tmpPref = getObject(key, pref.getClass());
            editor.remove(tmpPref.getDisplayedName());
            commit();
            return true;
        } else
            return false;
    }

    private <T> T getObject(String key, Class<T> a) {
        String gsonString = preferences.getString(key, null);
        if (gsonString == null) {
            return null;
        } else {
            try {
                return gson.fromJson(gsonString, a);
            } catch (Exception e) {
                Log.e("", "Object stored with key "
                        + key + " is instance of other class");
                return null;
            }
        }
    }

    public boolean addNewConnectionPreferences(String ipAddress, String displayedName) {
        try {
            LdpConnectionPreferences preferences = new LdpConnectionPreferences();
            preferences.setIPAddress(ipAddress);
            preferences.setDisplayedName(displayedName);

            boolean prefExists = checkPreferencesIfExists(displayedName);

            if (!prefExists) {
                putObject(displayedName, preferences);
                String mess = "Added new connection: ";
                showMessage(mess + ipAddress);
                Log.i(TAG, mess + displayedName);
                return true;
            } else {
                Log.e(TAG, "Preferences is already exists.");
                return false;
            }

        } catch (Exception ex) {
            Log.e(TAG, "Add preferences error.\n" + ex.getMessage());
            return false;
        }
    }

    private void showMessage(String mess) {
        Toast.makeText(context, mess, Toast.LENGTH_LONG).show();
    }

    private boolean checkPreferencesIfExists(String displayedName) {
        LdpConnectionPreferences preferences =
                getObject(displayedName, LdpConnectionPreferences.class);
        return preferences != null;
    }

    public void editPreferences(String ipAddress, String displayedName) {
        LdpConnectionPreferences preferences = new LdpConnectionPreferences();
        preferences.setIPAddress(ipAddress);
        preferences.setDisplayedName(displayedName);
        putObject(displayedName, preferences);
        String mess = "Settings successfully changed.";
        showMessage(mess);
        Log.i(TAG, mess);
    }

    public ArrayList<LdpConnectionPreferences> getPreferences() {
        Map<String, ?> allPrefs = preferences.getAll();
        ArrayList<LdpConnectionPreferences> tmp =
                new ArrayList<LdpConnectionPreferences>();
        for (Map.Entry<String, ?> entry : allPrefs.entrySet()) {
            LdpConnectionPreferences pref =
                    getObject(entry.getKey(), LdpConnectionPreferences.class);
            tmp.add(pref);
        }
        return tmp;
    }
}

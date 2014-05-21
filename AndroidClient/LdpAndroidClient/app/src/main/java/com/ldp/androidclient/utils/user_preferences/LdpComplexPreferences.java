package com.ldp.androidclient.utils.user_preferences;

import java.util.ArrayList;
import java.util.Collections;
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
            String key = pref.getConnectionName();
            LdpConnectionPreferences tmpPref = getObject(key, pref.getClass());
            editor.remove(tmpPref.getConnectionName());
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

    public boolean addNewConnectionPreferences(LdpConnectionPreferences prefs) {
        try {
            boolean prefExists = checkPreferencesIfExists(prefs);
            String connectionName = prefs.getConnectionName();
            if (!prefExists) {
                putObject(connectionName, prefs);
                String mess = "Added new connection: ";
                Log.i(TAG, mess + connectionName);
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


    private boolean checkPreferencesIfExists(LdpConnectionPreferences prefs) {
        LdpConnectionPreferences preferences =
                getObject(prefs.getConnectionName(), LdpConnectionPreferences.class);
        return preferences != null;
    }

    private boolean checkEdditablePreferencesIfExists(LdpConnectionPreferences oldPrefs,
                                                      LdpConnectionPreferences newPrefs) {
        String newConnectionName = newPrefs.getConnectionName();
        if (oldPrefs.getConnectionName().equals(newConnectionName)) {
            return false;
        } else {
            LdpConnectionPreferences preferences =
                    getObject(newConnectionName, LdpConnectionPreferences.class);
            return preferences != null;
        }
    }

    public boolean editPreferences(LdpConnectionPreferences oldPrefs,
                                   LdpConnectionPreferences newPrefs) {
        boolean prefExists = checkEdditablePreferencesIfExists(oldPrefs, newPrefs);
        if (!prefExists) {
            String newConnectionName = newPrefs.getConnectionName();
            String oldConnectionName = oldPrefs.getConnectionName();
            putObject(newConnectionName, newPrefs);
            if (!oldConnectionName.equals(newConnectionName))
                remove(oldPrefs);
            String mess = "Settings successfully changed.";
            Log.i(TAG, mess + newConnectionName);
            return true;
        } else {
            Log.e(TAG, "Settings is already exists.");
            return false;
        }
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
        Collections.reverse(tmp);
        return tmp;
    }
}

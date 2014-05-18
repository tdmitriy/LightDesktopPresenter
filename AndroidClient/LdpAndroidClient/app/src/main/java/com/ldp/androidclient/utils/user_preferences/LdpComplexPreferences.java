package com.ldp.androidclient.utils.user_preferences;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;


public class LdpComplexPreferences {

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

    public <T> T getObject(String key, Class<T> a) {
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

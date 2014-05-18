package com.ldp.androidclient.activities;

import android.app.Activity;
import android.app.ListActivity;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.utils.user_preferences.LdpClickableImageView;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpListViewPopulator;
import com.ldp.androidclient.utils.user_preferences.LdpObjectPreference;

import java.util.ArrayList;


public class ActivityMain extends ListActivity {

    private LdpListViewPopulator listViewPopulator;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    protected void onStart() {
        super.onStart();
        recreateListView();
    }

    private void recreateListView() {

        LdpObjectPreference objectPreference = (LdpObjectPreference) this.getApplication();
        LdpComplexPreferences complexPreferences = objectPreference.getComplexPreference();

        ArrayList<LdpConnectionPreferences> prefs = complexPreferences.getPreferences();
        listViewPopulator = new LdpListViewPopulator(this, prefs);
        setListAdapter(null);
        setListAdapter(listViewPopulator);
        addImgSettingsOnClickListener();

        LinearLayout layout = (LinearLayout) findViewById(R.id.linear_layout_bg);
        Drawable imagePC = getResources().getDrawable(R.drawable.pc_icon);
        if(prefs.isEmpty()) {
            showMessage("List is empty.");
            layout.setBackgroundResource(R.drawable.pc_icon);
        } else {
            layout.setBackgroundResource(R.drawable.abc_ab_transparent_light_holo);
        }
    }


    private void showMessage(String mess) {
        Toast.makeText(this, mess, Toast.LENGTH_LONG).show();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main_menu_action, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.settings_add_new_connection:
                Intent newConnectionActivity = new Intent(this, ActivityNewConnection.class);
                startActivity(newConnectionActivity);
                break;
        }
        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onListItemClick(ListView list, View v, int position, long id) {
        LdpConnectionPreferences pref = (LdpConnectionPreferences)
                list.getItemAtPosition(position);
        if (pref != null)
            Toast.makeText(this, "ip=" + pref.getIPAddress() +
                    " | " + "name=" + pref.getDisplayedName(), Toast.LENGTH_LONG).show();
    }

    private void addImgSettingsOnClickListener() {
        listViewPopulator.onClickListener = new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                final int position = getListView().getPositionForView(v);
                if (position != ListView.INVALID_POSITION) {
                    LdpConnectionPreferences pref = (LdpConnectionPreferences) listViewPopulator.getItem(position);
                    if (pref != null)
                        Toast.makeText(getApplicationContext(), "IPPPP=" + pref.getIPAddress() +
                                " | " + "name=" + pref.getDisplayedName(), Toast.LENGTH_LONG).show();
                }
            }
        };
    }
}

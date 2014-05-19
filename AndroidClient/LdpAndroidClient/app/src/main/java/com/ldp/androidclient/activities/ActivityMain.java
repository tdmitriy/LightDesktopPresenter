package com.ldp.androidclient.activities;

import android.app.ListActivity;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;
import com.ldp.androidclient.utils.main_listview_populator.LdpListViewPopulator;
import com.ldp.androidclient.utils.user_preferences.LdpObjectPreference;

import java.util.ArrayList;


public class ActivityMain extends ListActivity {

    private LdpListViewPopulator listViewPopulator;
    private LdpObjectPreference objectPreference;
    private LdpComplexPreferences complexPreferences;
    private ArrayList<LdpConnectionPreferences> prefs;

    private ImageView background;
    private TextView txtListEmptyinfo;
    private Drawable imageBigPC;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        initControls();
    }

    @Override
    protected void onStart() {
        super.onStart();
        recreateListView();
    }

    private void initControls() {
        background = (ImageView) findViewById(R.id.imgViewBigPC);
        txtListEmptyinfo = (TextView) findViewById(R.id.txtListEmptyinfo);
        imageBigPC = getResources().getDrawable(R.drawable.icon_big_pc);

        objectPreference = (LdpObjectPreference) this.getApplication();
        complexPreferences = objectPreference.getComplexPreference();
    }

    private void checkPreferencesIsEmpty() {
        if (prefs.isEmpty()) {
            setPreferencesEmptyContent();
        } else {
            clearPreferencesContentIfNotEmpty();
        }
    }

    private void setPreferencesEmptyContent() {
        showMessage("Connection list is empty.");
        background.setImageDrawable(imageBigPC);
        String message = "Tap + to add new connection.";
        txtListEmptyinfo.setText(message);
    }

    private void clearPreferencesContentIfNotEmpty() {
        background.setImageDrawable(null);
        txtListEmptyinfo.setText("");
    }

    private void recreateListView() {
        if (prefs != null) {
            prefs.clear();
            prefs = null;
        }
        prefs = complexPreferences.getPreferences();
        listViewPopulator = new LdpListViewPopulator(this, prefs);
        setListAdapter(null);
        setListAdapter(listViewPopulator);
        addImgSettingsOnClickListener();

        checkPreferencesIsEmpty();
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

package com.ldp.androidclient.activities;

import android.app.ListActivity;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.ldp.androidclient.R;
import com.ldp.androidclient.utils.controls.LdpConnectionTypeAdapter;
import com.ldp.androidclient.utils.controls.LdpMessageBox;
import com.ldp.androidclient.utils.controls.LdpListViewPopulator;
import com.ldp.androidclient.base_application.LdpBaseApplication;
import com.ldp.androidclient.utils.user_preferences.LdpComplexPreferences;
import com.ldp.androidclient.utils.user_preferences.LdpConnectionPreferences;

import java.util.ArrayList;

public class LdpActivityMainInterface extends ListActivity implements
        AdapterView.OnItemLongClickListener,
        DialogInterface.OnClickListener {

    private LdpListViewPopulator listViewPopulator;
    protected ListAdapter listViewConnectionTypePopulator;

    String[] items = {"Desktop control", "Volume control"};
    Integer[] images = {R.drawable.remote_pc_icon, R.drawable.remote_volume_icon};

    private LdpBaseApplication objectPreference;
    protected LdpComplexPreferences complexPreferences;
    private ArrayList<LdpConnectionPreferences> prefs;
    protected LdpConnectionPreferences selectedPrefs;

    private TextView txtListEmptyinfo;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        initializeControls();
        initializeObjects();
    }

    @Override
    protected void onStart() {
        super.onStart();
        recreateListView();
    }

    private void initializeObjects() {
        objectPreference = (LdpBaseApplication) this.getApplication();
        complexPreferences = objectPreference.getComplexPreference();
    }

    private void initializeControls() {
        txtListEmptyinfo = (TextView) findViewById(R.id.txtListEmptyinfo);
        this.getListView().setOnItemLongClickListener(this);
        listViewConnectionTypePopulator = new LdpConnectionTypeAdapter(this, items, images);
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
        String message = getResources().getString(R.string.string_empty_list);
        txtListEmptyinfo.setText(message);
    }

    private void clearPreferencesContentIfNotEmpty() {
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
        Toast.makeText(this, mess, Toast.LENGTH_SHORT).show();
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
                Intent newConnectionActivity = new Intent(this,
                        LdpActivityNewConnectionPreferences.class);
                startActivity(newConnectionActivity);
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private void addImgSettingsOnClickListener() {
        listViewPopulator.onClickListener = new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                final int position = getListView().getPositionForView(v);
                if (position != ListView.INVALID_POSITION) {
                    selectedPrefs = (LdpConnectionPreferences)
                            listViewPopulator.getItem(position);
                    editSelectedPrefs();
                }
            }
        };
    }

    @Override
    public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {
        selectedPrefs = (LdpConnectionPreferences)
                parent.getItemAtPosition(position);
        if (selectedPrefs != null) {
            String connectionName = selectedPrefs.getConnectionName();
            LdpMessageBox.showListDialog(this, connectionName,
                    R.array.connection_info_list, this);
        }
        return true;
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {
        switch (which) {
            case 0:
                showMessage("Connect");
                break;
            case 1:
                editSelectedPrefs();
                break;
            case 2:
                deleteCurrentPrefs();
                break;
        }
    }

    private void editSelectedPrefs() {
        if (selectedPrefs != null) {
            Intent editConnectionIntent = new Intent(getApplicationContext(),
                    LdpActivityEditPreferences.class);
            editConnectionIntent.putExtra("IpAddress", selectedPrefs.getIPAddress());
            editConnectionIntent.putExtra("ConnectionName", selectedPrefs.getConnectionName());
            editConnectionIntent.putExtra("Password", selectedPrefs.getPassword());
            startActivity(editConnectionIntent);
        }
    }

    private void deleteCurrentPrefs() {
        complexPreferences.remove(selectedPrefs);
        recreateListView();
    }

    protected LdpConnectionPreferences getSelectedPrefs() {
        return selectedPrefs;
    }

}

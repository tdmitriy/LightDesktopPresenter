package com.ldp.androidclient.activities;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;

import com.ldp.androidclient.R;


public class ActivityMain extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main_menu_action, menu);
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch(item.getItemId()) {
            case R.id.settings_add_new_connection:
                //Toast.makeText(this, "Add new coonection clicked", Toast.LENGTH_SHORT).show();
                Intent newConnectionActivity = new Intent(this, ActivityNewConnection.class);
                startActivity(newConnectionActivity);
                break;
        }
        return super.onOptionsItemSelected(item);
    }
}

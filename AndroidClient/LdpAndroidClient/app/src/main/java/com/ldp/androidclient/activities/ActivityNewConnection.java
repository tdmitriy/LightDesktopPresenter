package com.ldp.androidclient.activities;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import com.ldp.androidclient.R;


public class ActivityNewConnection extends Activity implements View.OnClickListener {


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_new_connection);

        initControls();

    }

    private void initControls() {
        Button btnDone = (Button) findViewById(R.id.btnDone);
        btnDone.setOnClickListener(this);

        Button btnCancel = (Button) findViewById(R.id.btnCancel);
        btnCancel.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.btnDone:
                // TODO btnDone
                break;
            case R.id.btnCancel:
                finish();
                break;
            default:
                break;
        }
    }
}

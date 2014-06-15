package com.ldp.androidclient.activities;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

import com.ldp.androidclient.R;
import com.ldp.androidclient.controls.seek_arc.SeekArc;

public class LdpActivityVolumeControl extends Activity {

    private SeekArc volumeControl;
    private TextView volumeProgress;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_volume_control);
        volumeControl = (SeekArc) findViewById(R.id.volumeControl);
        volumeControl.setArcWidth(10);
        volumeControl.setProgressWidth(10);
        volumeControl.setRoundedEdges(true);
        volumeControl.setOnSeekArcChangeListener(new VolumeProgressListener());

        volumeProgress = (TextView) findViewById(R.id.lblVolumeProgress);
    }


    private final class VolumeProgressListener implements SeekArc.OnSeekArcChangeListener {

        @Override
        public void onProgressChanged(SeekArc seekArc, int progress, boolean fromUser) {
            volumeProgress.setText(String.valueOf(progress));
        }

        @Override
        public void onStartTrackingTouch(SeekArc seekArc) {

        }

        @Override
        public void onStopTrackingTouch(SeekArc seekArc) {

        }
    }
}

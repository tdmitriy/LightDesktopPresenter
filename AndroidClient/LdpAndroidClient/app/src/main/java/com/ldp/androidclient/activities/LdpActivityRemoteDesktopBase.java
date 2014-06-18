package com.ldp.androidclient.activities;

import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.text.InputType;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import com.ldp.androidclient.R;
import com.ldp.androidclient.command_sender.LdpCommandSender;
import com.ldp.androidclient.controls.edit_text.LdpKeyboardEditText;
import com.ldp.androidclient.controls.image_view.cursor_view.LdpCursorView;
import com.ldp.androidclient.controls.image_view.desktop_view.LdpRemoteImageView;
import com.ldp.androidclient.controls.popup_menu.ActionItem;
import com.ldp.androidclient.controls.popup_menu.QuickAction;
import com.ldp.androidclient.protocol.LdpProtocol.*;
import com.ldp.androidclient.screen_utils.LdpKeyboardController;

public class LdpActivityRemoteDesktopBase extends Activity implements View.OnClickListener {

    private static final String TAG = "LdpActivityRemoteDesktopBase";
    private LdpCommandSender commandSender;
    protected LdpRemoteImageView remoteImageView;
    protected LdpCursorView cursorView;
    private RelativeLayout image_container;
    private ImageButton settingsButton;
    private ImageButton keyboardButton;
    private EditText keyboardInputText;

    private static final int SHOW_WINDOWS_KEYBOARD = 1;
    private static final int SHOW_COMMANDS = 2;
    private QuickAction mQuickAction;

    private LdpKeyboardController keyboardController;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        commandSender = new LdpCommandSender();
        initializePopupMenu();
    }

    private void initializePopupMenu() {
        mQuickAction = new QuickAction(this);
        ActionItem showKeyboardItem = new ActionItem(SHOW_WINDOWS_KEYBOARD,
                "Windows keyboard", getResources().getDrawable(R.drawable.ic_action_keyboard));
        ActionItem showCommandsItem = new ActionItem(SHOW_COMMANDS,
                "Commands", getResources().getDrawable(R.drawable.ic_action_accounts));

        //mQuickAction.addActionItem(showKeyboardItem);
        mQuickAction.addActionItem(showCommandsItem);
        addPopupMenuListener();
    }

    private void addPopupMenuListener() {
        mQuickAction.setOnActionItemClickListener(new QuickAction.OnActionItemClickListener() {
            @Override
            public void onItemClick(QuickAction quickAction, int pos, int actionId) {
                //ActionItem actionItem = quickAction.getActionItem(pos);
                switch (actionId) {
                    case SHOW_WINDOWS_KEYBOARD:
                        commandSender.sendCommand(CommandType.SHOW_WINDOWS_KEYBOARD);
                        break;
                    case SHOW_COMMANDS:
                        showCommandsDialog();
                        break;
                }
            }
        });
    }

    private void showCommandsDialog() {
        final String REBOOT_PC = "Reboot PC";
        final String TURN_OFF_PC = "Turn off PC";
        final CharSequence[] items = {REBOOT_PC, TURN_OFF_PC};

        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Make your selection");
        builder.setItems(items, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int item) {
                switch (item) {
                    case 0:
                        confirmationDialog(item, REBOOT_PC);
                        break;
                    case 1:
                        confirmationDialog(item, TURN_OFF_PC);
                        break;
                }
            }
        });
        builder.setNegativeButton("Cancel", null);
        AlertDialog alert = builder.create();
        alert.show();
    }

    private void confirmationDialog(final int id, final String item) {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Are you sure?");
        builder.setMessage(item + "?");
        builder.setIcon(R.drawable.ic_action_warning);
        builder.setPositiveButton("YES", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                switch (id) {
                    case 0:
                        commandSender.sendCommand(CommandType.REBOOT_PC);
                        break;
                    case 1:
                        commandSender.sendCommand(CommandType.TURN_OFF_PC);
                        break;
                }
            }
        });
        builder.setNegativeButton("NO", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });
        AlertDialog alert = builder.create();
        alert.show();
    }

    protected void initializeMainControls(Bitmap background) {

        int backgroundW = background.getWidth();
        int backgroundH = background.getHeight();

        setContentView(R.layout.activity_remote_desktop);

        image_container = (RelativeLayout) findViewById(R.id.image_container);
        remoteImageView = (LdpRemoteImageView) findViewById(R.id.remoteImageView);
        cursorView = (LdpCursorView) findViewById(R.id.cursorView);
        keyboardButton = (ImageButton) findViewById(R.id.keyboardButton);
        settingsButton = (ImageButton) findViewById(R.id.settingsButton);
        keyboardInputText = (EditText) findViewById(R.id.keyboardInputText);

        ViewGroup.LayoutParams layoutParams = image_container.getLayoutParams();
        layoutParams.width = backgroundW;
        layoutParams.height = backgroundH;
        image_container.setLayoutParams(layoutParams);

        ViewGroup.LayoutParams remoteViewParams = remoteImageView.getLayoutParams();
        remoteViewParams.width = backgroundW;
        remoteViewParams.height = backgroundH;
        remoteImageView.setLayoutParams(remoteViewParams);

        ViewGroup.LayoutParams cursorViewParams = cursorView.getLayoutParams();
        cursorViewParams.width = backgroundW;
        cursorViewParams.height = backgroundH;
        cursorView.setLayoutParams(cursorViewParams);

        remoteImageView.setImageBitmap(background);
        cursorView.prepareView(this, backgroundW, backgroundH, remoteImageView);

        keyboardButton.setOnClickListener(this);
        settingsButton.setOnClickListener(this);

        keyboardController = new LdpKeyboardController(this, keyboardInputText);
    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        switch (event.getAction()) {
            case KeyEvent.ACTION_DOWN:
                switch (event.getKeyCode()) {
                    case KeyEvent.KEYCODE_DEL:
                        keyboardController.sendKeyboardInfoPacket("", KeyboardKey.KEY_DEL);
                        return true;
                    case KeyEvent.KEYCODE_ENTER:
                        keyboardController.sendKeyboardInfoPacket("", KeyboardKey.KEY_ENTER);
                        return true;
                }
                break;
            case KeyEvent.ACTION_UP:
                int code = event.getKeyCode();
                processNumber(code);
                break;
        }
        return super.dispatchKeyEvent(event);
    }

    private void sendKeyboardInfoPacket(String number) {
        keyboardController.sendKeyboardInfoPacket(number, KeyboardKey.KEY_TEXT);
    }

    private void processNumber(int code) {
        String key;
        switch (code) {
            case KeyEvent.KEYCODE_0:
                key = "0";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_1:
                key = "1";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_2:
                key = "2";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_3:
                key = "3";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_4:
                key = "4";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_5:
                key = "5";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_6:
                key = "6";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_7:
                key = "7";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_8:
                key = "8";
                sendKeyboardInfoPacket(key);
                break;
            case KeyEvent.KEYCODE_9:
                key = "9";
                sendKeyboardInfoPacket(key);
                break;
        }
    }

    @Override
    public void onClick(View v) {

        switch (v.getId()) {
            case R.id.keyboardButton:
                keyboardController.showKeyboard();
                break;
            case R.id.settingsButton:
                mQuickAction.show(v);
                break;
        }
    }
}

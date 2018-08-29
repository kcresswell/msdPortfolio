package com.example.mcresswell.androidchatapp;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;

import java.util.ArrayList;

public class LoginActivity extends AppCompatActivity {

    public static final String EXTRA_LOGIN = "aaa";
    public static final String EXTRA_ROOM = "bbb";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
    }

    /** Called when the user taps the Send button */
    public void sendMessage(View view) {
        Intent intent = new Intent(this, RoomActivity.class);
        EditText getLogin = (EditText) findViewById(R.id.editLogin);
        String login = getLogin.getText().toString();
        intent.putExtra(EXTRA_LOGIN, login);

        EditText getRoom = (EditText) findViewById(R.id.editRoom);
        String room = getRoom.getText().toString();
        intent.putExtra(EXTRA_ROOM, room);

        startActivity(intent);
    }

}



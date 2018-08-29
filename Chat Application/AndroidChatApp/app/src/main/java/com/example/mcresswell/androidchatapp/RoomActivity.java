package com.example.mcresswell.androidchatapp;

import android.content.Intent;
import android.os.Handler;
import android.support.constraint.ConstraintLayout;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;
import com.koushikdutta.async.http.AsyncHttpClient;
import com.koushikdutta.async.http.WebSocket;
import org.json.JSONException;
import org.json.JSONObject;

public class RoomActivity extends AppCompatActivity {

    private WebSocket webSkt;
    private String login;
    private String room;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_room);

        // Get the Intent that started this activity and extract the string
        Intent intent = getIntent();
         login = intent.getStringExtra(LoginActivity.EXTRA_LOGIN);
         room = intent.getStringExtra(LoginActivity.EXTRA_ROOM);

        // Capture the layout's TextView and set the string as its text
        TextView textView = findViewById(R.id.loginDisplay);
        textView.setText("Welcome, " + login + ", to " + room + "!");


        String ipAddress = "10.0.2.2";
        String url = "ws://" + ipAddress + ":8080";

        //WEB SOCKET CODE
        AsyncHttpClient.getDefaultInstance().websocket(url, "my-protocol", new AsyncHttpClient.WebSocketConnectCallback() {
            @Override
            public void onCompleted(Exception ex, WebSocket webSocket) {
                RoomActivity.this.webSkt = webSocket;
                webSkt.send("join " + room);

                if (ex != null) {
                    ex.printStackTrace();
                    return;
                }

                //webSocket.send(login + " " + room);
                webSocket.setStringCallback(new WebSocket.StringCallback() {
                    public void onStringAvailable(String s) {
                        receiveMessage(s);
                    }
                });
            }
        });
    }

    public void onSendMessage(View view){
        //when send message button is pressed
        TextView messageText = findViewById(R.id.msgToSend);
        String message = messageText.getText().toString();

        sendMessage(message);
    }


    public void sendMessage(String message){
        webSkt.send(login + ": " + message);
    }

    public void receiveMessage(final String message){
        try {
            JSONObject object = new JSONObject(message);

            final String messageString = object.get("message").toString();

            final String username = object.get("user").toString();



            Handler h = new Handler(this.getMainLooper());

            h.post(new Runnable() {
                @Override
                public void run() {
                    //add view stuff
                    LinearLayout linearLayout = findViewById(R.id.msgLinearLayout);

                    //constraint layout
                    ConstraintLayout cLayout = new ConstraintLayout(RoomActivity.this);
                    cLayout.setMaxHeight(100);
                    cLayout.setBackgroundColor(2550255);

                    linearLayout.addView(cLayout);

                    TextView messageToPost = new TextView(RoomActivity.this);
                    messageToPost.setText(username + " " + messageString); //messageToSend.getText().toString()

                    linearLayout.addView(messageToPost);
                }
            });

        } catch (JSONException e) {
            e.printStackTrace();
        }



    }
}

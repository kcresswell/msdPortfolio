"use strict"; 
let body = document.getElementsByTagName('body')[0];

//-----COOKIE CODE------

//read cookie data
let cookieData = document.cookie; 

//feed in a cookieKey such as username or room and it will return just that part of the array
function getCookie(cookieKey) {
    let keyName = cookieKey + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let cDataArray = decodedCookie.split(';');
    for(let i = 0; i <cDataArray.length; i++) {
        let cData = cDataArray[i];
        while (cData.charAt(0) == ' ') {
            cData = cData.substring(1);
        }
        if (cData.indexOf(keyName) == 0) {
            return cData.substring(keyName.length, cData.length);
        }
    }
    return "";
}

let username = getCookie("username");
let room = getCookie("room");

//----Test Code
let welcomeTag = document.createElement('p1');
let welcomeText = "Welcome " + username + " to room " + room + "!";
let welcomeMessage = document.createTextNode(welcomeText); 
welcomeTag.appendChild(welcomeMessage); 
body.appendChild(welcomeTag); 


//-----WEB SOCKET CODE------
let webSocketUrl = "ws://" + location.host;

let webSocket = new WebSocket(webSocketUrl);

//-----SEND A MESSAGE-----
let messageContent; 
let joinRoom = "join " + room; 

webSocket.onopen = function(event){
    webSocket.send(joinRoom);
} 

function clickHandler(clickEvent){
    messageContent = document.getElementById("message").value;
    webSocket.send(username + " " + messageContent);
    
    //set message text box to blank for the next message
    messageContent = document.getElementById("message").value = "";
}

webSocket.onmessage = function(event){
	console.log(event.data); 
    let messageObject = JSON.parse(event.data);
    let messageP = document.createElement('p');
    let messageText = messageObject.user + " sent:  " + messageObject.message;
	let messageNode = document.createTextNode(messageText);
	messageP.appendChild(messageNode);
    body.appendChild(messageP);

    console.log(messageObject);
}




// "use strict"; 
// let body = document.getElementsByTagName('body')[0];

// //-----COOKIE CODE------

// //feed in a cookieKey such as username or room and it will return just that part of the array
// function getCookie(cookieKey) {
//     let keyName = cookieKey + "=";
//     let decodedCookie = decodeURIComponent(document.cookie);
//     let cDataArray = decodedCookie.split(';');
//     for(let i = 0; i <cDataArray.length; i++) {
//         let cData = cDataArray[i];
//         while (cData.charAt(0) == ' ') {
//             cData = cData.substring(1);
//         }
//         if (cData.indexOf(keyName) == 0) {
//             return cData.substring(keyName.length, cData.length);
//         }
//     }
//     return "";
// }

// let username = getCookie("username");
// let room = getCookie("room");

// //----WELCOME MESSAGE, identifies user and room at top of page----
// window.onload = function(){
//     let welcomeTag = document.createElement('p1');
//     let welcomeText = "Welcome " + username + " to room " + room + "!";
//     let welcomeMessage = document.createTextNode(welcomeText); 
//     welcomeTag.appendChild(welcomeMessage); 
//     body.appendChild(welcomeTag); 
// }

// //-----WEB SOCKET CODE------
// let webSocketUrl = "ws://" + location.host;

// let webSocket = new WebSocket(webSocketUrl);

// let messageContent; 
// let joinRoom = "join " + room; 

// //MIGHT NEED TO SWITCH OR HANDLE THESE DIFFERENTLY, iF THE PERSON CLICKS REALLY FAST IT COULD MESS IT UP
// webSocket.onopen = function(event){
//     webSocket.send(joinRoom);
// } 

// function clickHandler(clickEvent){
//     messageContent = document.getElementById("messageInputBox").value;
//     webSocket.send(username + " " + messageContent);
// }
// ///////That one (look up)
// webSocket.onmessage = function(event){
//     let messageObject = JSON.parse(event.data);
//     let messageP = document.createElement('p');
//     messageP.className = "message";
//     let messageText = messageObject.user + " sent:  " + messageContent;
// 	let messageNode = document.createTextNode(messageText);
// 	messageP.appendChild(messageNode);
//     body.appendChild(messageP);

//     // console.log(messageObject);
// }
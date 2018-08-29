
let body = document.getElementsByTagName('body')[0];

function clickHandler(event){
	let username = document.getElementById("username").value;
    let roomName = document.getElementById("room").value;

    //create a cookie and set it
    document.cookie = "username=" + username + ";"; 
    document.cookie = "room=" + roomName + "; ";

    //Reroute to room.html page once the user is logged in
    window.location.href = "http://" + location.host + "/room.html";
}
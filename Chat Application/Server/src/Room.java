import java.io.IOException;
import java.sql.SQLException;
import java.util.ArrayList;

public class Room {
	private ArrayList<Connection> clients;
	private ArrayList<Message> messages;
	private String roomName;
	
	
	Room(String room){
		roomName = room;
		clients = new ArrayList<Connection>(); 
		messages = new ArrayList<Message>(); 
	}
	
	public String getRoomName() {
		return roomName;
	}

	public void addClient(Connection client) {
		clients.add(client);
		//for each message send to client -- write it to the client pipe
		//connection write to pipe
	}
	
	public void removeClient(Connection client) {
		clients.remove(client);
	}
	
	public synchronized void postMessage(Message msgToPost) throws IOException, SQLException {
		messages.add(msgToPost);
		System.out.println("Message from post message : " + msgToPost.getJSONMessage());
		

		for(Connection client: clients) {
			client.updateClientWithMessages(msgToPost);
		}
		
		
//		for(int i = 0; i < clientCount; i++) {
//			SQLiteJDBCConnection db = null; 
//			db.insertMessage(roomName, messages.get(i).toString());
//		}
	}
}
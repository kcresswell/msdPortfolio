import java.io.Serializable;

public class Message implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private String username; 
	private String message;
	
	public Message(String msg) {
		
			String[] msgArray = msg.split(" ", 2);
			username = msgArray[0];
			message = msgArray[1];
		
	}

	public String getUsername() {
		return username;
	}

	public String getMessage() {
		return message;
	}
	
	public String getJSONMessage() {
		return "{ \"user\":\"" + username + "\", \"message\":\"" + message + "\" }";
	}
}
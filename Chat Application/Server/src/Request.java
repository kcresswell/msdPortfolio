import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.net.Socket;
import java.util.HashMap;
import java.util.Scanner;

public class Request {
	private InputStream input = null; 
	private File pathToFile;
	private HashMap<String, String> headerHM  = new HashMap<String,String>();
	private String webSocketKey; 
	private boolean isWebSocket = false; 

	//IOException is due to the socket, which is handled in the Server class
	public Request(Socket inputSocket){
		try {
			input = inputSocket.getInputStream();

			//Get the Filename
			Scanner scanner = new Scanner(input);
			String file = scanner.nextLine();
			String[] name = file.split(" ");

			//if it's not a valid request, close the scanner
			if (!name[0].equals("GET") || !name[name.length-1].equals("HTTP/1.1")) {
				scanner.close();
			}

			//Filepath has to be within the resources folder
			String resourceFolder = "resources/" + name[1];
			pathToFile = new File(resourceFolder);

			handshakeRequest(scanner);

		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private void handshakeRequest(Scanner scanner) {
		while(true) {
			String headerLine = scanner.nextLine(); 

			if(headerLine.equals("")) {
				break;
			}

			String[] lineStrings = headerLine.split(": "); 

//			System.out.println("Line Strings Array: " + lineStrings);
			
			headerHM.put(lineStrings[0], lineStrings[1]);
//			
//			System.out.println("Line Strings [0]: " + lineStrings[0]);
//			System.out.println("Line Strings [1]: " + lineStrings[1]);
		}

		webSocketKey = headerHM.get("Sec-WebSocket-Key");

		if(headerHM.containsKey("Sec-WebSocket-Key")) {
			System.out.println("CONTAINS WEB SOCKET KEY");
			isWebSocket = true; 
//			System.out.println("Value of isWebSocket: " + isWebSocket);
		}
	}

	public boolean isWebSocket() {
		return isWebSocket;
	}

	public String getWebSocketKey() {
		return webSocketKey;
	}

	public File readFile(){
		return pathToFile;
	}

}
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.Socket;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Base64;

public class Response {
	private OutputStream output = null;
	private String mdAsB64;
	private Socket socket;

	
	//IOException is due to the socket, which is handled in the Server class
	public Response(Socket inputSocket) throws IOException{
		output = inputSocket.getOutputStream();
		socket = inputSocket; 
	}


	public void sendResponse(File myFile) throws InterruptedException {
		PrintWriter printWriter = new PrintWriter(output);
		
		if(myFile.exists()) {
			printWriter.print("HTTP/1.1 200 OK\r\n");
			printWriter.print("Content-Length: " + myFile.length() + "\r\n");
			printWriter.print("\r\n");
			printWriter.flush(); 

			try {
				//File Data
				FileInputStream fileInputStream = new FileInputStream(myFile);
				byte[] buffer = new byte[1024];
				int myBytes = 0;

				do {
					myBytes = fileInputStream.read(buffer);

					if(myBytes > 0) {
						output.write(buffer, 0, myBytes);

						//10.23.17 testing of threading
						//output.flush();
						//Thread.sleep(30);
					}
				}
				while(myBytes > 0);

				fileInputStream.close();
				output.flush();
			} catch (FileNotFoundException e) {
				e.printStackTrace();
				//HANDLE SOMEHOW
			} catch (IOException e) {
				e.printStackTrace();
			} 
		}

		else {
			printWriter.print("HTTP/1.1 404 ERROR\n");
			printWriter.flush();
		}

		printWriter.close();
	}
	
	
	public void sendWebSktResponse(String webSKey) {
		//Server Response to Client is the web socket key plus this magic string "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
		String sRespToClient = webSKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		try {
			handshake(sRespToClient);

//			while(!socket.isClosed()) {
//				MsgRequest msgReq = new MsgRequest(socket);
//				msgReq.readMessage(socket);
//			}

		} catch (Exception e) {
			e.printStackTrace();
		}  
	}
	
	public void handshake(String sRespToClient) throws NoSuchAlgorithmException {
		MessageDigest md;
		md = MessageDigest.getInstance("SHA-1");
		mdAsB64 = Base64.getEncoder().encodeToString(md.digest(sRespToClient.getBytes()));

		PrintWriter printWriter = new PrintWriter(output);

		printWriter.print("HTTP/1.1 101 Switching Protocols\r\n");
		printWriter.print("Upgrade: websocket\r\n");
		printWriter.print("Connection: Upgrade\r\n");
		printWriter.print("Sec-WebSocket-Accept: " + mdAsB64 + "\r\n");
		printWriter.print("\r\n");

		System.out.println(printWriter);
		printWriter.flush(); 
	}
}
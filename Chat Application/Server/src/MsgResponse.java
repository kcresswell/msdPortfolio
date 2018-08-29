import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

public class MsgResponse {
	String message = ""; 

	public MsgResponse(Socket socket, String msgText) throws IOException{

		DataOutputStream msgData = new DataOutputStream(socket.getOutputStream());

		message = msgText; 
		int msgLength = msgText.length();

		byte[] header = new byte[2];
		header[0] = (byte) 0x81; //FIN is 1 and the opcode is 0001 because it is a text message 

		if(msgLength < 126) {
			header[1] = (byte) msgLength;
		}
		else {
			header[1] = 126;
			byte[] moreLength = new byte[2];
			moreLength[0] = (byte) ((msgText.length()) / 256);  
			moreLength[1] = (byte) ((msgText.length()) % 256);  
		}

		for(int i = 0; i < header.length; i++) {
			msgData.writeByte(header[i]);
		}
		
		//writes the message to the web socket output stream
		msgData.write(msgText.getBytes());
	}
}

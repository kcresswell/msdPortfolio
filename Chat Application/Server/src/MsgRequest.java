import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;

public class MsgRequest {
	private String msgString; 
	private int messageLength = 0;
	private DataInputStream messageData; 
	
	MsgRequest(Socket socket) throws IOException{
		messageData = new DataInputStream(socket.getInputStream()); 
		
		
		//----FIRST BYTE----
		byte firstByte = messageData.readByte(); //FIN, RSV, RSV, RSV, Opcode
		byte top4Bits = (byte) ((byte) (firstByte >>> 4) & 0xF); //looks at FIN, RSV, RSV, RSV | we expect 1000 if it is FIN (server sees message as delivered if FIN is 1)
		byte opCode = (byte) (firstByte & 0xF); 

		if(top4Bits != 8) {
			//server should see message as delivered with opcode 8... opcode 1 is text message
			//socket.close(); 
		}

		if(opCode == 8) {
			socket.close(); 
		}

		//opcode == 1 at this point
		//----SECOND BYTE----
		byte secondByte = messageData.readByte();
		byte mask = (byte) ((secondByte >>> 7) &0x1);
		//GET LENGTH
		byte length = (byte) (secondByte & 0x7F);

		if(length < 126) {
			messageLength = length; 
		}
		else if(length == 126) {
//			//read two more bytes to get length
//			byte[] moreLength = new byte[2];
//			messageData.readFully(moreLength);
			messageLength = messageData.readUnsignedShort();

		}
		else { //length == 127
			//read 8 more bytes
//			byte[] moreLength = new byte[8];
//			messageData.readFully(moreLength);
		}
		
		readMessage(mask); 
		
	}
	
	public void readMessage(byte mask) throws IOException {
		
		byte[] msgAsBytes = new byte[messageLength];
		
		if(mask == 1) {//mask was bit shifted to 2^0 position or 1
			byte[] key = new byte[4]; //key is the first four bits a, b, c, d
			messageData.readFully(key);

			messageData.readFully(msgAsBytes);

			for(int i = 0; i < messageLength; i++) {
				msgAsBytes[i] ^= key[i%4];
			}

			msgString = new String(msgAsBytes);
		}

	}

	public String getMsgString() {
		return msgString;
	}
	
}

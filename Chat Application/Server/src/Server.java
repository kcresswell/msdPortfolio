import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.ServerSocketChannel;
import java.nio.channels.SocketChannel;
import java.sql.SQLException;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Set;

public class Server {
	private ServerSocketChannel svrSktChnl = null;
	private Selector slctr = null;
	private  HashMap<String, Room> roomObjs = new HashMap<String, Room>();
	private SQLiteJDBCConnection connect; 
	
	public Server(int portNumber) throws SQLException{
		try {
			svrSktChnl = ServerSocketChannel.open();
			svrSktChnl.bind(new InetSocketAddress(portNumber));
			slctr = Selector.open();
			svrSktChnl.configureBlocking(false);
			svrSktChnl.register(slctr, SelectionKey.OP_ACCEPT);
			
			//connect = new SQLiteJDBCConnection();
			
		} catch (IOException e1) {
			e1.printStackTrace();
		} 
	}

	public void serve(){
		while(true) {
			try {
				//ready to wait
				slctr.select(); //waits for at least one event
				
				Set<SelectionKey> selKeys = slctr.selectedKeys();
				Iterator<SelectionKey> iterator = selKeys.iterator(); 
				
				while(iterator.hasNext()) {
					SelectionKey key = iterator.next();
					
					if(key.isAcceptable()) {
						iterator.remove();
						SocketChannel sktChnl = svrSktChnl.accept();
						
						//Thread
						Connection threadReqResp = new Connection(sktChnl, this);
						new Thread(threadReqResp).start(); //this calls run in connection class
					}
					
				}
			} 
			catch (IOException e) {
				e.printStackTrace();
				System.exit(1);
				System.err.println("IO Exception");
			}
		}
	}
	
	public Room getRoom(String roomNm) { 
		
		if(roomObjs.containsKey(roomNm)) {
			return roomObjs.get(roomNm);
		}
		else {
			Room room = new Room(roomNm);
			roomObjs.put(roomNm, room);
			return room; 
		}
		
	}
}
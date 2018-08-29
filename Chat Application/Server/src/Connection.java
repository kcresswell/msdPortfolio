import java.io.File;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.nio.channels.Channels;
import java.nio.channels.Pipe;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.SocketChannel;
import java.sql.SQLException;
import java.util.Iterator;
import java.util.Set;

public class Connection implements Runnable {
	private SocketChannel sktChnl;
	private Server server;
	private Room roomObj;
	private Pipe pipeObj;
	private String roomName;

	Connection(SocketChannel socketChannel, Server svr) {
		this.sktChnl = socketChannel;
		this.server = svr;
	}

	@Override
	public void run() {
		try {
			// DETERMINE IF WS OR FILE
			Request request = new Request(sktChnl.socket());

			if (request.isWebSocket()) {
				Response response = new Response(sktChnl.socket());
				response.sendWebSktResponse(request.getWebSocketKey()); // this is where the handshake happens

				joinRoom();

				Selector selector = Selector.open();

				pipeObj = Pipe.open();
				pipeObj.source().configureBlocking(false);
				pipeObj.source().register(selector, SelectionKey.OP_READ);

				sktChnl.configureBlocking(false);
				sktChnl.register(selector, SelectionKey.OP_READ);

				while (!sktChnl.socket().isClosed()) {

					Message message;

					selector.select();

					Set<SelectionKey> selKeys = selector.selectedKeys();
					Iterator<SelectionKey> iterator = selKeys.iterator();

					while (iterator.hasNext()) {
						SelectionKey key = iterator.next();
						iterator.remove();

						if (key.channel() == sktChnl) {


							key.cancel(); // unregister the channel

							sktChnl.configureBlocking(true);

							//// IF FROM OUR USER HANDLING
							MsgRequest msgReq = new MsgRequest(sktChnl.socket());

							System.out.println("MESSAGE fed into message class: " + msgReq.getMsgString());
							message = new Message(msgReq.getMsgString());

							// msgReq.readMessage(socket);
							System.out.println("MESSAGE:" + message.toString());
							roomObj.postMessage(message);

							sktChnl.configureBlocking(false);
							selector.selectNow();
							sktChnl.register(selector, SelectionKey.OP_READ);
						}else {
							key.cancel();
							/// IF FROM ROOM HANDLING
							// pipe and socket must be in blocking mode here
							pipeObj.source().configureBlocking(true);

							sktChnl.keyFor(selector).cancel();
							sktChnl.configureBlocking(true);

							// read msg out of pipe
							ObjectInputStream msgInputStream = new ObjectInputStream(Channels.newInputStream(pipeObj.source()));
							Message msg = (Message) msgInputStream.readObject();

							System.out.println("JSON MESSAGE: " + msg.getJSONMessage());
							new MsgResponse(sktChnl.socket(), msg.getJSONMessage());

							// reregister socket, open pipe
							pipeObj.source().configureBlocking(false);
							selector.selectNow();
							pipeObj.source().register(selector, SelectionKey.OP_READ);

							// socket channel non blocking
							sktChnl.configureBlocking(false);
							selector.selectNow();
							sktChnl.register(selector, SelectionKey.OP_READ);
							break;
						}

					}
				}

			} else {
				Response response = new Response(sktChnl.socket());
				File myFile = request.readFile(); // void method
				response.sendResponse(myFile); // response.getFile in here
			}

		} catch (IOException | InterruptedException | ClassNotFoundException e) {
			e.printStackTrace();
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public void updateClientWithMessages(Message message) {
		try {
			System.out.println("update client method :" + message.getJSONMessage());
			ObjectOutputStream msgOutputStream = new ObjectOutputStream(Channels.newOutputStream(pipeObj.sink()));
			msgOutputStream.writeObject(message);
			msgOutputStream.flush();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}

	public void joinRoom() throws IOException {
		// first message request
		MsgRequest joinReq = new MsgRequest(sktChnl.socket());

		String joinRoomText = joinReq.getMsgString();
		String[] joinRoomArray = joinRoomText.split(" ");

		if (joinRoomArray[0].equals("join")) {
			roomName = joinRoomArray[1];

			roomObj = server.getRoom(roomName);
			roomObj.addClient(this);
		}

		System.out.println("Room Name: " + roomName);
	}
}
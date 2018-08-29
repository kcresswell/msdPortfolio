import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.Connection;
import java.sql.DatabaseMetaData;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;


public class SQLiteJDBCConnection {
	private Connection conn = null; 
	private String driverName; 
	private DatabaseMetaData metaData; 

	public SQLiteJDBCConnection() throws SQLException{
		String url = "jdbc:sqlite:/Server/sqlite/db/ChatServer.db";
		conn = DriverManager.getConnection(url);

		metaData = conn.getMetaData();
		driverName = metaData.getDriverName();

		createTable(); 
	}

	public DatabaseMetaData getMetaData() {
		return metaData;
	}


	public void createTable() throws SQLException {
		Statement stmt = conn.createStatement();

		String sql = "CREATE TABLE IF NOT EXISTS chat (\n"
				+ "	roomName text PRIMARY KEY,\n"
				+ "	message text);";

		stmt.execute(sql);
	}

	public void insertMessage(String roomName, String message) throws SQLException {

		String sql = "INSERT INTO chat(roomName,message) VALUES(?,?)";
		PreparedStatement pstmt = conn.prepareStatement(sql);
			pstmt.setString(1, roomName);
			pstmt.setString(2, message);
			pstmt.executeUpdate();
	}

	public ArrayList<Message> getMessages(String roomName) throws SQLException{
		ArrayList<Message> msgArray = new ArrayList<Message>(); 
		
		Statement stmt = conn.createStatement();
		
		String sql = "SELECT message FROM chat WHERE roomName =" + roomName + ";";
		
		ResultSet rs = stmt.executeQuery(sql);
		
		while(rs.next()) {
			String resultToAdd = rs.getString(1);
			//msgArray.add(resultToAdd);
		}
		
		
		return msgArray; 

	}

}


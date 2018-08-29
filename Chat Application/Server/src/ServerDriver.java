import java.sql.SQLException;

public class ServerDriver {
	public static void main(String[] args) throws SQLException{
		Server server = new Server(8080); 
		server.serve();
	}
}
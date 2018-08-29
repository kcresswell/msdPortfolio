
public class BadRequestException extends Exception{
	//default constructor - super
	public BadRequestException() {
		super();
	}
	
	//string constructor - super
	public BadRequestException(String errorStr) {
		super(errorStr);
	}
}
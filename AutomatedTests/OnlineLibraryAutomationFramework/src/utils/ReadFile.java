package utils;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class ReadFile  {

	public static final String PASS_FILE_PATH = "D:\\password.txt";   
	
	public static String readString(String filePath) throws IOException {
		return new String(Files.readAllBytes(Paths.get(filePath)));
	}

	
}
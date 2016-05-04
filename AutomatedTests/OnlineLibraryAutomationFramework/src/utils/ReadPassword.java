package utils;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

public class ReadPassword  {

	public static final String PASS_FILE_PATH = "D:\\password.txt";   
	
	public static String readPassword(String filePath) throws IOException {
		return new String(Files.readAllBytes(Paths.get(filePath)));
	}

	
}
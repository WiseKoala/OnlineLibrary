package logs;

import org.apache.log4j.xml.DOMConfigurator;

public class LoggingTc extends Log4jLogger {

	public LoggingTc() {
		
		
		 
        
 
			// Provide Log4j configuration settings
 
			DOMConfigurator.configure("log4j.xml");
 
			startTestCase("Name");
			endTestCase("End");
			
			
	
	}

}

package JunitTest;


import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import utils.UI;

public class superAdministratorLogin extends UI{

	
	@Before 
	public void openAplication (){
		super.openAplication("https://localhost:44300/power");
		initializeXpathProp();
		
	}
	
	@After
	public void closeAplication(){
		super.closeAplication();
		openLogFile();
	}
	
	@Test 
	public void test1(){

	
	inputInto("poe-c(cU", "passwordInputSuperAdminPage");
	
	clickItem("okButtonSuperAdminPage");
		
	}
	

}

package JunitTest;


import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import utils.UI;

public class test extends UI{

	
	@Before 
	public void openAplication (){
		super.openAplication("https://localhost:44300/");
		initializeXpathProp();
		
	}
	
	@After
	public void closeAplication(){
		//super.closeAplication();
	}
	
	@Test 
	public void test1(){
	
	clickItem("liblogo");
	
	clickItem("homeMenuBarItem");
	
	clickItem("rolesMenuBarItem");
	
	clickItem("loginMenuBarItem");
	
	clickItem("loginButtonLoginPage");
	
	inputInto("dimaturcan96@gmail.com", "loginInputGoogleLoginPage");
	
	clickItem("googleLoginNextButton");
	
	inputInto("LM78bnc32K", "passwordInputGoogleLoginPage");
	
	clickItem("googleSigninButton");
	
	temporar();
	
	
	
	
	
	
		
		
	}
	

}

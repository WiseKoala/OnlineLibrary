package JunitTest;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.remote.UnreachableBrowserException;

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
	
	
	@Test // Pavel Vacari TC 01012301923- First User Login
	
	public void tc01123123(){
		
		clickItem("loginMenuBarItem");
		
		clickItem("loginButtonLoginPage");
		
		inputInto("aurelas188", "loginInputGoogleLoginPage");
		
		clickItem("googleLoginNextButton");
		
		inputInto("careersendava", "passwordInputGoogleLoginPage");
		
		clickItem("googleSigninButton");
		
		clickItem("allowAccesGoogleAccount");
		
		clickItem("registerButton");
		
		changeRole("admin");
		
		clickItem("addToRoleCheckBox");
		
		clickItem("saveButtonEditRolePage");
		
		clickItem("homeMenuBarItem");
		
		clickItem("logOfMenuBar");
		
	}
	
	
	
	@Test
	public void test1() throws UnreachableBrowserException{
	
	clickItem("liblogo");
	
	clickItem("homeMenuBarItem");
	
	clickItem("loginMenuBarItem");
	
	clickItem("loginButtonLoginPage");
	
	inputInto("libraryproject11@gmail.com", "loginInputGoogleLoginPage");
	
	clickItem("googleLoginNextButton");
	
	inputInto("careerss", "passwordInputGoogleLoginPage");
	
	clickItem("googleSigninButton");
	
	clickItem("allowAccesGoogleAccount");
	
	clickRandomBook(6);
	
	
		
	}
	

	
	
	
	
	
}

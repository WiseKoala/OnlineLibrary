package JunitTest;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.By;

import static org.junit.Assert.*;

import utils.UI;
import aserts.Asert;




public class TC_First_user_login_as_Sys_admin extends UI{

	
	@Before 
	public void openAplication (){
		super.openAplication("https://localhost:44300");
		initializeXpathProp();
		
	}
	
	@After
	public void closeAplication(){
		//super.closeAplication();
		openLogFile();
		
	}
	
	
	
	
	@Test 
	
	public void test1(){
		
		clickItem("loginMenuBarItem");		
		
		clickItem("loginButtonLoginPage");
		
		inputInto("aurelas188", "loginInputGoogleLoginPage");
		
		clickItem("googleLoginNextButton");
		
		inputInto("careersendava", "passwordInputGoogleLoginPage");
		
		clickItem("googleSigninButton");
		
		clickItem("allowAccesGoogleAccount");
		
		clickItem("registerButton");
		
		clickItem("editButtonSystemAdminRolesPage");
		
		clickItem("addToRoleCheckBox");
		
		clickItem("saveButtonEditRolePage");
		
		clickItem("homeMenuBarItem");
		
		clickRandomBook(6);
		
		clickItem("logOfMenuBar");
		
	}
	
	
	

	
	
}

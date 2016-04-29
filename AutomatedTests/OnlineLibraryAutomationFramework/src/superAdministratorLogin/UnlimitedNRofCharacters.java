package superAdministratorLogin;


import org.apache.http.util.Asserts;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.testng.Assert;
import org.testng.AssertJUnit;
import static org.testng.Assert.*;


import utils.UI;

public class UnlimitedNRofCharacters extends UI{

	
	@Before 
	public void openAplication (){
		super.openAplication("https://localhost:44300/power");
		initializeXpathProp();
		
	}
	
	@After
	public void closeAplication(){
		super.closeAplication();
		//openLogFile();
	}
	
	@Test 
	public void test1(){

	
	inputInto("1111111111111111111111", "passwordInputSuperAdminPage");
	
	clickItem("okButtonSuperAdminPage");
	String URL = driver.getCurrentUrl();
	Assert.assertEquals(URL, "https://localhost:44300/power" );
	
	assertTrue(driver.getPageSource().contains("Enter the password:"), "Text found");
	
	}
	

}

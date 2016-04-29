package superAdministratorLogin;


import org.apache.http.util.Asserts;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.lift.find.PageTitleFinder;
import org.testng.Assert;
import org.testng.AssertJUnit;
import static org.testng.Assert.*;
import utils.UI;

public class EmptyPasswordField extends UI{

	
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

	
	//inputInto("g-Hs[dQl", "passwordInputSuperAdminPage");
	
	clickItem("okButtonSuperAdminPage");
	String URL = driver.getCurrentUrl();
	Assert.assertEquals(URL, "https://localhost:44300/power" );
	
	String title = driver.getTitle();
	assertTrue(title.contains("Value"));

	}
	

}

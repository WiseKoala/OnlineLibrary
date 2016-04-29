package superAdministratorLogin;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.WebElement;
import static org.testng.Assert.*;

import utils.UI;

public class SuccessfulLogin extends UI {

	@Before
	public void openAplication() {
		super.openAplication("https://localhost:44300/power");
		initializeXpathProp();

	}

	@After
	public void closeAplication() {
		super.closeAplication();
		openLogFile();
	}

	@Test
	public void test1() {
		// given
		
		// when
		inputInto("g-Hs[dQl", "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		//String URL = driver.getCurrentUrl();
		//Assert.assertEquals(URL, "https://localhost:44300");

		//then expect
		String xpath = "helloSuperAdmin";
		WebElement usernameResult = getElementByPropertiesKey(xpath);
		assertEquals(usernameResult.getText(), "Hello,");
	}

}

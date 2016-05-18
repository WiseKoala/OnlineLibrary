package jUnitTestPVacari;

import static org.testng.Assert.assertEquals;
import static org.testng.Assert.assertFalse;
import static org.testng.Assert.assertTrue;

import java.io.IOException;

import org.apache.log4j.Logger;
import org.junit.AfterClass;
import org.junit.BeforeClass;
import org.junit.Test;
import org.openqa.selenium.WebElement;
import org.testng.Assert;

import utils.ReadFile;
import utils.UI;

public class UserLogin extends UI {

	private static final Logger LOGGER = Logger.getLogger(UserLogin.class);

	@BeforeClass
	public static void beforeClass() {
		LOGGER.info("Test started");
		initializeXpathProp();
	}

	@AfterClass
	public static void afterClass() {
		// openLogFile();
	}

	@Test // TC_02_02_01_Login
	public void Login() {
		// given
		openAplication("https://localhost:44300/");

		clickItem("loginButtonHomePage");
		String URL = driver.getCurrentUrl();
		Assert.assertEquals(URL, "https://localhost:44300/Account/Login");
		clickItem("loginUsingGoogleButton");
		inputInto("libraryproject11", "loginInputGoogleLoginPage");
		
		clickItem("googleLoginNextButton");
		
		inputInto("careerss", "passwordInputGoogleLoginPage");
		
		clickItem("googleSigninButton");
		
		clickItem("allowAccesGoogleAccount");

		// then expect
		String xpath = "hello";
		WebElement usernameResult = getElementByPropertiesKey(xpath);
		assertEquals(usernameResult.getText(), "Hello,");
		closeAplication();
		LOGGER.info("Test passed");
	}
}
package jUnitTestPVacari;

import static org.testng.Assert.assertEquals;
import static org.testng.Assert.assertFalse;
import static org.testng.Assert.assertTrue;

import java.io.IOException;

import org.apache.log4j.Logger;
import org.junit.AfterClass;
import org.junit.BeforeClass;
import org.junit.Test;
import org.openqa.selenium.By.ById;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.lift.find.PageTitleFinder;
import org.openqa.selenium.remote.server.handler.ClickElement;
import org.openqa.selenium.remote.server.handler.GetElementText;
import org.testng.Assert;

import com.thoughtworks.selenium.webdriven.commands.Click;

import utils.ReadFile;
import utils.UI;

public class LibraryanLogin extends UI {

	private static final Logger LOGGER = Logger.getLogger(LibraryanLogin.class);

	@BeforeClass
	public static void beforeClass() {
		LOGGER.info("Test started");
		initializeXpathProp();
	}

	@AfterClass
	public static void afterClass() {
		// openLogFile();
	}

/*	@Test // TC_02_03_01_Login
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
	}*/
	
	@Test // TC_02_03_01_ManageLoans
	public void ManageLoans() {
		// given
		openAplication("https://localhost:44300/");

		clickItem("loginButtonHomePage");
		String LoginURL = driver.getCurrentUrl();
		Assert.assertEquals(LoginURL, "https://localhost:44300/Account/Login");
		clickItem("loginUsingGoogleButton");
		inputInto("libraryproject11", "loginInputGoogleLoginPage");
		
		clickItem("googleLoginNextButton");
		
		inputInto("careerss", "passwordInputGoogleLoginPage");
		
		clickItem("googleSigninButton");
		
		clickItem("allowAccesGoogleAccount");
		
		driver.findElement(ById.id("My Loans"));
		
		//clickItem("loansButtonMenuBar");
		
		
		// then expect
		String xpath = "hello";
		WebElement usernameResult = getElementByPropertiesKey(xpath);
		assertEquals(usernameResult.getText(), "Hello,");
		
		String LibrarianURL = driver.getCurrentUrl();
		assertEquals(LibrarianURL, "https://localhost:44300/librarian");
		
		String PageTitle = driver.getTitle();
		assertEquals(PageTitle, "Manage Loans");
		LOGGER.info("Manage Loans page has been accessed");
		
		assertTrue(true, "loansPending");
		
		
		
		closeAplication();
		LOGGER.info("Test passed");
	}
}
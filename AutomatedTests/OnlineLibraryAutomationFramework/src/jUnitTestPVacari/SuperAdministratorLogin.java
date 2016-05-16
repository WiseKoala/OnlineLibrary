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

public class SuperAdministratorLogin extends UI {
	
	private static final Logger LOGGER = Logger.getLogger(SuperAdministratorLogin.class);
	
	static String password;
	
	

	@BeforeClass
	public static void beforeClass() {
		LOGGER.info("Test started");
		initializeXpathProp();
		try {
			password = ReadFile.readString(ReadFile.PASS_FILE_PATH);
		} catch (IOException e) {
			LOGGER.error("Cannot read file with password.");
		}
	}

	@AfterClass
	public static void afterClass() {
		//openLogFile();
	}

	@Test // TC_02_05_01_01_Successful_login
	public void SuccessfullLogin() {
		// given
		openAplication("https://localhost:44300/power");
		// when
		inputInto(password, "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		// String URL = driver.getCurrentUrl();
		// Assert.assertEquals(URL, "https://localhost:44300");

		// then expect
		String xpath = "helloSuperAdmin";
		WebElement usernameResult = getElementByPropertiesKey(xpath);
		assertEquals(usernameResult.getText(), "Hello,");
		closeAplication();
		LOGGER.info("Test passed");
	}

	@Test // TC_02_05_01_02_Empty password field
	public void EmptyPasswordField() {
		openAplication("https://localhost:44300/power");

		// inputInto("g-Hs[dQl", "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		String URL = driver.getCurrentUrl();
		Assert.assertEquals(URL, "https://localhost:44300/power");

		String title = driver.getTitle();
		assertTrue(title.contains("Super Administrator"));
		assertTrue(driver.getPageSource().contains("Password can not be empty"), "Text found");
		closeAplication();
	}

	@Test // TC_02_05_01_05_Incorrect password(x+pass+x)
	public void IncorrectPassword3() {
		openAplication("https://localhost:44300/power");

		inputInto("1111111111111111111111" + password + "1111111111111111111111", "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		String URL = driver.getCurrentUrl();
		Assert.assertEquals(URL, "https://localhost:44300/power");

		assertTrue(driver.getPageSource().contains("Enter the password:"), "Text found");
		closeAplication();

	}

	@Test // TC_02_05_01_04_Incorrect password(pass+x)
	public void IncorrectPassword2() {
		openAplication("https://localhost:44300/power");

		inputInto(password + "1111111111111111111111", "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		String URL = driver.getCurrentUrl();
		Assert.assertEquals(URL, "https://localhost:44300/power");

		assertTrue(driver.getPageSource().contains("Enter the password:"), "Text found");
		closeAplication();

	}

	@Test // TC_02_05_01_03_Incorrect password(x+pass)
	public void IncorrectPassword1() {
		openAplication("https://localhost:44300/power");

		inputInto("1111111111111111111111" + password, "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		String URL = driver.getCurrentUrl();
		Assert.assertEquals(URL, "https://localhost:44300/power");

		assertTrue(driver.getPageSource().contains("Enter the password:"), "Text found");
		closeAplication();

	}

	@Test // TC_02_05_01_06_Crypted password
	public void CryptedPassword() {
		openAplication("https://localhost:44300/power");

		inputInto(password, "passwordInputSuperAdminPage");

		assertFalse(driver.getPageSource().contains(password), "Password not found");

		// verifica daaca pe pagina este vizibila parola introdusa
		// if(driver.getPageSource().contains(""))
		// {
		// log.debug("Parola : este");
		// // System.out.println("am gasit");
		// }
		//
		// else
		// {
		// log.debug("Parola : nu-i");
		// // System.out.println("nu am gasit");
		// }
		//

		// clickItem("okButtonSuperAdminPage");
		// String URL = driver.getCurrentUrl();
		// Assert.assertEquals(URL, "https://localhost:44300" );
		closeAplication();
	}

	@Test // TC_02_05_07_Unlimited nr of characters
	public void UnlimitedNRofCharacters() {
		openAplication("https://localhost:44300/power");

		inputInto("1111111111111111111111", "passwordInputSuperAdminPage");

		clickItem("okButtonSuperAdminPage");
		String URL = driver.getCurrentUrl();
		Assert.assertEquals(URL, "https://localhost:44300/power");

		assertTrue(driver.getPageSource().contains("Enter the password:"), "Text found");
		closeAplication();

	}

}

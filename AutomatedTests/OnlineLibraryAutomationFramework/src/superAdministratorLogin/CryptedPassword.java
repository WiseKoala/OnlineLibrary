package superAdministratorLogin;

import org.apache.http.util.Asserts;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import static org.testng.Assert.*;
import org.testng.AssertJUnit;

import utils.UI;

public class CryptedPassword extends UI {

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

		String password = "g-Hs[dQl";

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

	}

}

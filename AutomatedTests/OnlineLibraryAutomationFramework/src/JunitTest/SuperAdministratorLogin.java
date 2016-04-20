package JunitTest;


import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import utils.UI;

public class SuperAdministratorLogin extends UI{

	
	@Before 
	public void openAplication (){
		super.openAplication("https://localhost:44300/power");
		initializeXpathProp();
		
	}
	
	@After
	public void closeAplication(){
		super.closeAplication();
	}
	
	@Test 
	public void test1(){

	
	inputInto("//pass//", "//passField//");
	
	clickItem("OKButton");
	
	
	
	temporar();
	
	
	
	
	
	
		
		
	}
	

}

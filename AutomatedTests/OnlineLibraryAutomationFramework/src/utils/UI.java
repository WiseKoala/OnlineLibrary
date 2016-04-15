package utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Properties;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.firefox.FirefoxDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;


public class UI {

	static Properties prop = new Properties();
	
	public static WebDriver driver = new FirefoxDriver(); 
																//declarations
	
	public void openAplication (String link){
		
		driver.get(link);
		System.out.println("The aplication with the address:'"+link+"' has oppened in Firefox...");
		
	}

	public void closeAplication(){
		
		driver.close();
		System.out.println("The aplication is closed...");
		
	}
	
	public  void initializeXpathProp(){
		
		File file = new File("xpath.properties");
		  
		FileInputStream fileInput = null;
		try {
			fileInput = new FileInputStream(file);
			
		} catch (FileNotFoundException e) {
			
			e.printStackTrace();
		}
		
		try {
			prop.load(fileInput);
			
		} catch (IOException e) {
			
			e.printStackTrace();
		}
	
		System.out.println("The 'xpath.properties' file was initialized with success...");
		
	}

	public void clickItem (String xpathProperty){
		
		waitElement(xpathProperty);
		driver.findElement(By.xpath(prop.getProperty(xpathProperty))).click();
		System.out.println(xpathProperty + " was clicked");
		
		
		
	}
	
	public void clickItem (String xpathProperty, String externalSourceElement){
	
		driver.findElement(By.xpath(prop.getProperty(xpathProperty))).click();
		System.out.println(xpathProperty + " was clicked");	
		waitElement(externalSourceElement);
		
	}
	
	public void inputInto (String value, String element){
		
		waitElement(element);
		driver.findElement(By.xpath(prop.getProperty(element))).sendKeys(value);
		System.out.println("The input data '"+value+ "' was successfuly added into '"+element+"' element...");
		
		
		
		
	}
	
	
	
	
	
	public void temporar(){
		
		try {
			Thread.sleep(5000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		driver.findElement(By.id("submit_approve_access")).click();
		
	}
	
	private static void waitElement(String element){
		
		WebDriverWait wait = new WebDriverWait(driver, 10000);
		
		wait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath(prop.getProperty(element)))); 
		
		System.out.println(element + " has loaded...");
		
		
		
	}
	
	
	
	
}

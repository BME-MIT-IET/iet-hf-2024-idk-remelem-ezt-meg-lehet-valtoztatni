from selenium.webdriver.common.by import By
from selenium import webdriver
from selenium.webdriver.common.keys import Keys
import time

def test_adminlogin(driver):
    # Find and click the Login button
    login_link = driver.find_element(By.LINK_TEXT, "BEJELENTKEZÉS")
    login_link.click()
    time.sleep(2)

    #Write Password and email of the account
    email_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Email')]/following-sibling::div//input")
    password_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Password')]/following-sibling::div//input")
    login_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Login')]")
    email_input.send_keys("admin@admin.com")
    time.sleep(2)
    password_input.send_keys("ADMIN_admin")
    time.sleep(2)
    login_button.click()
    time.sleep(2)

    #Check if the name of the user is admin on the logged in page
    user_name = driver.find_element(By.CLASS_NAME, "MuiTypography-root")
    assert "admin" in user_name.text

if __name__ == "__main__":
    test_adminlogin(webdriver.Chrome)
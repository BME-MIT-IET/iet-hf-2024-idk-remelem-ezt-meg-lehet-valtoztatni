from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
import time

def test_registration(driver):
    login_link = driver.find_element(By.LINK_TEXT, "BEJELENTKEZÉS")
    login_link.click()
    time.sleep(2)

    email_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Email')]/following-sibling::div//input")
    password_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Password')]/following-sibling::div//input")
    login_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Login')]")
    email_input.send_keys("ujfelhasznalo@gmail.com")
    time.sleep(2)
    password_input.send_keys("UjFelhasznalo123")
    time.sleep(2)
    login_button.click()
    time.sleep(2)

    error_message = driver.find_element(By.TAG_NAME, "h1")
    assert "váratlan hiba törént" in error_message.text.lower()

    registration_link = driver.find_element(By.LINK_TEXT, "REGISZTRÁLÁS")
    registration_link.click()
    time.sleep(2)

    name_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Name')]/following-sibling::div//input")
    email_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Email')]/following-sibling::div//input")
    password_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Password')]/following-sibling::div//input")
    register_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Register')]")
    name_input.send_keys("UjFelhasznalo")
    time.sleep(2)
    email_input.send_keys("ujfelhasznalo@gmail.com")
    time.sleep(2)
    password_input.send_keys("UjFelhasznalo123")
    time.sleep(2)
    register_button.click()
    time.sleep(2)

    user_name = driver.find_element(By.CLASS_NAME, "MuiTypography-root")
    assert "UjFelhasznalo" in user_name.text

    logout_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Kijelentkezés')]")
    logout_button.click()
    time.sleep(2)

    login_link = driver.find_element(By.LINK_TEXT, "BEJELENTKEZÉS")
    login_link.click()
    time.sleep(2)

    email_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Email')]/following-sibling::div//input")
    password_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Password')]/following-sibling::div//input")
    login_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Login')]")
    email_input.send_keys("ujfelhasznalo@gmail.com")
    time.sleep(2)
    password_input.send_keys("UjFelhasznalo123")
    time.sleep(2)
    login_button.click()
    time.sleep(2)

    user_name = driver.find_element(By.CLASS_NAME, "MuiTypography-root")
    assert "UjFelhasznalo" in user_name.text

if __name__ == "__main__":
    test_registration(webdriver.Chrome)

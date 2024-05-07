from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
import time

def test_userlogin(driver):
    login_link = driver.find_element(By.LINK_TEXT, "BEJELENTKEZÉS")
    login_link.click()
    time.sleep(2)

    email_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Email')]/following-sibling::div//input")
    password_input = driver.find_element(By.XPATH, "//label[contains(text(), 'Password')]/following-sibling::div//input")
    login_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Login')]")
    email_input.send_keys("user@user.com")
    time.sleep(2)
    password_input.send_keys("USER_user")
    time.sleep(2)
    login_button.click()
    time.sleep(2)

    user_name = driver.find_element(By.CLASS_NAME, "MuiTypography-root")
    assert "user" in user_name.text

    if __name__ == "__main__":
        test_userlogin()
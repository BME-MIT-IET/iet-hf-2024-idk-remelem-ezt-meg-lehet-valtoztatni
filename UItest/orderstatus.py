from selenium import webdriver
from selenium.webdriver import ActionChains
import time
import ProductsToCart
from selenium.webdriver.common.by import By
import order
import adminlogin
import userlogin

def test_orderstatus(driver):
    #Place an order
    order.test_order(driver)
    time.sleep(2)

    #Log out of the user account
    logout_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Kijelentkezés')]")
    logout_button.click()
    time.sleep(2)

    #Log in to the admin account
    adminlogin.test_adminlogin(driver)
    time.sleep(2)

    # Find and click the Order button
    order_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Megrendelések')]")
    order_button.click()
    time.sleep(2)

    # Find and click the Processing button
    processing_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Processing')]")
    processing_button.click()
    time.sleep(2)

    paragraphs = driver.find_elements(By.CSS_SELECTOR, "p")
    # Select the last paragraph
    last_paragraph = paragraphs[-1].text if paragraphs else ""
    assert "Processing" in last_paragraph, f"Expected 'Processing' to be part of the text, but got '{last_paragraph}'."

    # Find and click the Logout button
    logout_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Kijelentkezés')]")
    logout_button.click()
    time.sleep(2)

    #Log in with user account
    userlogin.test_userlogin(driver)
    time.sleep(2)

    # Find and click the Order button
    order_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Megrendelések')]")
    order_button.click()
    time.sleep(2)

    paragraphs = driver.find_elements(By.CSS_SELECTOR, "p")
    # Select the last paragraph
    last_paragraph = paragraphs[-1].text if paragraphs else ""
    assert "Processing" in last_paragraph, f"Expected 'Processing' to be part of the text, but got '{last_paragraph}'."


if __name__ == "__main__":
    test_orderstatus(webdriver.Chrome)
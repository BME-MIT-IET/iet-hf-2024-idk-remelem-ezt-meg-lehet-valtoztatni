from selenium import webdriver
from selenium.webdriver import ActionChains
import time
import ProductsToCart
from selenium.webdriver.common.by import By
from selenium.common.exceptions import NoSuchElementException

def test_order(driver):
    #Put some products in the cart
    ProductsToCart.test_cart(driver)
    time.sleep(2)

    #Find and click the Order button
    order_button = driver.find_element(By.XPATH, "//button[contains(text(), 'Megrendel')]")
    order_button.click()
    time.sleep(2)

    #Find the product names of the list
    items = driver.find_elements(By.CSS_SELECTOR, "main ul > li p:first-child")
    # Extract text and store in a list
    item_texts = [item.text for item in items]
    # Check for 'Sajt' and 'Roller' in the list of items
    assert 'Sajt' in item_texts, "Sajt is not in the list."
    assert 'Roller' in item_texts, "Roller is not in the list."
    print("Both Sajt and Roller are correctly listed.")

    paragraphs = driver.find_elements(By.CSS_SELECTOR, "p")
    # Select the last paragraph
    last_paragraph = paragraphs[-1].text if paragraphs else ""
    assert "Unread" in last_paragraph, f"Expected 'Unread' to be part of the text, but got '{last_paragraph}'."

    #Find and click the Cart button
    basket_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Kosár')]")
    basket_button.click()
    time.sleep(2)

    #The table needs to be empty because we have already ordered the items
    table = driver.find_element(By.CSS_SELECTOR, "main table")
    # Find all rows in the tbody of the table
    rows = table.find_elements(By.CSS_SELECTOR, "tbody tr")
    # Check if the tbody has no rows, meaning the table is empty except for the header
    assert len(rows) == 0, "The table is not empty; it has additional rows."
    print("The table is correctly empty")

    #Find and click the Order button
    order_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Megrendelések')]")
    order_button.click()
    time.sleep(2)

    #Find and click the Receipt button
    receipt_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Számla')]")
    receipt_button.click()
    time.sleep(2)


if __name__ == "__main__":
    test_order(webdriver.Chrome)
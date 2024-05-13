from selenium import webdriver
from selenium.webdriver import ActionChains
import time
import userlogin
from selenium.webdriver.common.by import By
from selenium.common.exceptions import NoSuchElementException

#Used to check if given items are visible after the price interval has changed
def check_item_visibility(driver, item_text, expected_visibility=True):
    try:
        item = driver.find_element(By.XPATH, f"//*[contains(text(), '{item_text}')]")
        if not expected_visibility:
            raise AssertionError(f"Item '{item_text}' should not be visible.")
    except NoSuchElementException:
        if expected_visibility:
            raise AssertionError(f"Item '{item_text}' should be visible.")


#Drags the price interval slider to the wanted value. slider_index tells if we want to move the lower or the higher handler of the slider
def drag_slider_to_value(driver, slider_value, slider_index):
    try:
        # Find the slider handle for the specified index
        slider_input = driver.find_element(By.XPATH, f"//input[@type='range'][@data-index='{slider_index}']")
        slider_track = driver.find_element(By.XPATH, "//span[contains(@class, 'MuiSlider-rail')]")

        # Calculate the offset to drag based on the desired value and slider's max value
        slider_width = slider_track.size['width']
        max_value = int(slider_input.get_attribute('max')) * 1000
        current_value = int(slider_input.get_attribute('value')) * 1000
        desired_value = int(slider_value)

        # Debug output
        print(f"Current position: {current_value}, Desired position: {desired_value}, Max value: {max_value}")

        # Calculate the new position on the slider
        target_position = (desired_value / max_value) * slider_width
        current_position = (current_value / max_value) * slider_width
        offset = int(target_position - current_position)

        # Debug output
        print(f"Target pixel position: {target_position}, Current pixel position: {current_position}, Offset: {offset}")

        # Perform the drag and drop action using ActionChains
        action = ActionChains(driver)
        action.click_and_hold(slider_input).move_by_offset(offset, 0).release().perform()

        print(f"Slider {slider_index} attempted to set to {slider_value}.")
    except Exception as e:
        print(f"Failed to drag the slider to set the value: {str(e)}")


#Checks the items and their quantities in a table. expected_items is a set containing the expected results
def check_items_and_quantities(driver, expected_items):

    # Find all rows in the table
    rows = driver.find_elements(By.CSS_SELECTOR, "table > tbody > tr")

    # Dictionary to hold the actual quantities found
    found_items = {}

    # Iterate over each row and extract items and quantities
    for row in rows:
        cells = row.find_elements(By.TAG_NAME, "td")
        if cells:
            item_name = cells[0].text.strip()  # Ensure text is clean without extra whitespaces
            quantity = cells[3].text.strip()

            if item_name in expected_items:
                found_items[item_name] = quantity

    # Check if found items match expected quantities
    for item, quantity in expected_items.items():
        if item in found_items and found_items[item] == quantity:
            print(f"Item {item} with quantity {quantity} is correctly listed.")
        else:
            print(f"Item {item} with quantity {quantity} is missing or incorrect.")

def test_cart(driver):
    #Log in with user account
    userlogin.test_userlogin(driver)
    time.sleep(2)

    # Find and click the Products button
    products_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Termékek')]")
    products_button.click()
    time.sleep(2)

    # Find and click the Add button of the Cheese product
    add_cheese_button = driver.find_element(By.XPATH, "//td[text()='Sajt']/following-sibling::td/button")
    add_cheese_button.click()
    time.sleep(2)

    # Find and click the Food button
    food_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Élelmiszer')]")
    food_button.click()
    time.sleep(2)
    check_item_visibility(driver, "Sajt", expected_visibility=True)  # Expect Sajt to be visible
    check_item_visibility(driver, "Alaplap", expected_visibility=False)  # Expect Alaplap not to be visible

    # Find and click the Add button of the Cheese product
    add_cheese_button = driver.find_element(By.XPATH, "//td[text()='Sajt']/following-sibling::td/button")
    add_cheese_button.click()
    time.sleep(2)

    # Find and click the Electronics button
    electronics_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Elektronika')]")
    electronics_button.click()
    time.sleep(2)
    check_item_visibility(driver, "Sajt", expected_visibility=False)  # Expect Sajt not to be visible
    check_item_visibility(driver, "Alaplap", expected_visibility=True)  # Expect Alaplap to be visible

    #Drag the slider
    drag_slider_to_value(driver, '79000', 1)
    time.sleep(2)
    check_item_visibility(driver, "Processzor", expected_visibility=False)
    time.sleep(2)

    # Find and click the Add button of the Motherboard product
    add_motherboard_button = driver.find_element(By.XPATH, "//td[text()='Alaplap']/following-sibling::td/button")
    add_motherboard_button.click()
    time.sleep(2)

    # Find and click the Sport button
    sport_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Sport')]")
    sport_button.click()
    time.sleep(2)

    # Drag the slider
    drag_slider_to_value(driver, '4000',0 )
    time.sleep(2)
    drag_slider_to_value(driver, '95000', 1)
    time.sleep(2)
    check_item_visibility(driver, "UTP kábel", expected_visibility=False)

    # Find and click the Add button of the Roller product
    add_roller_button = driver.find_element(By.XPATH, "//td[text()='Roller']/following-sibling::td/button")
    add_roller_button.click()
    time.sleep(2)

    # Find and click the Cart button
    basket_button = driver.find_element(By.XPATH, "//a[contains(text(), 'Kosár')]")
    basket_button.click()
    time.sleep(2)
    items_to_check = {
        'Sajt': '2',
        'Alaplap': '1',
        'Roller': '1'
    }
    check_items_and_quantities(driver, items_to_check)

    # Find and click the Remove button
    remove_button = driver.find_element(By.XPATH, "//td[text()='Alaplap']/following-sibling::td/button")
    remove_button.click()
    items_to_check = {
        'Sajt': '2',
        'Roller': '1'
    }
    check_items_and_quantities(driver, items_to_check)


if __name__ == "__main__":
    test_cart(webdriver.Chrome)
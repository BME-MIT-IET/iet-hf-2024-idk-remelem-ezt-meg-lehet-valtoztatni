import subprocess
import time
import os
from selenium import webdriver
import signal

import adminlogin
import registration
import userlogin
import ProductsToCart
import order
import orderstatus
from selenium.webdriver.chrome.options import Options
import argparse

# Paths to the application
root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
backend_path = os.path.join(root_dir, 'Backend', 'WebShop.Api')
frontend_path = os.path.join(root_dir, 'Frontend')
db_path = os.path.join(backend_path, 'WebShop.Api.csproj')
dotnet_ef_path = os.path.join(os.environ['USERPROFILE'], '.dotnet', 'tools', 'dotnet-ef')  # Change if dotnet-ef can be found elsewhere

class BackendManager:
    def __init__(self):
        self.backend = None

    def start_backend(self):
        self.backend = subprocess.Popen(
            ["dotnet", "run", "--project", backend_path],
            creationflags=subprocess.CREATE_NEW_PROCESS_GROUP
        )

    def kill_backend(self):
        if self.backend and self.backend.poll() is None:  # Check if the process is still running
            self.backend.terminate()  # Terminate the process
            try:
                self.backend.wait(timeout=5)  # Wait for the process to close
            except subprocess.TimeoutExpired:
                self.backend.kill()

    def reset_database(self):
        self.kill_backend()  # Stops Backend in order to be able to run migrations on the database
        subprocess.run([dotnet_ef_path, "database", "drop", "--force", "--project", db_path], check=True)
        print("Database dropped successfully.")
        subprocess.run([dotnet_ef_path, "database", "update", "--project", db_path], check=True)
        print("Database updated successfully.")
        self.start_backend()  # Starts Backend after migrations are done

# Used for terminating the Frontend
def kill_frontend(frontend):
    frontend.send_signal(signal.CTRL_BREAK_EVENT)
    frontend.wait()

# Used for starting the Frontend
def start_frontend():
    npm_path = r'C:\Program Files\nodejs\npm.cmd'  # Change if npm can be found elsewhere
    return subprocess.Popen([npm_path, "start", "--prefix", frontend_path],
                            creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)

# Creates a Chrome webdriver
def create_driver():
    chrome_options = Options()
    chrome_options.add_argument("--incognito")  # Incognito mode won't save cookies between tests so they can start on a clean site
    return webdriver.Chrome(options=chrome_options)

# Runs tests
def run_tests(test_number, backend_manager):
    driver = create_driver()
    driver.get("http://localhost:3000")  # Connect the webdriver to the Frontend
    try:
        if test_number == 1:
            registration.test_registration(driver)
            backend_manager.reset_database()
        elif test_number == 2:
            userlogin.test_userlogin(driver)
            backend_manager.reset_database()
        elif test_number == 3:
            ProductsToCart.test_cart(driver)
            backend_manager.reset_database()
        elif test_number == 4:
            order.test_order(driver)
            backend_manager.reset_database()
        elif test_number == 5:
            adminlogin.test_adminlogin(driver)
            backend_manager.reset_database()
        elif test_number == 6:
            orderstatus.test_orderstatus(driver)
            backend_manager.reset_database()
        elif test_number == 7:
            registration.test_registration(driver)
            backend_manager.reset_database()
            driver = create_driver()
            driver.get("http://localhost:3000")
            userlogin.test_userlogin(driver)
            backend_manager.reset_database()
            driver = create_driver()
            driver.get("http://localhost:3000")
            ProductsToCart.test_cart(driver)
            backend_manager.reset_database()
            driver = create_driver()
            driver.get("http://localhost:3000")
            order.test_order(driver)
            backend_manager.reset_database()
            driver = create_driver()
            driver.get("http://localhost:3000")
            adminlogin.test_adminlogin(driver)
            backend_manager.reset_database()
            driver = create_driver()
            driver.get("http://localhost:3000")
            orderstatus.test_orderstatus(driver)
            backend_manager.reset_database()
        else:
            print(f"Invalid test number: {test_number}. Please provide a number between 1 and 7.")
    finally:
        driver.quit()

def main():
    parser = argparse.ArgumentParser(description="Run specified test by number.")
    parser.add_argument('test_number', type=int, help='The number of the test to run (1-7, 7 runs each test).')
    args = parser.parse_args()

    backend_manager = BackendManager()
    backend_manager.start_backend()
    frontend = start_frontend()
    time.sleep(5)
    try:
        run_tests(args.test_number, backend_manager)
    finally:
        backend_manager.kill_backend()
        kill_frontend(frontend)
        print("Cleanup completed.")

if __name__ == "__main__":
    main()
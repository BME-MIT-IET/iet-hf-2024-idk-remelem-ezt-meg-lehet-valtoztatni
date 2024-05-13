import subprocess
import time
import os
from selenium import webdriver
import signal
import registration
import userlogin
import ProductsToCart
import order
import orderstatus
from selenium.webdriver.chrome.options import Options

#paths to the application
root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
backend_path = os.path.join(root_dir, 'Backend', 'WebShop.Api')
frontend_path = os.path.join(root_dir, 'Frontend')
db_path = os.path.join(backend_path, 'WebShop.Api.csproj')
dotnet_ef_path = os.path.join(os.environ['USERPROFILE'], '.dotnet', 'tools', 'dotnet-ef') #Change if dotnet-ef can be found elsewhere

backend = None  # Global variable for the backend process

#Used for terminating the Backend
def kill_process(process):
    if process and process.poll() is None:  # Check if the process is still running
        process.terminate()  # Terminate the process
        try:
            process.wait(timeout=5)  # Wait for the process to close
        except subprocess.TimeoutExpired:
            process.kill()

#Used for terminating the Frontend
def kill_frontend(frontend):
    frontend.send_signal(signal.CTRL_BREAK_EVENT)
    frontend.wait()

#Used for starting the Backend
def start_backend():
    global backend
    backend = subprocess.Popen(
        ["dotnet", "run", "--project", backend_path],
        creationflags=subprocess.CREATE_NEW_PROCESS_GROUP
    )

#Used for starting the Frontend
def start_frontend():
    npm_path = r'C:\Program Files\nodejs\npm.cmd' #Change if npm can be found elsewhere
    return subprocess.Popen([npm_path, "start", "--prefix", frontend_path],
                            creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)

#Resets the database of the Backend
def reset_database():
    global backend
    kill_process(backend) #Stops Backend in order to be able to run migrations on the database
    subprocess.run([dotnet_ef_path, "database", "drop", "--force", "--project", db_path], check=True)
    print("Database dropped successfully.")
    subprocess.run([dotnet_ef_path, "database", "update", "--project", db_path], check=True)
    print("Database updated successfully.")
    start_backend() #Starts Backend after migrations are done


#Creates a Chrome webdriver
def create_driver():
    chrome_options = Options()
    chrome_options.add_argument("--incognito") #Incognito mode won't save cookies between tests so they can start on a clean site
    return webdriver.Chrome(options=chrome_options)


#Runs tests
def run_tests():
    global backend
    driver = create_driver()
    driver.get("http://localhost:3000") #Connect the webdriver to the Frontend
    try:
        registration.test_registration(driver)
        reset_database()
        driver = create_driver()
        driver.get("http://localhost:3000")
        orderstatus.test_orderstatus(driver)
        reset_database()                            #Change tests here, if you want to check a single one. You need to keep the test row and a reset_database row.
        #userlogin.test_userlogin(driver)           #If multiple tests run after eachother, you also need to refresh the driver to ensure it didn't save anything from the previous test
        #reset_database()
        #driver = create_driver()
        #driver.get("http://localhost:3000")
        #ProductsToCart.test_cart(driver)
        #reset_database()
        #order.test_order(driver)
        #reset_database()

    finally:
        driver.quit()

def main():
    global backend
    start_backend()
    frontend = start_frontend()
    time.sleep(5)
    try:
        run_tests()
    finally:
        kill_process(backend)
        kill_frontend(frontend)
        print("Cleanup completed.")

if __name__ == "__main__":
    main()
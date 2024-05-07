import subprocess
import time
import os
from selenium import webdriver
import signal
import registration
import userlogin
from selenium.webdriver.chrome.options import Options

root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
backend_path = os.path.join(root_dir, 'Backend', 'WebShop.Api')
frontend_path = os.path.join(root_dir, 'Frontend')
db_path = os.path.join(backend_path, 'WebShop.Api.csproj')
dotnet_ef_path = os.path.join(os.environ['USERPROFILE'], '.dotnet', 'tools', 'dotnet-ef')

backend = None  # Global variable for the backend process

def kill_process(process):
    if process and process.poll() is None:  # Check if the process is still running
        process.terminate()  # Terminate the process
        try:
            process.wait(timeout=5)  # Wait for the process to close
        except subprocess.TimeoutExpired:
            process.kill()

def kill_frontend(frontend):
    frontend.send_signal(signal.CTRL_BREAK_EVENT)
    frontend.wait()

def start_backend():
    global backend
    backend = subprocess.Popen(
        ["dotnet", "run", "--project", backend_path],
        creationflags=subprocess.CREATE_NEW_PROCESS_GROUP
    )

def start_frontend():
    npm_path = r'C:\Program Files\nodejs\npm.cmd'
    return subprocess.Popen([npm_path, "start", "--prefix", frontend_path],
                            creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)

def reset_database():
    global backend
    kill_process(backend)
    subprocess.run([dotnet_ef_path, "database", "drop", "--force", "--project", db_path], check=True)
    print("Database dropped successfully.")
    subprocess.run([dotnet_ef_path, "database", "update", "--project", db_path], check=True)
    print("Database updated successfully.")
    start_backend()


def create_driver():
    chrome_options = Options()
    chrome_options.add_argument("--incognito")
    return webdriver.Chrome(options=chrome_options)

def run_tests():
    global backend
    driver = create_driver()
    driver.get("http://localhost:3000")
    try:
        registration.test_registration(driver)
        reset_database()
        driver = create_driver()
        driver.get("http://localhost:3000")
        userlogin.test_userlogin(driver)
        reset_database()
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
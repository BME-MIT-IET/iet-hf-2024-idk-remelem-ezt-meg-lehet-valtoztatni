import subprocess
import time
import os
from selenium import webdriver
import signal


root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

backend_path = os.path.join(root_dir, 'Backend', 'WebShop.Api')

frontend_path = os.path.join(root_dir, 'Frontend')

def kill_frontend(frontend):
    frontend.send_signal(signal.CTRL_BREAK_EVENT)  # Send CTRL_BREAK_EVENT to effectively kill Node processes on Windows
    frontend.wait()  # Wait for the process to terminate
def start_backend():
    # Assuming backend can be started with a simple command
    return subprocess.Popen(["dotnet", "run", "--project", backend_path], creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)

def start_frontend():
    # Start the frontend using npm
    npm_path = r'C:\Program Files\nodejs\npm.cmd'
    return subprocess.Popen([npm_path, "start", "--prefix", frontend_path], creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)

def run_tests():
    # Example: Launch and run a Selenium test
    driver = webdriver.Chrome()
    driver.get("http://localhost:3000")  # Adjust port if different
    # Add your Selenium test logic here
    driver.quit()

def main():
    backend = start_backend()
    frontend = start_frontend()
    time.sleep(10)  # Wait for servers to start
    try:
        run_tests()
    finally:
        backend.terminate()
        backend.wait()
        kill_frontend(frontend)
        print("Cleanup completed.")

if __name__ == "__main__":
    main()
import subprocess
import time
import os
from selenium import webdriver
import signal
import registration

root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

backend_path = os.path.join(root_dir, 'Backend', 'WebShop.Api')

frontend_path = os.path.join(root_dir, 'Frontend')

db_path = os.path.join(backend_path, 'WebShop.Api.csproj')

dotnet_ef_path = os.path.join(os.environ['USERPROFILE'], '.dotnet', 'tools', 'dotnet-ef')


def kill_frontend(frontend):
    frontend.send_signal(signal.CTRL_BREAK_EVENT)  # Send CTRL_BREAK_EVENT to effectively kill Node processes on Windows
    frontend.wait()  # Wait for the process to terminate

def start_backend():
    # Assuming backend can be started with a simple command
    return subprocess.Popen(
        ["dotnet", "run", "--project", backend_path],
        creationflags=subprocess.CREATE_NEW_PROCESS_GROUP
    )


def start_frontend():
    # Start the frontend using npm
    npm_path = r'C:\Program Files\nodejs\npm.cmd'
    return subprocess.Popen([npm_path, "start", "--prefix", frontend_path],
                            creationflags=subprocess.CREATE_NEW_PROCESS_GROUP)


def reset_database(backend):
    backend.terminate()
    backend.wait()
    try:
        # Command to drop the database
        subprocess.run([dotnet_ef_path, "database", "drop", "--force", "--project", db_path], check=True)
        print("Database dropped successfully.")

        # Command to reapply migrations
        subprocess.run([dotnet_ef_path, "database", "update", "--project", db_path], check=True)
        print("Database updated successfully.")

    except subprocess.CalledProcessError as e:
        print(f"An error occurred while resetting the database: {e}")
        raise

    return start_backend()


def run_tests(backend):
    # Example: Launch and run a Selenium test
    driver = webdriver.Chrome()
    driver.get("http://localhost:3000")  # Adjust port if different
    try:
        registration.test_registration(driver)
        backend = reset_database(backend)
    finally:
        driver.quit()
    backend = reset_database(backend)
    return backend

def main():
    backend = start_backend()
    frontend = start_frontend()
    time.sleep(5)  # Wait for servers to start
    try:
        backend = run_tests(backend)
    finally:
        backend.terminate()
        backend.wait()
        kill_frontend(frontend)
        print("Cleanup completed.")


if __name__ == "__main__":
    main()

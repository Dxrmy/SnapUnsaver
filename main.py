import pygetwindow as gw

import pyautogui
import cv2
import time
import random
import keyboard
import os
import io
import zipfile
import urllib.request
import subprocess
import shutil

# --- Configuration ---
SAVED_INDICATOR_IMG = 'saved_message_indicator.png'
UNSAVE_BUTTON_IMG = 'unsave_button.png'
CONFIDENCE_LEVEL = 0.8
GRAYSCALE_MATCHING = True
SCRCPY_VERSION = "v3.3.4"
SCRCPY_URL = f"https://github.com/Genymobile/scrcpy/releases/download/{SCRCPY_VERSION}/scrcpy-win64-{SCRCPY_VERSION}.zip"
SCRCPY_DIR = "scrcpy"

# Fail-safe
pyautogui.FAILSAFE = True

def random_sleep(min_seconds=0.8, max_seconds=2.2):
    duration = random.uniform(min_seconds, max_seconds)
    time.sleep(duration)

def setup_scrcpy():
    """Downloads and sets up scrcpy if not present."""
    if os.path.exists(os.path.join(SCRCPY_DIR, "scrcpy.exe")):
        print("scrcpy is already installed.")
        return

    print(f"scrcpy not found. Downloading from {SCRCPY_URL}...")
    try:
        with urllib.request.urlopen(SCRCPY_URL) as response:
            with zipfile.ZipFile(io.BytesIO(response.read())) as z:
                root_folder = z.namelist()[0].split('/')[0]
                z.extractall(".")
                if os.path.exists(SCRCPY_DIR):
                     shutil.rmtree(SCRCPY_DIR)
                os.rename(root_folder, SCRCPY_DIR)
        print("scrcpy downloaded and extracted successfully.")
    except Exception as e:
        print(f"Failed to download scrcpy: {e}")
        print("Please download it manually and extract it to a 'scrcpy' folder.")

def start_scrcpy():
    """Starts scrcpy in a subprocess."""
    scrcpy_exe = os.path.join(SCRCPY_DIR, "scrcpy.exe")
    if os.path.exists(scrcpy_exe):
        print("Starting scrcpy...")
        subprocess.Popen(["scrcpy.exe", "--max-fps=15"], cwd=SCRCPY_DIR, shell=True)
        time.sleep(3) 
    else:
        print("Could not find scrcpy.exe to launch.")

def get_target_window_region():
    """Asks the user to focus the target window, then grabs its region."""
    # Wait for the user to be ready
    print("\nXXX IMPORTANT XXX")
    print("Please CLICK on the scrcpy window to focus it.")
    input("Then press ENTER in this terminal immediately... ")
    
    # Give a tiny buffer in case they clicked terminal to press enter
    # But wait, if they press enter in terminal, terminal is active.
    # Better approach: "Hover over the scrcpy window and press Enter" or "After pressing Enter, you have 3 seconds to focus scrcpy".
    
    print("You have 3 seconds to switch back to/focus the scrcpy window!")
    for i in range(3, 0, -1):
        print(f"{i}...", end=" ", flush=True)
        time.sleep(1)
    print("Capturing active window...")
    
    win = gw.getActiveWindow()
    if win:
        print(f"Captured window: '{win.title}' at ({win.left}, {win.top}) size {win.width}x{win.height}")
        return (win.left, win.top, win.width, win.height)
    
    print("Could not detect active window.")
    return None

def scroll_up(region=None):
    """
    Simulates swiping DOWN to scroll UP (view older messages).
    Constraints swipe to the provided region.
    """
    print("Scrolling up (viewing older)...")
    
    if region:
        left, top, width, height = region
        # Stay safe within margins
        center_x = left + width // 2
        # To scroll to OLDER messages (up in history), we drag from TOP to BOTTOM
        start_y = int(top + height * 0.3) 
        end_y = int(top + height * 0.7)
    else:
        # Full screen fallback (NOT RECOMMENDED if region fails, but safe def)
        screen_width, screen_height = pyautogui.size()
        center_x = screen_width // 2
        start_y = int(screen_height * 0.3)
        end_y = int(screen_height * 0.7)

    pyautogui.moveTo(center_x, start_y)
    pyautogui.dragTo(center_x, end_y, duration=random.uniform(0.6, 1.1), button='left')

def check_fail_safe():
    if keyboard.is_pressed('q'):
        print("Fail safe 'Q' pressed. Exiting.")
        return True
    return False

def main():
    print("Welcome to SnapUnsaver.")
    
    setup_scrcpy()
    
    print("Ensure your phone is connected via USB and debugging is on.")
    start_scrcpy()

    input("\nResize/Position the scrcpy window if needed.\nThen press ENTER to start setup... ")
    
    # Locate window
    scrcpy_region = get_target_window_region()
    
    if not scrcpy_region:
        print("WARNING: Could not find target window.")
        print("Will scan FULL SCREEN (higher risk of false positives).")
    else:
        print(f"Targeting window region: {scrcpy_region}")
        # Validate region size to avoid (0,0) issues
        if scrcpy_region[2] < 100 or scrcpy_region[3] < 100:
             print("WARNING: Detected window seems very small or invalid. Check if it was minimized.")
             # Fallback?
             # scrcpy_region = None 
        else:
             # Ensure focus one last time
             try:
                 center = pyautogui.center(scrcpy_region)
                 pyautogui.click(center)
             except: pass
    else:
        print(f"Targeting scrcpy window at: {scrcpy_region}")
        # Focus it
        try:
             # simple click on title bar or center to focus?
             # Safe click in center
             safe_center = pyautogui.center(scrcpy_region)
             pyautogui.click(safe_center)
        except: pass

    if not os.path.exists(SAVED_INDICATOR_IMG) or not os.path.exists(UNSAVE_BUTTON_IMG):
        print("Error: Reference images not found!")
        return

    while True:
        if check_fail_safe(): break

        try:
            print("Scanning for 'Saved' message...")
            # Use restricted region
            location = pyautogui.locateOnScreen(
                SAVED_INDICATOR_IMG, 
                confidence=CONFIDENCE_LEVEL, 
                grayscale=GRAYSCALE_MATCHING,
                region=scrcpy_region
            )

            if location:
                print(f"Found saved message at: {location}")
                center_point = pyautogui.center(location)
                
                pyautogui.moveTo(center_point)
                print("Long pressing...")
                pyautogui.mouseDown()
                time.sleep(random.uniform(0.8, 1.2)) 
                pyautogui.mouseUp()
                
                random_sleep(0.5, 0.8)
                if check_fail_safe(): break

                print("Looking for 'Unsave in Chat'...")
                # Search primarily in region, but the context menu might pop slightly outside IF edge?
                # Usually context menu is near key. Region is safer.
                unsave_btn = pyautogui.locateOnScreen(
                    UNSAVE_BUTTON_IMG,
                    confidence=CONFIDENCE_LEVEL,
                    grayscale=GRAYSCALE_MATCHING,
                    region=scrcpy_region
                )
                
                if unsave_btn:
                    print("Found Unsave button.")
                    print("Waiting 1 second (SAFETY) - Press 'Q' to abort...")
                    time.sleep(1.0) 
                    if check_fail_safe(): break
                    
                    print("Clicking Unsave...")
                    pyautogui.click(pyautogui.center(unsave_btn))
                    print("Unsaved!")
                    random_sleep()
                else:
                    print("Unsave button NOT found in region.")
                    print("Pressing ESC (Back) to close menu...")
                    pyautogui.press('esc') 
                    random_sleep(0.5, 1.0)
                    
                    scroll_up(scrcpy_region)
                    random_sleep()
                
            else:
                print("No saved message found in region. Scrolling...")
                scroll_up(scrcpy_region)
                random_sleep()

        except Exception as e:
             print(f"Loop error: {e}")
             # If error (e.g. ImageNotFound), we scroll
             scroll_up(scrcpy_region)
             random_sleep()

if __name__ == "__main__":
    main()

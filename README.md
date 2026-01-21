# SnapUnsaver

Automated Snapchat message unsaving tool using Python, PyAutoGUI, and OpenCV.

## Features
- **Auto-Scrcpy**: Automatically downloads and sets up `scrcpy`.
- **Smart Targeting**: Asks you to focus the scrcpy window to lock onto it.
- **Human-like Behavior**: Random delays, scroll overshooting, and safety waits.
- **Fail Safe**: Press `Q` or slam mouse to corner to stop.

## Setup

1.  **Install Python**: Ensure you have Python installed.
2.  **Install Dependencies**:
    ```bash
    pip install -r requirements.txt
    ```
3.  **Enable USB Debugging** on your Android phone and connect it to your PC.

## Reference Images

Ensure `saved_message_indicator.png` and `unsave_button.png` are in this folder.

## Usage

1.  Run the script:
    ```bash
    python main.py
    ```
2.  If `scrcpy` is missing, it will download it.
3.  The script will start `scrcpy`.
4.  **Important**: The script will ask you to **CLICK** the scrcpy window to focus it, then press ENTER in the terminal. This ensures it finds the right window.
5.  Automation starts.

## Controls

*   **Quit**: Press `Q`.
*   **Fail Safe**: Slam mouse to top-left corner.

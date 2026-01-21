<div align="center">
  <img src="icon.png" alt="SnapUnsaver Icon" width="128" />
  <h1>SnapUnsaver</h1>
  <p>
    <strong>Universal Personal Data Redaction & Automation</strong>
  </p>
  
  ![C#](https://img.shields.io/badge/C%23-12-239120?style=flat&logo=c-sharp&logoColor=white)
  ![Privacy](https://img.shields.io/badge/Focus-Redaction-red)
  ![Status](https://img.shields.io/badge/Status-Active-success)
  ![License](https://img.shields.io/badge/License-MIT-green)

  <br />
</div>

**SnapUnsaver** is a high-speed automation utility designed for mass data management and personal privacy. While it currently specializes in unsaving Snapchat media via ADB, it is evolving into a universal redaction suite for all major platforms.

## Features
- **Snapchat Automation:** Specialized ADB-driven engine for rapid unsaving and archiving of chat media.
- **Computer Vision:** Leveraging OpenCV for real-time button detection and state verification across varying app versions.
- **Privacy Core:** Local execution ensured—your data never touches our servers during the redaction process.
- **ADB Integration:** Low-latency control system for non-intrusive mobile interaction.
- **Universal Expansion:** (WIP) Modular detection engine designed to easily map new platform UIs (Reddit, Discord).

## 🚧 Roadmap & Todo
State of the project as of latest push:

- [x] **Core Automation**: ADB command bridge and screen-capture pipeline.
- [x] **Snapchat Module**: Reliable scraping and unsaving logic with state recovery.
- [x] **Visual Feedback**: Integration with `scrcpy` for real-time monitoring.
- [/] **Platform Expansion**:
    - [ ] Reddit Redaction (Mass post/comment deletion).
    - [ ] Discord Purge (Mass private message removal).
- [ ] **Universal UI Manager**: Dynamic OpenCV template matching for any app interface.

## 🛠 Tech Stack
- **Languages**: C# (Current Native Core), Python (Legacy Logic)
- **Computer Vision**: OpenCvSharp4
- **Mobile Interaction**: Android Debug Bridge (ADB), scrcpy

## 📊 Analytics
<div align="center">
  <a href="https://github.com/Dxrmy/SnapUnsaver">
  <img height="130" align="center" src="https://github-readme-stats.vercel.app/api/pin/?username=Dxrmy&repo=SnapUnsaver&theme=transparent&border_color=30363d&show_owner=true"/>
  </a>
</div>

## 📄 License
Distributed under the MIT License. See `LICENSE` for more information.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.IO.Compression;
using System.Net.Http;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace SnapUnsaverWin
{
    public partial class Form1 : Form
    {
        private const string ScrcpyVersion = "v3.3.4";
        private const string ScrcpyUrl = $"https://github.com/Genymobile/scrcpy/releases/download/{ScrcpyVersion}/scrcpy-win64-{ScrcpyVersion}.zip";
        
        private Process? scrcpyProcess;
        private bool isRunning = false;
        private Random rnd = new Random();
        private string toolsPath;

        public Form1()
        {
            InitializeComponent();
            toolsPath = Path.Combine(Application.StartupPath, "tools");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await SetupScrcpyAsync();
            await StartScrcpyAsync();
            LoadPreviews();
        }

        private async System.Threading.Tasks.Task SetupScrcpyAsync()
        {
            if (File.Exists(Path.Combine(toolsPath, "scrcpy.exe")))
            {
                return;
            }

            Log("scrcpy not found. Downloading dependencies...");
            try
            {
                if (!Directory.Exists(toolsPath)) Directory.CreateDirectory(toolsPath);

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(ScrcpyUrl);
                    response.EnsureSuccessStatusCode();
                    
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var archive = new ZipArchive(stream))
                    {
                        // The zip contains a folder like scrcpy-win64-v3.3.4/
                        // We want to extract its contents directly into toolsPath
                        string? rootFolder = null;
                        foreach (var entry in archive.Entries)
                        {
                            if (rootFolder == null && entry.FullName.Contains("/"))
                            {
                                rootFolder = entry.FullName.Split('/')[0];
                            }

                            if (string.IsNullOrEmpty(entry.Name)) continue; // Skip directories

                            string relativePath = rootFolder != null && entry.FullName.StartsWith(rootFolder) 
                                ? entry.FullName.Substring(rootFolder.Length + 1) 
                                : entry.FullName;

                            string destinationPath = Path.Combine(toolsPath, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                            entry.ExtractToFile(destinationPath, true);
                        }
                    }
                }
                Log("Dependencies installed successfully.");
            }
            catch (Exception ex)
            {
                Log("Failed to download scrcpy: " + ex.Message);
            }
        }

        private void LoadPreviews()
        {
            // Load patterns for preview
            string savedPath = Path.Combine(Application.StartupPath, "saved_bar.png");
            string unsavePath = Path.Combine(Application.StartupPath, "unsave_text.png");

            if (File.Exists(savedPath)) picSavedPreview.Image = Image.FromFile(savedPath);
            if (File.Exists(unsavePath)) picUnsavePreview.Image = Image.FromFile(unsavePath);
        }

        private async System.Threading.Tasks.Task StartScrcpyAsync()
        {
            string scrcpyExe = Path.Combine(toolsPath, "scrcpy.exe");
            if (!File.Exists(scrcpyExe))
            {
                Log("Error: scrcpy.exe not found in " + toolsPath);
                return;
            }

            // Check for ADB devices
            if (!CheckAdbDevices())
            {
                Log("[ADB Warning] No devices detected! Please connect your phone.");
            }

            ProcessStartInfo psi = new ProcessStartInfo(scrcpyExe, "--window-title scrcpy_embed --window-borderless")
            {
                WorkingDirectory = toolsPath,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true // Hide console window
            };

            scrcpyProcess = new Process { StartInfo = psi };
            
            scrcpyProcess.OutputDataReceived += (s, e) => { if (e.Data != null) Log("[scrcpy] " + e.Data); };
            scrcpyProcess.ErrorDataReceived += (s, e) => { if (e.Data != null) Log("[scrcpy error] " + e.Data); };

            if (!scrcpyProcess.Start())
            {
                Log("Error: Failed to start scrcpy process.");
                return;
            }
            
            scrcpyProcess.BeginOutputReadLine();
            scrcpyProcess.BeginErrorReadLine();

            Log("Waiting for scrcpy window...");
            
            IntPtr handle = IntPtr.Zero;
            int retries = 20; // 10 seconds total
            while (retries > 0)
            {
                scrcpyProcess.Refresh();
                handle = scrcpyProcess.MainWindowHandle;
                if (handle != IntPtr.Zero) break;
                
                await System.Threading.Tasks.Task.Delay(500);
                retries--;
            }

            if (handle != IntPtr.Zero)
            {
                Win32.SetParent(handle, phonePanel.Handle);
                Win32.RemoveBorder(handle);
                Win32.MoveWindow(handle, 0, 0, phonePanel.Width, phonePanel.Height, true);
                Log("scrcpy embedded successfully.");
            }
            else
            {
                Log("Error: Could not find scrcpy window handle.");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isRunning) return;
            isRunning = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            backgroundWorker1.RunWorkerAsync();
            Log("Automation started.");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isRunning = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            Log("Stopping automation...");
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string savedBarPath = Path.Combine(Application.StartupPath, "saved_bar.png");
            string unsaveTextPath = Path.Combine(Application.StartupPath, "unsave_text.png");

            while (isRunning)
            {
                try
                {
                    Bitmap screen = CapturePanel();
                    using (Mat screenMat = screen.ToMat())
                    using (Mat savedBar = new Mat(savedBarPath))
                    {
                        var res = screenMat.MatchTemplate(savedBar, TemplateMatchModes.CCoeffNormed);
                        res.MinMaxLoc(out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                        if (maxVal > 0.8)
                        {
                            Log($"Found Message Indicator ({maxVal:P0})");
                            
                            // Long press via ADB
                            // We need to map Panel coords to phone coords. 
                            // For simplicity, we assume scrcpy is fit to panel.
                            // ADB Swipe: x1 y1 x2 y2 duration
                            int phoneX = maxLoc.X + (savedBar.Width / 2);
                            int phoneY = maxLoc.Y + (savedBar.Height / 2);

                            RunAdb($"shell input swipe {phoneX} {phoneY} {phoneX} {phoneY} 600");
                            Thread.Sleep(1000);

                            // Look for unsave button
                            using (Bitmap menuScreen = CapturePanel())
                            using (Mat menuMat = menuScreen.ToMat())
                            using (Mat unsaveText = new Mat(unsaveTextPath))
                            {
                                var resMenu = menuMat.MatchTemplate(unsaveText, TemplateMatchModes.CCoeffNormed);
                                resMenu.MinMaxLoc(out _, out double maxValMenu, out _, out OpenCvSharp.Point maxLocMenu);

                                if (maxValMenu > 0.8)
                                {
                                    int tapX = maxLocMenu.X + (unsaveText.Width / 2);
                                    int tapY = maxLocMenu.Y + (unsaveText.Height / 2);
                                    RunAdb($"shell input tap {tapX} {tapY}");
                                    Log("Unsaved message.");
                                }
                            }
                        }
                        else
                        {
                            // Scroll up
                            Log("No message found, scrolling...");
                            RunAdb("shell input swipe 500 300 500 800 300");
                        }
                    }

                    Thread.Sleep(rnd.Next(1500, 3000));
                }
                catch (Exception ex)
                {
                    Log("Error: " + ex.Message);
                }
            }
        }

        private Bitmap CapturePanel()
        {
            Bitmap bmp = new Bitmap(phonePanel.Width, phonePanel.Height);
            phonePanel.DrawToBitmap(bmp, new Rectangle(0, 0, phonePanel.Width, phonePanel.Height));
            return bmp;
        }

        private void RunAdb(string args)
        {
            string adbExe = Path.Combine(toolsPath, "adb.exe");
            ProcessStartInfo psi = new ProcessStartInfo(adbExe, args)
            {
                WorkingDirectory = toolsPath,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi)?.WaitForExit();
        }

        private void Log(string msg)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(msg)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void KillAllAdbProcesses()
        {
            Log("Cleaning up ADB processes...");
            try
            {
                foreach (var proc in Process.GetProcessesByName("adb"))
                {
                    try { proc.Kill(); } catch { }
                }
                // Also kill any lingering scrcpy
                foreach (var proc in Process.GetProcessesByName("scrcpy"))
                {
                    try { proc.Kill(); } catch { }
                }
            }
            catch (Exception ex)
            {
                Log($"Cleanup Warning: {ex.Message}");
            }
        }

        private bool CheckAdbDevices()
        {
            string adbExe = Path.Combine(toolsPath, "adb.exe");
            if (!File.Exists(adbExe)) return false;

            string RunAdbCommand(string cmd)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(adbExe, cmd)
                    {
                        WorkingDirectory = toolsPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (var proc = Process.Start(psi))
                    {
                        if (proc == null) return "";
                        string outStr = proc.StandardOutput.ReadToEnd();
                        proc.WaitForExit();
                        return outStr;
                    }
                }
                catch (Exception ex) 
                {
                    Log($"ADB Error ({cmd}): {ex.Message}");
                    return ""; 
                }
            }

            // Step 1: Nuclear Option - Kill everything first
            KillAllAdbProcesses();
            Thread.Sleep(500);

            // Step 2: Start Server explicitly
            Log("[ADB] Starting ADB server...");
            RunAdbCommand("start-server");
            
            // Give the daemon time to spin up and detect devices
            Log("[ADB] Waiting for device detection...");
            Thread.Sleep(2000); // 2 second delay for detection

            // Step 3: Check Devices
            string output = RunAdbCommand("devices -l");
            Log("[ADB Raw Output]:\r\n" + output.Trim());

            if (!output.Contains(" device "))
            {
                Log("[ADB] Retry: Killing server and trying again slowly...");
                RunAdbCommand("kill-server");
                Thread.Sleep(1000);
                RunAdbCommand("start-server");
                Thread.Sleep(3000); // Longer wait
                
                output = RunAdbCommand("devices -l");
                Log("[ADB Retry Output]:\r\n" + output.Trim());
            }

            if (output.Contains(" device ")) return true;
            
            if (output.Contains("unauthorized"))
            {
                Log("[ADB Warning] Device found but UNAUTHORIZED. Check your phone screen for a prompt!");
            }
            else if (output.Contains("offline"))
            {
                Log("[ADB Warning] Device found but OFFLINE. Reconnect cable/USB debugging.");
            }
            
            return false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
            try { scrcpyProcess?.Kill(); } catch { }
        }
    }
}

using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace In_Place_Mover
{
    public partial class Form1 : Form
    {
        string error = "\n";
        bool advancedOpen = false;
        bool isFolderSelected = false;
        bool fallback = true;
        bool overwrite = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void advanced_Click(object sender, EventArgs e)
        {
            if (!advancedOpen)
            {
                this.Height = 250;
                advancedOpen = true;
            }
            else
            {
                this.Height = 140;
                advancedOpen = false;
            }
        }

        private void selFileSource_Click(object sender, EventArgs e)
        {
            doFileSelect(false,false);
        }

        private void selFolderSource_Click(object sender, EventArgs e)
        {
            doFileSelect(true,false);
        }

        private void selFolderDest_Click(object sender, EventArgs e)
        {
            doFileSelect(true,true);
        }
        private void doFileSelect(bool folder, bool destOnly)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog();
            StringBuilder title = new StringBuilder("Select ");
            if (folder) title.Append("Folder ");
            else title.Append("File ");
            if (destOnly) title.Append("Source");
            else title.Append("Destination");
            fileDialog.InitialDirectory = @"C:\";
            fileDialog.EnsureFileExists = true;
            fileDialog.IsFolderPicker = folder;
            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = fileDialog.FileName;
                if (!destOnly) fileSource.Text = path;
                fileDest.Text = Regex.Replace(path, @"C:\\", "D:\\");
                isFolderSelected = folder;
            }
        }


        private void startMove_Click(object sender, EventArgs e)
        {
            doMoveOperation(fileSource.Text, fileDest.Text);
        }

        private void doMoveOperation(string source, string dest)
        {
            moveStatus.Text = "In Progress...";
            moveStatusBar.Minimum = 0;
            moveStatusBar.Step = 1;

            if (source.Length > 3 && dest.Length > 3)
            {
                if (isFolderSelected)
                {
                    moveStatus.Text = "Enumerating Files...";
                    moveStatusBar.Style = ProgressBarStyle.Marquee;
                    int files = enumerateFiles(source);
                    if (files == -1)
                    {
                        return;
                    }
                    moveStatusBar.Maximum = files + 4;
                    moveStatus.Text = "Enumerated files.";
                    moveStatusBar.Style = ProgressBarStyle.Continuous;
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Copying files...";
                    if (!copyDirectory(source, dest)) return;
                    moveStatus.Text = "Finished Copy.";
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Deleting source...";
                    Directory.Delete(source, true);
                    moveStatus.Text = "Deleted source.";
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Linking directory";
                    if (attemptJunct.Checked || attemptHard.Checked)
                    {
                        Process makeLink = Process.Start("CMD.exe", "/c mklink /J \"" + source + "\" \"" + dest + "\"");
                        makeLink.WaitForExit(1000);
                        if (!makeLink.HasExited)
                        {
                            error += "\n" +  "ERROR: Make link never exited. Operation Aborted.";
                            return;
                        }
                        if (makeLink.ExitCode != 0)
                        {
                            if (fallback)
                            {
                                error += "\n" +  "WARN: Failed to create a junction with code " + makeLink.ExitCode + ", Falling back to symlink";;
                                makeLink = Process.Start("CMD.exe", "/c mklink /D \"" + source + "\" \"" + dest + "\"");
                                Console.WriteLine("/c mklink /D \"" + source + "\" \"" + dest + "\"");
                                makeLink.WaitForExit(1000);
                                if (!makeLink.HasExited)
                                {
                                    error += "\n" +  "ERROR: Make link never exited. Operation Aborted.";;
                                    return;
                                }
                                if (makeLink.ExitCode != 0)
                                {
                                    error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". Operation aborted.";;
                                    return;
                                }
                            }
                            else
                            {
                                error += "\n" +  "ERROR: Failed to create a junction with code " + makeLink.ExitCode + ". Operation aborted.";;
                                return;
                            }
                        }
                    }
                    else
                    {
                        Process makeLink = Process.Start("CMD.exe", "/c mklink /D \"" + source + "\" \"" + dest + "\"");
                        makeLink.WaitForExit(1000);
                        if (!makeLink.HasExited)
                        {
                            error += "\n" +  "ERROR: Make link never exited. Operation Aborted.";;
                            return;
                        }
                        if (makeLink.ExitCode != 0)
                        {
                            error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". Operation aborted.";;
                            return;
                        }
                    }
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Done.";
                }
                else
                {

                    moveStatusBar.Maximum = 5;
                    moveStatus.Text = "Checking destination.";
                    if (!Directory.Exists(dest.Substring(0, dest.LastIndexOf('\\'))))
                    {
                        Directory.CreateDirectory(dest.Substring(0, dest.LastIndexOf('\\')));
                    }
                    moveStatusBar.PerformStep();
                    if (File.Exists(dest) && !overwrite)
                    {
                        error += "\n" +  "ERROR: File " + dest + " already exists. Operation aborted.";;
                        return;
                    }
                    moveStatus.Text = "Destination Checked.";
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Copying file.";
                    File.Copy(source, dest, true);
                    moveStatus.Text = "File Copied.";
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Deleting Original File.";
                    File.Delete(source);
                    moveStatus.Text = "Original File Deleted.";
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Linking file.";
                    if (attemptJunct.Checked || attemptHard.Checked)
                    {
                        Process makeLink = Process.Start("CMD.exe", "/c mklink /H \"" + source + "\" \"" + dest + "\"");
                        makeLink.WaitForExit(1000);
                        if (!makeLink.HasExited)
                        {
                            error += "\n" +  "ERROR: Make link never exited. Operation Aborted.";;
                            return;
                        }
                        if (makeLink.ExitCode != 0)
                        {
                            if (fallback)
                            {
                                error += "\n" +  "WARN: Failed to create a Hardlink with code " + makeLink.ExitCode + ", Falling back to symlink";;
                                makeLink = Process.Start("CMD.exe", "/c mklink \"" + source + "\" \"" + dest + "\"");
                                Console.WriteLine("/c mklink /D \"" + source + "\" \"" + dest + "\"");
                                makeLink.WaitForExit(1000);
                                if (!makeLink.HasExited)
                                {
                                    error += "\n" +  "ERROR: Make link never exited. Operation Aborted.";;
                                    return;
                                }
                                if (makeLink.ExitCode != 0)
                                {
                                    error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". Operation aborted.";;
                                    return;
                                }
                            }
                            else
                            {
                                error += "\n" +  "ERROR: Failed to create a Hardlink with code " + makeLink.ExitCode + ". Operation aborted.";;
                                return;
                            }
                        }
                    }
                    else
                    {
                        Process makeLink = Process.Start("CMD.exe", "/c mklink \"" + source + "\" \"" + dest + "\"");
                        makeLink.WaitForExit(1000);
                        if (!makeLink.HasExited)
                        {
                            error += "\n" +  "ERROR: Make link never exited. Operation Aborted.";;
                            return;
                        }
                        if (makeLink.ExitCode != 0)
                        {
                            error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". Operation aborted.";;
                            return;
                        }
                    }
                    moveStatusBar.PerformStep();
                    moveStatus.Text = "Done.";
                }
            }
            else
            {
                error += "\n" +  "ERROR: Both Source and Destination need to be a proper file or directory. Operation aborted";;
                Console.WriteLine(source);
                Console.WriteLine(dest);
                return;
            }
        }

        private void loadBatchFile_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog();
            fileDialog.InitialDirectory = @"C:\";
            fileDialog.EnsureFileExists = true;
            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = fileDialog.FileName;
                string contents = File.ReadAllText(path, Encoding.UTF8);
                batchFilesList.Text = contents;
            }
        }
        public bool copyDirectory(string source, string dest)
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            if (!dir.Exists)
            {
                error += "\n" +  "ERROR: Folder "+source+" Didnt exist. Operation aborted.";;
                return false;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string path = Path.Combine(dest, file.Name);
                if(File.Exists(path) && !overwrite)
                {
                    error += "\n" +  "ERROR: File " + path + " Already existed. Operation aborted.";;
                }
                else
                {
                    file.CopyTo(path,true);
                }
                moveStatusBar.PerformStep();
                moveStatus.Text = "Copied "+file.Name;
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string path = Path.Combine(dest, subdir.Name);
                if(!copyDirectory(subdir.FullName, path)) return false;
            }
            return true;
        }
        public int enumerateFiles(string source)
        {
            int filesNum = 0;
            DirectoryInfo dir = new DirectoryInfo(source);
            if (!dir.Exists)
            {
                error += "\n" +  "ERROR: Folder " + source + " Didnt exist. Operation aborted.";;
                return -1;
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                filesNum++;
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                filesNum += enumerateFiles(subdir.FullName);
            }
            return filesNum;
        }

        private void fallbackCheck_CheckedChanged(object sender, EventArgs e)
        {
            fallback = fallbackCheck.Checked;
        }

        private void overwriteBtn_CheckedChanged(object sender, EventArgs e)
        {
            overwrite = overwriteBtn.Checked;
        }

        private void startBatchMove_Click(object sender, EventArgs e)
        {
            List<List<string>> lines = new List<List<string>>();
            foreach(var i in batchFilesList.Text.Split('\n'))
            {
                string[] srcdstcombo = i.Split('>');
                if(srcdstcombo.Length != 2)
                {
                    int badLine = Array.IndexOf(batchFilesList.Text.Split('\n'),i);
                    error += "\n" +  "ERROR: Failed to parse batch list: Too little or many arguments on line "+ badLine + ". Operation aborted.";;
                    return;
                }
                int insertTo = lines.Count;
                List<string> temp = new List<string>();
                if(!File.Exists(srcdstcombo[0]) && !Directory.Exists(srcdstcombo[0]))
                {
                    int badLine = Array.IndexOf(batchFilesList.Text.Split('\n'), i);
                    error += "\n" +  "ERROR: File/Folder " + srcdstcombo[0] + " Didnt exist on line "+badLine+" . Operation aborted.";;
                    return;
                }
                temp.Add(srcdstcombo[0]);
                temp.Add(srcdstcombo[1]);
                lines.Add(temp);
            }
            for(var i = 0;i < lines.Count;i++)
            {
                if (!File.Exists(lines[i][0]))
                {
                    if(!Directory.Exists(lines[i][0])) {
                        error += "\n" + "ERROR: File/Folder " + lines[i][0] + " Didnt exist between parsing and doing. Operation aborted.";;
                        return;
                    }
                    else
                    {
                        isFolderSelected = true;
                        doMoveOperation(lines[i][0], lines[i][1]);
                    }
                }
                else
                {
                    isFolderSelected = false;
                    doMoveOperation(lines[i][0], lines[i][1]);
                }
            }
            if (error.Length > 1)
            {
                string showText = "Operation finished with the following warnings:" + error;
                MessageBox.Show(showText);
            }
        }
    }
}

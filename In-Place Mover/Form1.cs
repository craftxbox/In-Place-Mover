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
using System.Threading;
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
        bool skipExisting = false;
        bool isDoThreadActive = false;
        Thread doThread;
        Thread watchThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             watchThread = new Thread(() => {
                 while (true)
                 {
                     if (isDoThreadActive)
                     {
                         if (!doThread.IsAlive)
                         {
                             Invoke(new Action(() => { isDoThreadActive = false; }));
                             error += "\n" + "ERROR: Worker thread died. Operation aborted.";
                             MessageBox.Show("Operation failed with errors: \n" + error,"Operation Aborted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                             foreach (Control i in this.Controls)
                             {
                                 Invoke(new Action(() => { i.Enabled = true; }));
                             }
                         }
                     }
                 }
             });
            watchThread.Start();
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
            this.moveStatus.Text = "In Progress...";
            doThread = new Thread(()=>{doMoveOperation(fileSource.Text, fileDest.Text);});
            doThread.Start();
            foreach(Control i in this.Controls)
            {
                if (i.Name.StartsWith("moveStatus")) continue;
                i.Enabled = false;
            }
        }

        private void doMoveOperation(string source, string dest)
        {
            error = "\n";
            Invoke(new Action(() => { isDoThreadActive = true; }));
            Invoke(new Action(() => { this.moveStatus.Text = "In Progress..."; }));
            Invoke(new Action(() => { this.moveStatusBar.Minimum = 0; }));
            Invoke(new Action(() => { this.moveStatusBar.Step = 1;  }));

            if (source.Length > 3 && dest.Length > 3)
            {
                if (isFolderSelected)
                {
                    Invoke(new Action(() => { this.moveStatus.Text = "Enumerating Files..."; }));
                    Invoke(new Action(() => { this.moveStatusBar.Style = ProgressBarStyle.Marquee; }));
                    int files = enumerateFiles(source);
                    if (files == -1)
                    {
                        return;
                    }
                    Invoke(new Action(() => { this.moveStatusBar.Maximum = files + 4; }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Enumerated files."; }));
                    Invoke(new Action(() => { this.moveStatusBar.Style = ProgressBarStyle.Continuous; }));
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Copying files..."; }));
                    if (!copyDirectory(source, dest)) return;
                    Invoke(new Action(() => { this.moveStatus.Text = "Finished Copy."; }));
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Deleting source..."; }));
                    try
                    {
                        Directory.Delete(source, true);
                    }
                    catch (Exception e)
                    {
                        error += "\n ERROR: " + e.ToString() + ". Try running as administrator.\n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION AND MAY BE DAMAGED IN THE SOURCE FOLDER !!!\n\n Operation aborted.";
                        return;
                    }
                    Invoke(new Action(() => { this.moveStatus.Text = "Deleted source."; }));
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Linking directory"; }));
                    if (attemptJunct.Checked || attemptHard.Checked)
                    {
                        Process makeLink = Process.Start("CMD.exe", "/c mklink /J \"" + source + "\" \"" + dest + "\"");
                        makeLink.WaitForExit(1000);
                        if (!makeLink.HasExited)
                        {
                            error += "\n" + "ERROR: Make link never exited. \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation Aborted.";
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
                                    error += "\n" + "ERROR: Make link never exited. \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation Aborted."; ;
                                    return;
                                }
                                if (makeLink.ExitCode != 0)
                                {
                                    error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation aborted."; ;
                                    return;
                                }
                            }
                            else
                            {
                                error += "\n" +  "ERROR: Failed to create a junction with code " + makeLink.ExitCode + ". \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation aborted."; ;
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
                            error += "\n" + "ERROR: Make link never exited. \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation Aborted."; ;
                            return;
                        }
                        if (makeLink.ExitCode != 0)
                        {
                            error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ".\n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE FOLDER NO LONGER EXISTS. !!!\n\n Operation aborted."; ;
                            return;
                        }
                    }
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Done."; }));
                    if (error.Length > 1)
                    {
                        string showText = "Operation finished with the following warnings:" + error;
                        MessageBox.Show(showText, "Operation Finished", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Operation finished with no errors.", "Operation Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    Invoke(new Action(() => { this.moveStatusBar.Maximum = 5; }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Checking destination."; }));
                    if (!Directory.Exists(dest.Substring(0, dest.LastIndexOf('\\'))))
                    {
                        Directory.CreateDirectory(dest.Substring(0, dest.LastIndexOf('\\')));
                    }
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    if (File.Exists(dest) && !overwrite && !skipExisting)
                    {
                        error += "\n" +  "ERROR: File " + dest + " already exists. Enable overwrite to ignore this error. Operation aborted."; ;
                        return;
                    }
                    Invoke(new Action(() => { this.moveStatus.Text = "Destination Checked."; }));
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Copying file."; }));
                    try
                    {
                        if (File.Exists(dest) && !skipExisting)
                        {
                            File.Copy(source, dest, true);
                        }
                    }
                    catch (Exception e)
                    {
                        error += "\n ERROR: " + e.ToString() + ". Try running as administrator. Operation aborted.";
                        return;
                    }
                    Invoke(new Action(() => { this.moveStatus.Text = "File Copied."; }));
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Deleting Original File."; }));
                    File.Delete(source);
                    try
                    {
                        File.Delete(source);
                    }
                    catch (Exception e)
                    {
                        error += "\n ERROR: " + e.ToString() + ". Try running as administrator. Operation aborted.";
                        return;
                    }
                    Invoke(new Action(() => { this.moveStatus.Text = "Original File Deleted."; }));
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => { this.moveStatus.Text = "Linking file."; }));
                    if (attemptJunct.Checked || attemptHard.Checked)
                    {
                        Process makeLink = Process.Start("CMD.exe", "/c mklink /H \"" + source + "\" \"" + dest + "\"");
                        makeLink.WaitForExit(1000);
                        if (!makeLink.HasExited)
                        {
                            error += "\n" +  "ERROR: Make link never exited. \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation Aborted.";;
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
                                    error += "\n" +  "ERROR: Make link never exited. \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation Aborted.";;
                                    return;
                                }
                                if (makeLink.ExitCode != 0)
                                {
                                    error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation aborted.";;
                                    return;
                                }
                            }
                            else
                            {
                                error += "\n" +  "ERROR: Failed to create a Hardlink with code " + makeLink.ExitCode + ". \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation aborted.";;
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
                            error += "\n" + "ERROR: Make link never exited. \n\n!!! YOUR DATA HAS BEEN SUCCESSFULLY COPIED TO THE DESTINATION. THE SOURCE NO LONGER EXISTS. !!!\n\n Operation Aborted."; ;
                            return;
                        }
                        if (makeLink.ExitCode != 0)
                        {
                            error += "\n" +  "ERROR: Failed to create symlink with code " + makeLink.ExitCode + ". Operation aborted.";;
                            return;
                        }
                    }
                    Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                    Invoke(new Action(() => this.moveStatus.Text = "Done"));
                    if (error.Length > 1)
                    {
                        string showText = "Operation finished with the following warnings:" + error;
                        MessageBox.Show(showText, "Operation Finished", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Operation finished with no errors.", "Operation Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                error += "\n" +  "ERROR: Both Source and Destination need to be a proper file or directory. Operation aborted";;
                Console.WriteLine(source);
                Console.WriteLine(dest);
                return;
            }
            Invoke(new Action(() => { isDoThreadActive = false; }));
            foreach (Control i in this.Controls)
            {
                Invoke(new Action(() => { i.Enabled = true; }));
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
                try {
                    Directory.CreateDirectory(dest);
                }
                catch (Exception e)
                {
                    error += "\n ERROR: " + e.ToString() + ". Try running as administrator. Operation aborted.";
                    return false;
                }
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string path = Path.Combine(dest, file.Name);
                if(File.Exists(path) && !overwrite && !skipExisting)
                {
                    error += "\n" +  "ERROR: File " + path + " Already existed. Enable overwrite to ignore this error. Operation aborted.";
                    return false;
                }
                else
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            if(skipExisting)
                            {
                                Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                                Invoke(new Action(() => { this.moveStatus.Text = "Ignored " + file.Name; }));
                            } else if (overwrite)
                            {
                                file.CopyTo(path, true);
                                Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                                Invoke(new Action(() => { this.moveStatus.Text = "Overwritten " + file.Name; }));
                            } else
                            {
                                error += "\n" + "ERROR: File " + path + " Already existed. This error is abnormal (code 1), Please report it as a bug! Operation aborted.";
                                return false;
                            }
                        } else
                        {
                            Invoke(new Action(() => { this.moveStatus.Text = "Copying " + file.Name; }));
                            file.CopyTo(path, true);
                            Invoke(new Action(() => { this.moveStatusBar.PerformStep(); }));
                        }
                    }
                    catch (Exception e)
                    {
                        error += "\n ERROR: " + e.ToString() + ". Try running as administrator. Operation aborted.";
                        return false;
                    }
                }
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

        private void startBatchMove_Click(object sender, EventArgs e)
        {
            error = "\n";
            this.moveStatus.Text = "In Progress...";
            doThread = new Thread(() =>
            {
                Invoke(new Action(() => { isDoThreadActive = false; }));
                List<List<string>> lines = new List<List<string>>();
                foreach (var i in batchFilesList.Text.Split('\n'))
                {
                    string[] srcdstcombo = i.Split('>');
                    if (srcdstcombo.Length != 2)
                    {
                        int badLine = Array.IndexOf(batchFilesList.Text.Split('\n'), i);
                        error += "\n" + "ERROR: Failed to parse batch list: Too little or many arguments on line " + badLine + ". Operation aborted."; ;
                        return;
                    }
                    int insertTo = lines.Count;
                    List<string> temp = new List<string>();
                    if (!File.Exists(srcdstcombo[0]) && !Directory.Exists(srcdstcombo[0]))
                    {
                        int badLine = Array.IndexOf(batchFilesList.Text.Split('\n'), i);
                        error += "\n" + "ERROR: File/Folder " + srcdstcombo[0] + " Didnt exist on line " + badLine + " . Operation aborted."; ;
                        return;
                    }
                    temp.Add(srcdstcombo[0]);
                    temp.Add(srcdstcombo[1]);
                    lines.Add(temp);
                }
                for (var i = 0; i < lines.Count; i++)
                {
                    if (!File.Exists(lines[i][0]))
                    {
                        if (!Directory.Exists(lines[i][0]))
                        {
                            error += "\n" + "ERROR: File/Folder " + lines[i][0] + " Didnt exist between parsing and doing. Operation aborted."; ;
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
                    MessageBox.Show(showText, "Operation Finished", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Operation finished with no errors.", "Operation Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Invoke(new Action(() => this.moveStatus.Text = "Done."));
                Invoke(new Action(() => { isDoThreadActive = false; }));
                foreach (Control i in this.Controls)
                {
                    Invoke(new Action(() => { i.Enabled = true; }));
                }
            });
            doThread.Start();
            foreach (Control i in this.Controls)
            {
                if (i.Name.StartsWith("moveStatus")) continue;
                i.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            watchThread.Abort();
        }

        private void existingFilesBehaviour_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (existingFilesBehaviour.SelectedIndex)
            {
                case 0:
                    overwrite = false;
                    skipExisting = false;
                    break;
                case 1:
                    overwrite = false;
                    skipExisting = true;
                    break;
                case 2:
                    overwrite = true;
                    skipExisting = false;
                    break;
            }
        }
    }
}

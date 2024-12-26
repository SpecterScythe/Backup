using System.IO;

namespace Backup
{
    public partial class Form1 : Form
    {
        int Days = 0;
        int Hours = 0;
        int Minutes = 0;
        int countdown = 0;
        public Form1()
        {
            InitializeComponent();
            if (Properties.Settings.Default.FoldersToSave != null)
            {
                FoldersTxtBx.Text = Properties.Settings.Default.FoldersToSave;
            }
        }

        private void BackupBtn_Click(object sender, EventArgs e)
        {
            string backupLocation = Properties.Settings.Default.BackupLocation;
            if (string.IsNullOrEmpty(backupLocation))
            {
                MessageBox.Show("Please set a backup location before starting the backup.", "Backup Location Not Set", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(FoldersTxtBx.Text))
            {
                MessageBox.Show("No folders to backup. Please add folders first.", "No Folders", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get the list of folders to back up
                string[] foldersToBackup = FoldersTxtBx.Text.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                int totalFiles = 0;
                foreach (string folder in foldersToBackup)
                {
                    if (Directory.Exists(folder))
                    {
                        totalFiles += Directory.GetFiles(folder, "*", SearchOption.AllDirectories).Length;
                    }
                }

                BackupProgressBar.Value = 0;
                BackupProgressBar.Maximum = totalFiles;

                int copiedFiles = 0;
                

                foreach (string folder in foldersToBackup)
                {
                    if (Directory.Exists(folder))
                    {
                        // Ensure each folder is backed up in its own subdirectory
                        string folderName = new DirectoryInfo(folder).Name;
                        string destinationPath = Path.Combine(backupLocation, folderName);

                        // Copy the folder and its contents
                        CopyFolderWithProgress(folder, destinationPath, ref copiedFiles, totalFiles);
                    }
                    else
                    {
                        MessageBox.Show($"The folder \"{folder}\" does not exist and will be skipped.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                MessageBox.Show("Backup completed successfully.", "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during backup: {ex.Message}", "Backup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyFolderWithProgress(string sourcePath, string destinationPath, ref int copiedFiles, int totalFiles)
        {
            // Create the destination directory if it doesn't exist
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Copy all files
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string destFile = Path.Combine(destinationPath, Path.GetFileName(file));
                File.Copy(file, destFile, true); // Overwrite if the file exists

                copiedFiles++;
                UpdateProgressBar(copiedFiles, totalFiles);
            }

            // Recursively copy all subdirectories
            foreach (string directory in Directory.GetDirectories(sourcePath))
            {
                string destDirectory = Path.Combine(destinationPath, Path.GetFileName(directory));
                CopyFolderWithProgress(directory, destDirectory, ref copiedFiles, totalFiles);
            }
        }

        private void UpdateProgressBar(int copiedFiles, int totalFiles)
        {
            if (BackupProgressBar.InvokeRequired)
            {
                BackupProgressBar.Invoke(new Action(() =>
                {
                    BackupProgressBar.Value = copiedFiles;
                }));
            }
            else
            {
                BackupProgressBar.Value = copiedFiles;
            }
        }

        private void AddFolderBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    if (!FoldersTxtBx.Text.Contains(selectedPath))
                    {
                        if (!string.IsNullOrWhiteSpace(FoldersTxtBx.Text))
                        {
                            FoldersTxtBx.AppendText("|");
                        }
                        FoldersTxtBx.AppendText(selectedPath);
                        Properties.Settings.Default.FoldersToSave = FoldersTxtBx.Text;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("This folder is already added.", "Duplicate Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void SetTimerBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Days = int.Parse(DaysTxtBx.Text);
                Hours = int.Parse(HrsTxtBx.Text);
                Minutes = int.Parse(MinTxtBx.Text);
            }
            catch { }
            int i = 0;
            try
            {
                while (i < Days)
                {
                    DateTime dt = DateTime.Now.AddDays(i);
                    DateTime targetTime = dt.Date.AddHours(Hours).AddMinutes(Minutes);
                    if (DateTime.Now < targetTime)
                    {
                        Timer.Interval = (int)Math.Abs((targetTime - DateTime.Now).TotalMilliseconds);
                        countdown = (int)Math.Abs((targetTime - DateTime.Now).TotalMilliseconds);
                        Timer.Start();
                        timer1.Start();
                        break;
                    }
                    i++;
                }
            }
            catch { }
        }

        private void StopTimerBtn_Click(object sender, EventArgs e)
        {
            Countdown.Text = "";
            Timer.Stop();
            Timer.Interval = 100;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string backupLocation = Properties.Settings.Default.BackupLocation;
            if (string.IsNullOrEmpty(backupLocation))
            {
                MessageBox.Show("Please set a backup location before starting the backup.", "Backup Location Not Set", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(FoldersTxtBx.Text))
            {
                MessageBox.Show("No folders to backup. Please add folders first.", "No Folders", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get the list of folders to back up
                string[] foldersToBackup = FoldersTxtBx.Text.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                int totalFiles = 0;
                foreach (string folder in foldersToBackup)
                {
                    if (Directory.Exists(folder))
                    {
                        totalFiles += Directory.GetFiles(folder, "*", SearchOption.AllDirectories).Length;
                    }
                }

                BackupProgressBar.Value = 0;
                BackupProgressBar.Maximum = totalFiles;

                int copiedFiles = 0;


                foreach (string folder in foldersToBackup)
                {
                    if (Directory.Exists(folder))
                    {
                        // Ensure each folder is backed up in its own subdirectory
                        string folderName = new DirectoryInfo(folder).Name;
                        string destinationPath = Path.Combine(backupLocation, folderName);

                        // Copy the folder and its contents
                        CopyFolderWithProgress(folder, destinationPath, ref copiedFiles, totalFiles);
                    }
                    else
                    {
                        MessageBox.Show($"The folder \"{folder}\" does not exist and will be skipped.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                MessageBox.Show("Backup completed successfully.", "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during backup: {ex.Message}", "Backup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackupLocationBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Backup Location";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.BackupLocation = folderDialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    BackupBtn.Text = "Saved";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            countdown -= 100;
            Countdown.Text = countdown.ToString();
        }

        private void ClearFoldersBtn_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackupLocation = null;
            Properties.Settings.Default.FoldersToSave = null;
            FoldersTxtBx.Text = null;
            Properties.Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
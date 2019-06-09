namespace In_Place_Mover
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.fileSource = new System.Windows.Forms.TextBox();
            this.selFileSource = new System.Windows.Forms.Button();
            this.selFolderSource = new System.Windows.Forms.Button();
            this.selFolderDest = new System.Windows.Forms.Button();
            this.fileDest = new System.Windows.Forms.TextBox();
            this.startMove = new System.Windows.Forms.Button();
            this.attemptSym = new System.Windows.Forms.RadioButton();
            this.attemptHard = new System.Windows.Forms.RadioButton();
            this.attemptJunct = new System.Windows.Forms.RadioButton();
            this.advanced = new System.Windows.Forms.Button();
            this.advancedLbl = new System.Windows.Forms.Label();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.batchFilesList = new System.Windows.Forms.RichTextBox();
            this.loadBatchFile = new System.Windows.Forms.Button();
            this.startBatchMove = new System.Windows.Forms.Button();
            this.moveStatusBar = new System.Windows.Forms.ProgressBar();
            this.moveStatus = new System.Windows.Forms.Label();
            this.fallbackCheck = new System.Windows.Forms.CheckBox();
            this.overwriteBtn = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // fileSource
            // 
            this.fileSource.Location = new System.Drawing.Point(12, 12);
            this.fileSource.Name = "fileSource";
            this.fileSource.Size = new System.Drawing.Size(317, 20);
            this.fileSource.TabIndex = 0;
            // 
            // selFileSource
            // 
            this.selFileSource.Location = new System.Drawing.Point(335, 12);
            this.selFileSource.Name = "selFileSource";
            this.selFileSource.Size = new System.Drawing.Size(65, 20);
            this.selFileSource.TabIndex = 2;
            this.selFileSource.Text = "Select File";
            this.selFileSource.UseVisualStyleBackColor = true;
            this.selFileSource.Click += new System.EventHandler(this.selFileSource_Click);
            // 
            // selFolderSource
            // 
            this.selFolderSource.Location = new System.Drawing.Point(406, 12);
            this.selFolderSource.Name = "selFolderSource";
            this.selFolderSource.Size = new System.Drawing.Size(84, 20);
            this.selFolderSource.TabIndex = 3;
            this.selFolderSource.Text = "Select Folder";
            this.selFolderSource.UseVisualStyleBackColor = true;
            this.selFolderSource.Click += new System.EventHandler(this.selFolderSource_Click);
            // 
            // selFolderDest
            // 
            this.selFolderDest.Location = new System.Drawing.Point(335, 38);
            this.selFolderDest.Name = "selFolderDest";
            this.selFolderDest.Size = new System.Drawing.Size(155, 20);
            this.selFolderDest.TabIndex = 6;
            this.selFolderDest.Text = "Select Folder";
            this.selFolderDest.UseVisualStyleBackColor = true;
            this.selFolderDest.Click += new System.EventHandler(this.selFolderDest_Click);
            // 
            // fileDest
            // 
            this.fileDest.Location = new System.Drawing.Point(12, 38);
            this.fileDest.Name = "fileDest";
            this.fileDest.Size = new System.Drawing.Size(317, 20);
            this.fileDest.TabIndex = 4;
            // 
            // startMove
            // 
            this.startMove.Location = new System.Drawing.Point(335, 65);
            this.startMove.Name = "startMove";
            this.startMove.Size = new System.Drawing.Size(65, 23);
            this.startMove.TabIndex = 8;
            this.startMove.Text = "Go";
            this.startMove.UseVisualStyleBackColor = true;
            this.startMove.Click += new System.EventHandler(this.startMove_Click);
            // 
            // attemptSym
            // 
            this.attemptSym.AutoSize = true;
            this.attemptSym.Location = new System.Drawing.Point(12, 168);
            this.attemptSym.Name = "attemptSym";
            this.attemptSym.Size = new System.Drawing.Size(90, 17);
            this.attemptSym.TabIndex = 9;
            this.attemptSym.Text = "Symbolic Link";
            this.tooltip.SetToolTip(this.attemptSym, "Force symbolic link for everything.");
            this.attemptSym.UseVisualStyleBackColor = true;
            // 
            // attemptHard
            // 
            this.attemptHard.AutoSize = true;
            this.attemptHard.Location = new System.Drawing.Point(12, 145);
            this.attemptHard.Name = "attemptHard";
            this.attemptHard.Size = new System.Drawing.Size(71, 17);
            this.attemptHard.TabIndex = 10;
            this.attemptHard.Text = "Hard Link";
            this.tooltip.SetToolTip(this.attemptHard, "Attempt a hard link if possible.");
            this.attemptHard.UseVisualStyleBackColor = true;
            // 
            // attemptJunct
            // 
            this.attemptJunct.AutoSize = true;
            this.attemptJunct.Checked = true;
            this.attemptJunct.Location = new System.Drawing.Point(12, 122);
            this.attemptJunct.Name = "attemptJunct";
            this.attemptJunct.Size = new System.Drawing.Size(65, 17);
            this.attemptJunct.TabIndex = 11;
            this.attemptJunct.TabStop = true;
            this.attemptJunct.Text = "Junction";
            this.tooltip.SetToolTip(this.attemptJunct, "Attempt a directory junction if possible (Default)");
            this.attemptJunct.UseVisualStyleBackColor = true;
            // 
            // advanced
            // 
            this.advanced.Location = new System.Drawing.Point(406, 65);
            this.advanced.Name = "advanced";
            this.advanced.Size = new System.Drawing.Size(84, 23);
            this.advanced.TabIndex = 12;
            this.advanced.Text = "Advanced";
            this.advanced.UseVisualStyleBackColor = true;
            this.advanced.Click += new System.EventHandler(this.advanced_Click);
            // 
            // advancedLbl
            // 
            this.advancedLbl.AutoSize = true;
            this.advancedLbl.Location = new System.Drawing.Point(12, 106);
            this.advancedLbl.Name = "advancedLbl";
            this.advancedLbl.Size = new System.Drawing.Size(97, 13);
            this.advancedLbl.TabIndex = 13;
            this.advancedLbl.Text = "Advanced Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(115, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Batch Move";
            // 
            // batchFilesList
            // 
            this.batchFilesList.Location = new System.Drawing.Point(118, 126);
            this.batchFilesList.Name = "batchFilesList";
            this.batchFilesList.Size = new System.Drawing.Size(372, 73);
            this.batchFilesList.TabIndex = 15;
            this.batchFilesList.Text = "";
            // 
            // loadBatchFile
            // 
            this.loadBatchFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadBatchFile.Location = new System.Drawing.Point(183, 101);
            this.loadBatchFile.Margin = new System.Windows.Forms.Padding(0);
            this.loadBatchFile.Name = "loadBatchFile";
            this.loadBatchFile.Size = new System.Drawing.Size(85, 22);
            this.loadBatchFile.TabIndex = 16;
            this.loadBatchFile.Text = "Load From File";
            this.loadBatchFile.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.loadBatchFile.UseVisualStyleBackColor = true;
            this.loadBatchFile.Click += new System.EventHandler(this.loadBatchFile_Click);
            // 
            // startBatchMove
            // 
            this.startBatchMove.Location = new System.Drawing.Point(271, 101);
            this.startBatchMove.Name = "startBatchMove";
            this.startBatchMove.Size = new System.Drawing.Size(65, 22);
            this.startBatchMove.TabIndex = 17;
            this.startBatchMove.Text = "Go";
            this.startBatchMove.UseVisualStyleBackColor = true;
            this.startBatchMove.Click += new System.EventHandler(this.startBatchMove_Click);
            // 
            // moveStatusBar
            // 
            this.moveStatusBar.Location = new System.Drawing.Point(12, 64);
            this.moveStatusBar.MarqueeAnimationSpeed = 15;
            this.moveStatusBar.Name = "moveStatusBar";
            this.moveStatusBar.Size = new System.Drawing.Size(317, 23);
            this.moveStatusBar.TabIndex = 19;
            // 
            // moveStatus
            // 
            this.moveStatus.AutoSize = true;
            this.moveStatus.BackColor = System.Drawing.SystemColors.Control;
            this.moveStatus.Location = new System.Drawing.Point(12, 87);
            this.moveStatus.Name = "moveStatus";
            this.moveStatus.Size = new System.Drawing.Size(41, 13);
            this.moveStatus.TabIndex = 20;
            this.moveStatus.Text = "Ready.";
            // 
            // fallbackCheck
            // 
            this.fallbackCheck.AutoSize = true;
            this.fallbackCheck.Checked = true;
            this.fallbackCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fallbackCheck.Location = new System.Drawing.Point(11, 190);
            this.fallbackCheck.Name = "fallbackCheck";
            this.fallbackCheck.Size = new System.Drawing.Size(94, 17);
            this.fallbackCheck.TabIndex = 21;
            this.fallbackCheck.Text = "Fallback Links";
            this.fallbackCheck.UseVisualStyleBackColor = true;
            this.fallbackCheck.CheckedChanged += new System.EventHandler(this.fallbackCheck_CheckedChanged);
            // 
            // overwriteBtn
            // 
            this.overwriteBtn.AutoSize = true;
            this.overwriteBtn.Location = new System.Drawing.Point(343, 103);
            this.overwriteBtn.Name = "overwriteBtn";
            this.overwriteBtn.Size = new System.Drawing.Size(134, 17);
            this.overwriteBtn.TabIndex = 22;
            this.overwriteBtn.Text = "Overwrite Existing Files";
            this.overwriteBtn.UseVisualStyleBackColor = true;
            this.overwriteBtn.CheckedChanged += new System.EventHandler(this.overwriteBtn_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 101);
            this.Controls.Add(this.overwriteBtn);
            this.Controls.Add(this.fallbackCheck);
            this.Controls.Add(this.moveStatus);
            this.Controls.Add(this.moveStatusBar);
            this.Controls.Add(this.startBatchMove);
            this.Controls.Add(this.loadBatchFile);
            this.Controls.Add(this.batchFilesList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.advancedLbl);
            this.Controls.Add(this.advanced);
            this.Controls.Add(this.attemptJunct);
            this.Controls.Add(this.attemptHard);
            this.Controls.Add(this.attemptSym);
            this.Controls.Add(this.startMove);
            this.Controls.Add(this.selFolderDest);
            this.Controls.Add(this.fileDest);
            this.Controls.Add(this.selFolderSource);
            this.Controls.Add(this.selFileSource);
            this.Controls.Add(this.fileSource);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(516, 250);
            this.MinimumSize = new System.Drawing.Size(516, 140);
            this.Name = "Form1";
            this.Text = "In-Place Mover";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fileSource;
        private System.Windows.Forms.Button selFileSource;
        private System.Windows.Forms.Button selFolderSource;
        private System.Windows.Forms.Button selFolderDest;
        private System.Windows.Forms.TextBox fileDest;
        private System.Windows.Forms.Button startMove;
        private System.Windows.Forms.RadioButton attemptSym;
        private System.Windows.Forms.RadioButton attemptHard;
        private System.Windows.Forms.RadioButton attemptJunct;
        private System.Windows.Forms.Button advanced;
        private System.Windows.Forms.Label advancedLbl;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox batchFilesList;
        private System.Windows.Forms.Button loadBatchFile;
        private System.Windows.Forms.Button startBatchMove;
        private System.Windows.Forms.ProgressBar moveStatusBar;
        private System.Windows.Forms.Label moveStatus;
        private System.Windows.Forms.CheckBox fallbackCheck;
        private System.Windows.Forms.CheckBox overwriteBtn;
    }
}


namespace MigratorUI
{
    partial class TDSMigrator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TDSMigrator));
            this.pickFileButton = new System.Windows.Forms.Button();
            this.TDDPathTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.repositoryName = new System.Windows.Forms.TextBox();
            this.repositoryNameText = new System.Windows.Forms.Label();
            this.createRepositoryButton = new System.Windows.Forms.Button();
            this.clearRepositoryButton = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.deselectAllButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.apiURL = new System.Windows.Forms.Label();
            this.apiUrlTextBox = new System.Windows.Forms.TextBox();
            this.deleteRepositoryButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.loadRefreshRepositories = new System.Windows.Forms.Button();
            this.repositoriesBox = new System.Windows.Forms.ListBox();
            this.repositoryDescriptionText = new System.Windows.Forms.Label();
            this.repositoryDescriptionTextbox = new System.Windows.Forms.TextBox();
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.verifyUrlButton = new System.Windows.Forms.Button();
            this.categoriesLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // pickFileButton
            // 
            this.pickFileButton.BackColor = System.Drawing.SystemColors.Control;
            this.pickFileButton.Enabled = false;
            this.pickFileButton.Location = new System.Drawing.Point(438, 60);
            this.pickFileButton.Name = "pickFileButton";
            this.pickFileButton.Size = new System.Drawing.Size(130, 27);
            this.pickFileButton.TabIndex = 4;
            this.pickFileButton.Text = "...";
            this.pickFileButton.UseVisualStyleBackColor = false;
            this.pickFileButton.Click += new System.EventHandler(this.pickFileButton_Click);
            // 
            // TDDPathTextBox
            // 
            this.TDDPathTextBox.Enabled = false;
            this.TDDPathTextBox.Location = new System.Drawing.Point(164, 62);
            this.TDDPathTextBox.Name = "TDDPathTextBox";
            this.TDDPathTextBox.Size = new System.Drawing.Size(248, 22);
            this.TDDPathTextBox.TabIndex = 3;
            this.TDDPathTextBox.TextChanged += new System.EventHandler(this.TDDPathTextBox_TextChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "tdd";
            this.openFileDialog.Filter = "tdd files|*.tdd";
            this.openFileDialog.InitialDirectory = "C:\\Users\\";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(57, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select .tdd file ";
            // 
            // GenerateButton
            // 
            this.GenerateButton.Enabled = false;
            this.GenerateButton.Location = new System.Drawing.Point(56, 634);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(512, 59);
            this.GenerateButton.TabIndex = 13;
            this.GenerateButton.Text = "Load categories into repository";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.loadIntoRepositoryButton_Click);
            // 
            // repositoryName
            // 
            this.repositoryName.Enabled = false;
            this.repositoryName.Location = new System.Drawing.Point(245, 566);
            this.repositoryName.Name = "repositoryName";
            this.repositoryName.Size = new System.Drawing.Size(167, 22);
            this.repositoryName.TabIndex = 10;
            // 
            // repositoryNameText
            // 
            this.repositoryNameText.AutoSize = true;
            this.repositoryNameText.BackColor = System.Drawing.Color.Transparent;
            this.repositoryNameText.Location = new System.Drawing.Point(57, 566);
            this.repositoryNameText.Name = "repositoryNameText";
            this.repositoryNameText.Size = new System.Drawing.Size(115, 17);
            this.repositoryNameText.TabIndex = 5;
            this.repositoryNameText.Text = "Repository name";
            // 
            // createRepositoryButton
            // 
            this.createRepositoryButton.Enabled = false;
            this.createRepositoryButton.Location = new System.Drawing.Point(438, 566);
            this.createRepositoryButton.Name = "createRepositoryButton";
            this.createRepositoryButton.Size = new System.Drawing.Size(130, 33);
            this.createRepositoryButton.TabIndex = 12;
            this.createRepositoryButton.Text = "Create repository";
            this.createRepositoryButton.UseVisualStyleBackColor = true;
            this.createRepositoryButton.Click += new System.EventHandler(this.createRepositoryButton_Click);
            // 
            // clearRepositoryButton
            // 
            this.clearRepositoryButton.Enabled = false;
            this.clearRepositoryButton.Location = new System.Drawing.Point(438, 414);
            this.clearRepositoryButton.Name = "clearRepositoryButton";
            this.clearRepositoryButton.Size = new System.Drawing.Size(130, 33);
            this.clearRepositoryButton.TabIndex = 7;
            this.clearRepositoryButton.Text = "Clear repository";
            this.clearRepositoryButton.UseVisualStyleBackColor = false;
            this.clearRepositoryButton.Click += new System.EventHandler(this.clearRepositoryButton_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.ColumnWidth = 111;
            this.checkedListBox1.Enabled = false;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(56, 143);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkedListBox1.Size = new System.Drawing.Size(356, 225);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 15;
            this.checkedListBox1.ThreeDCheckBoxes = true;
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.Enabled = false;
            this.deselectAllButton.Location = new System.Drawing.Point(438, 182);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(130, 33);
            this.deselectAllButton.TabIndex = 6;
            this.deselectAllButton.Text = "Deselect All";
            this.deselectAllButton.UseVisualStyleBackColor = true;
            this.deselectAllButton.Click += new System.EventHandler(this.deselectAllButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Enabled = false;
            this.selectAllButton.Location = new System.Drawing.Point(438, 143);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(130, 33);
            this.selectAllButton.TabIndex = 5;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // apiURL
            // 
            this.apiURL.AutoSize = true;
            this.apiURL.BackColor = System.Drawing.Color.Transparent;
            this.apiURL.Location = new System.Drawing.Point(57, 36);
            this.apiURL.Name = "apiURL";
            this.apiURL.Size = new System.Drawing.Size(69, 17);
            this.apiURL.TabIndex = 13;
            this.apiURL.Text = "API URL :";
            // 
            // apiUrlTextBox
            // 
            this.apiUrlTextBox.BackColor = System.Drawing.Color.White;
            this.apiUrlTextBox.Location = new System.Drawing.Point(164, 33);
            this.apiUrlTextBox.Name = "apiUrlTextBox";
            this.apiUrlTextBox.Size = new System.Drawing.Size(248, 22);
            this.apiUrlTextBox.TabIndex = 1;
            this.apiUrlTextBox.Text = "http://localhost:81/testdataservice";
            // 
            // deleteRepositoryButton
            // 
            this.deleteRepositoryButton.Enabled = false;
            this.deleteRepositoryButton.Location = new System.Drawing.Point(438, 453);
            this.deleteRepositoryButton.Name = "deleteRepositoryButton";
            this.deleteRepositoryButton.Size = new System.Drawing.Size(130, 33);
            this.deleteRepositoryButton.TabIndex = 8;
            this.deleteRepositoryButton.Text = "Delete repository";
            this.deleteRepositoryButton.UseVisualStyleBackColor = false;
            this.deleteRepositoryButton.Click += new System.EventHandler(this.deleteRepositoryButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(57, 386);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 17);
            this.label3.TabIndex = 19;
            this.label3.Text = "Repositories";
            // 
            // loadRefreshRepositories
            // 
            this.loadRefreshRepositories.Enabled = false;
            this.loadRefreshRepositories.Location = new System.Drawing.Point(438, 492);
            this.loadRefreshRepositories.Name = "loadRefreshRepositories";
            this.loadRefreshRepositories.Size = new System.Drawing.Size(130, 33);
            this.loadRefreshRepositories.TabIndex = 9;
            this.loadRefreshRepositories.Text = "Refresh";
            this.loadRefreshRepositories.UseVisualStyleBackColor = true;
            this.loadRefreshRepositories.Click += new System.EventHandler(this.loadRefreshRepositories_Click);
            // 
            // repositoriesBox
            // 
            this.repositoriesBox.Enabled = false;
            this.repositoriesBox.FormattingEnabled = true;
            this.repositoriesBox.ItemHeight = 16;
            this.repositoriesBox.Location = new System.Drawing.Point(56, 414);
            this.repositoriesBox.Name = "repositoriesBox";
            this.repositoriesBox.Size = new System.Drawing.Size(356, 132);
            this.repositoriesBox.Sorted = true;
            this.repositoriesBox.TabIndex = 16;
            // 
            // repositoryDescriptionText
            // 
            this.repositoryDescriptionText.AutoSize = true;
            this.repositoryDescriptionText.BackColor = System.Drawing.Color.Transparent;
            this.repositoryDescriptionText.Location = new System.Drawing.Point(57, 593);
            this.repositoryDescriptionText.Name = "repositoryDescriptionText";
            this.repositoryDescriptionText.Size = new System.Drawing.Size(149, 17);
            this.repositoryDescriptionText.TabIndex = 23;
            this.repositoryDescriptionText.Text = "Repository description";
            // 
            // repositoryDescriptionTextbox
            // 
            this.repositoryDescriptionTextbox.Enabled = false;
            this.repositoryDescriptionTextbox.Location = new System.Drawing.Point(245, 593);
            this.repositoryDescriptionTextbox.Name = "repositoryDescriptionTextbox";
            this.repositoryDescriptionTextbox.Size = new System.Drawing.Size(167, 22);
            this.repositoryDescriptionTextbox.TabIndex = 11;
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(625, 32);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(310, 657);
            this.logTextBox.TabIndex = 14;
            this.logTextBox.Text = "";
            this.logTextBox.TextChanged += new System.EventHandler(this.logTextBox_TextChanged);
            // 
            // verifyUrlButton
            // 
            this.verifyUrlButton.Location = new System.Drawing.Point(438, 32);
            this.verifyUrlButton.Name = "verifyUrlButton";
            this.verifyUrlButton.Size = new System.Drawing.Size(130, 25);
            this.verifyUrlButton.TabIndex = 2;
            this.verifyUrlButton.Text = "Verify Url";
            this.verifyUrlButton.UseVisualStyleBackColor = true;
            this.verifyUrlButton.Click += new System.EventHandler(this.verifyUrlButton_Click_1);
            // 
            // categoriesLabel
            // 
            this.categoriesLabel.AutoSize = true;
            this.categoriesLabel.BackColor = System.Drawing.Color.Transparent;
            this.categoriesLabel.Location = new System.Drawing.Point(57, 112);
            this.categoriesLabel.Name = "categoriesLabel";
            this.categoriesLabel.Size = new System.Drawing.Size(76, 17);
            this.categoriesLabel.TabIndex = 26;
            this.categoriesLabel.Text = "Categories";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(625, 634);
            this.progressBar.MarqueeAnimationSpeed = 25;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(310, 59);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 27;
            this.progressBar.Visible = false;
            // 
            // TDSMigrator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(992, 733);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.categoriesLabel);
            this.Controls.Add(this.verifyUrlButton);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.repositoryDescriptionText);
            this.Controls.Add(this.repositoryDescriptionTextbox);
            this.Controls.Add(this.repositoriesBox);
            this.Controls.Add(this.loadRefreshRepositories);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.deleteRepositoryButton);
            this.Controls.Add(this.apiUrlTextBox);
            this.Controls.Add(this.apiURL);
            this.Controls.Add(this.selectAllButton);
            this.Controls.Add(this.deselectAllButton);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.clearRepositoryButton);
            this.Controls.Add(this.createRepositoryButton);
            this.Controls.Add(this.repositoryNameText);
            this.Controls.Add(this.repositoryName);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TDDPathTextBox);
            this.Controls.Add(this.pickFileButton);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TDSMigrator";
            this.Text = "Tricentis TDM to TDS Migrator";
            this.Load += new System.EventHandler(this.TDSMigrator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pickFileButton;
        private System.Windows.Forms.TextBox TDDPathTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.TextBox repositoryName;
        private System.Windows.Forms.Label repositoryNameText;
        private System.Windows.Forms.Button createRepositoryButton;
        private System.Windows.Forms.Button clearRepositoryButton;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button deselectAllButton;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Label apiURL;
        private System.Windows.Forms.TextBox apiUrlTextBox;
        private System.Windows.Forms.Button deleteRepositoryButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadRefreshRepositories;
        private System.Windows.Forms.ListBox repositoriesBox;
        private System.Windows.Forms.Label repositoryDescriptionText;
        private System.Windows.Forms.TextBox repositoryDescriptionTextbox;
        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.Button verifyUrlButton;
        private System.Windows.Forms.Label categoriesLabel;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}


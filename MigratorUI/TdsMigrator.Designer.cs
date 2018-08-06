namespace MigratorUI
{
    partial class TdsMigrator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TdsMigrator));
            this.pickFileButton = new System.Windows.Forms.Button();
            this.TDDPathTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.repositoryNameTextBox = new System.Windows.Forms.TextBox();
            this.repositoryNameText = new System.Windows.Forms.Label();
            this.createRepositoryButton = new System.Windows.Forms.Button();
            this.clearRepositoryButton = new System.Windows.Forms.Button();
            this.categoriesListBox = new System.Windows.Forms.CheckedListBox();
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
            this.tddFileProcessingProgressBar = new System.Windows.Forms.ProgressBar();
            this.loadIntoRepositoryButton = new System.Windows.Forms.Button();
            this.migrationProgressBar = new System.Windows.Forms.ProgressBar();
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
            this.pickFileButton.Click += new System.EventHandler(this.PickFileButton_Click);
            // 
            // TDDPathTextBox
            // 
            this.TDDPathTextBox.Enabled = false;
            this.TDDPathTextBox.Location = new System.Drawing.Point(164, 62);
            this.TDDPathTextBox.Name = "TDDPathTextBox";
            this.TDDPathTextBox.Size = new System.Drawing.Size(248, 22);
            this.TDDPathTextBox.TabIndex = 3;
            this.TDDPathTextBox.TextChanged += new System.EventHandler(this.TddPathTextBox_TextChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "tdd";
            this.openFileDialog.Filter = "tdd files|*.tdd";
            this.openFileDialog.InitialDirectory = "C:\\Users\\";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog_FileOk);
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
            // repositoryNameTextBox
            // 
            this.repositoryNameTextBox.Enabled = false;
            this.repositoryNameTextBox.Location = new System.Drawing.Point(822, 210);
            this.repositoryNameTextBox.Name = "repositoryNameTextBox";
            this.repositoryNameTextBox.Size = new System.Drawing.Size(166, 22);
            this.repositoryNameTextBox.TabIndex = 10;
            // 
            // repositoryNameText
            // 
            this.repositoryNameText.AutoSize = true;
            this.repositoryNameText.BackColor = System.Drawing.Color.Transparent;
            this.repositoryNameText.Location = new System.Drawing.Point(646, 213);
            this.repositoryNameText.Name = "repositoryNameText";
            this.repositoryNameText.Size = new System.Drawing.Size(141, 17);
            this.repositoryNameText.TabIndex = 5;
            this.repositoryNameText.Text = "New repository name";
            // 
            // createRepositoryButton
            // 
            this.createRepositoryButton.Enabled = false;
            this.createRepositoryButton.Location = new System.Drawing.Point(1015, 210);
            this.createRepositoryButton.Name = "createRepositoryButton";
            this.createRepositoryButton.Size = new System.Drawing.Size(130, 49);
            this.createRepositoryButton.TabIndex = 12;
            this.createRepositoryButton.Text = "Create repository";
            this.createRepositoryButton.UseVisualStyleBackColor = true;
            this.createRepositoryButton.Click += new System.EventHandler(this.CreateRepositoryButton_Click);
            // 
            // clearRepositoryButton
            // 
            this.clearRepositoryButton.Enabled = false;
            this.clearRepositoryButton.Location = new System.Drawing.Point(1014, 61);
            this.clearRepositoryButton.Name = "clearRepositoryButton";
            this.clearRepositoryButton.Size = new System.Drawing.Size(130, 45);
            this.clearRepositoryButton.TabIndex = 7;
            this.clearRepositoryButton.Text = "Clear repository";
            this.clearRepositoryButton.UseVisualStyleBackColor = true;
            this.clearRepositoryButton.Click += new System.EventHandler(this.ClearRepositoryButton_Click);
            // 
            // categoriesListBox
            // 
            this.categoriesListBox.CheckOnClick = true;
            this.categoriesListBox.ColumnWidth = 111;
            this.categoriesListBox.Enabled = false;
            this.categoriesListBox.FormattingEnabled = true;
            this.categoriesListBox.Location = new System.Drawing.Point(632, 322);
            this.categoriesListBox.Name = "categoriesListBox";
            this.categoriesListBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.categoriesListBox.Size = new System.Drawing.Size(356, 225);
            this.categoriesListBox.Sorted = true;
            this.categoriesListBox.TabIndex = 15;
            this.categoriesListBox.ThreeDCheckBoxes = true;
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.Enabled = false;
            this.deselectAllButton.Location = new System.Drawing.Point(1010, 361);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(130, 33);
            this.deselectAllButton.TabIndex = 6;
            this.deselectAllButton.Text = "Deselect All";
            this.deselectAllButton.UseVisualStyleBackColor = true;
            this.deselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Enabled = false;
            this.selectAllButton.Location = new System.Drawing.Point(1010, 322);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(130, 33);
            this.selectAllButton.TabIndex = 5;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
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
            this.deleteRepositoryButton.Location = new System.Drawing.Point(1014, 112);
            this.deleteRepositoryButton.Name = "deleteRepositoryButton";
            this.deleteRepositoryButton.Size = new System.Drawing.Size(130, 43);
            this.deleteRepositoryButton.TabIndex = 8;
            this.deleteRepositoryButton.Text = "Delete repository";
            this.deleteRepositoryButton.UseVisualStyleBackColor = true;
            this.deleteRepositoryButton.Click += new System.EventHandler(this.DeleteRepositoryButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(633, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 17);
            this.label3.TabIndex = 19;
            this.label3.Text = "Repositories";
            // 
            // loadRefreshRepositories
            // 
            this.loadRefreshRepositories.Enabled = false;
            this.loadRefreshRepositories.Location = new System.Drawing.Point(1014, 161);
            this.loadRefreshRepositories.Name = "loadRefreshRepositories";
            this.loadRefreshRepositories.Size = new System.Drawing.Size(130, 32);
            this.loadRefreshRepositories.TabIndex = 9;
            this.loadRefreshRepositories.Text = "Refresh";
            this.loadRefreshRepositories.UseVisualStyleBackColor = true;
            this.loadRefreshRepositories.Click += new System.EventHandler(this.LoadRefreshRepositories_Click);
            // 
            // repositoriesBox
            // 
            this.repositoriesBox.Enabled = false;
            this.repositoriesBox.FormattingEnabled = true;
            this.repositoriesBox.ItemHeight = 16;
            this.repositoriesBox.Location = new System.Drawing.Point(632, 61);
            this.repositoriesBox.Name = "repositoriesBox";
            this.repositoriesBox.Size = new System.Drawing.Size(356, 132);
            this.repositoriesBox.Sorted = true;
            this.repositoriesBox.TabIndex = 16;
            this.repositoriesBox.SelectedValueChanged += new System.EventHandler(this.repositoriesBox_SelectedValueChanged);
            // 
            // repositoryDescriptionText
            // 
            this.repositoryDescriptionText.AutoSize = true;
            this.repositoryDescriptionText.BackColor = System.Drawing.Color.Transparent;
            this.repositoryDescriptionText.Location = new System.Drawing.Point(646, 240);
            this.repositoryDescriptionText.Name = "repositoryDescriptionText";
            this.repositoryDescriptionText.Size = new System.Drawing.Size(175, 17);
            this.repositoryDescriptionText.TabIndex = 23;
            this.repositoryDescriptionText.Text = "New repository description";
            // 
            // repositoryDescriptionTextbox
            // 
            this.repositoryDescriptionTextbox.Enabled = false;
            this.repositoryDescriptionTextbox.Location = new System.Drawing.Point(822, 237);
            this.repositoryDescriptionTextbox.Name = "repositoryDescriptionTextbox";
            this.repositoryDescriptionTextbox.Size = new System.Drawing.Size(166, 22);
            this.repositoryDescriptionTextbox.TabIndex = 11;
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(56, 100);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(512, 535);
            this.logTextBox.TabIndex = 14;
            this.logTextBox.Text = "";
            this.logTextBox.TextChanged += new System.EventHandler(this.LogTextBox_TextChanged);
            // 
            // verifyUrlButton
            // 
            this.verifyUrlButton.Location = new System.Drawing.Point(438, 32);
            this.verifyUrlButton.Name = "verifyUrlButton";
            this.verifyUrlButton.Size = new System.Drawing.Size(130, 25);
            this.verifyUrlButton.TabIndex = 2;
            this.verifyUrlButton.Text = "Verify Url";
            this.verifyUrlButton.UseVisualStyleBackColor = true;
            this.verifyUrlButton.Click += new System.EventHandler(this.VerifyUrlButton_Click_1);
            // 
            // categoriesLabel
            // 
            this.categoriesLabel.AutoSize = true;
            this.categoriesLabel.BackColor = System.Drawing.Color.Transparent;
            this.categoriesLabel.Location = new System.Drawing.Point(629, 291);
            this.categoriesLabel.Name = "categoriesLabel";
            this.categoriesLabel.Size = new System.Drawing.Size(76, 17);
            this.categoriesLabel.TabIndex = 26;
            this.categoriesLabel.Text = "Categories";
            // 
            // tddFileProcessingProgressBar
            // 
            this.tddFileProcessingProgressBar.Location = new System.Drawing.Point(711, 293);
            this.tddFileProcessingProgressBar.MarqueeAnimationSpeed = 25;
            this.tddFileProcessingProgressBar.Name = "tddFileProcessingProgressBar";
            this.tddFileProcessingProgressBar.Size = new System.Drawing.Size(277, 15);
            this.tddFileProcessingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.tddFileProcessingProgressBar.TabIndex = 27;
            this.tddFileProcessingProgressBar.Visible = false;
            // 
            // loadIntoRepositoryButton
            // 
            this.loadIntoRepositoryButton.Enabled = false;
            this.loadIntoRepositoryButton.Location = new System.Drawing.Point(632, 576);
            this.loadIntoRepositoryButton.Name = "loadIntoRepositoryButton";
            this.loadIntoRepositoryButton.Size = new System.Drawing.Size(508, 59);
            this.loadIntoRepositoryButton.TabIndex = 13;
            this.loadIntoRepositoryButton.Text = "Load categories into repository";
            this.loadIntoRepositoryButton.UseVisualStyleBackColor = true;
            this.loadIntoRepositoryButton.Click += new System.EventHandler(this.LoadIntoRepositoryButton_Click);
            // 
            // migrationProgressBar
            // 
            this.migrationProgressBar.Location = new System.Drawing.Point(751, 641);
            this.migrationProgressBar.MarqueeAnimationSpeed = 25;
            this.migrationProgressBar.Name = "migrationProgressBar";
            this.migrationProgressBar.Size = new System.Drawing.Size(273, 15);
            this.migrationProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.migrationProgressBar.TabIndex = 28;
            this.migrationProgressBar.Visible = false;
            // 
            // TdsMigrator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1192, 678);
            this.Controls.Add(this.migrationProgressBar);
            this.Controls.Add(this.tddFileProcessingProgressBar);
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
            this.Controls.Add(this.categoriesListBox);
            this.Controls.Add(this.clearRepositoryButton);
            this.Controls.Add(this.createRepositoryButton);
            this.Controls.Add(this.repositoryNameText);
            this.Controls.Add(this.repositoryNameTextBox);
            this.Controls.Add(this.loadIntoRepositoryButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TDDPathTextBox);
            this.Controls.Add(this.pickFileButton);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TdsMigrator";
            this.Text = "Tricentis TDM to TDS Migrator";
            this.Load += new System.EventHandler(this.TdsMigrator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pickFileButton;
        private System.Windows.Forms.TextBox TDDPathTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox repositoryNameTextBox;
        private System.Windows.Forms.Label repositoryNameText;
        private System.Windows.Forms.Button createRepositoryButton;
        private System.Windows.Forms.Button clearRepositoryButton;
        private System.Windows.Forms.CheckedListBox categoriesListBox;
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
        private System.Windows.Forms.ProgressBar tddFileProcessingProgressBar;
        private System.Windows.Forms.Button loadIntoRepositoryButton;
        private System.Windows.Forms.ProgressBar migrationProgressBar;
    }
}


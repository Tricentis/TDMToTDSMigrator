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
            this.createRepositoryButton = new System.Windows.Forms.Button();
            this.clearRepositoryButton = new System.Windows.Forms.Button();
            this.categoriesListBox = new System.Windows.Forms.CheckedListBox();
            this.deselectAllButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.apiURL = new System.Windows.Forms.Label();
            this.apiUrlTextBox = new System.Windows.Forms.TextBox();
            this.deleteRepositoryButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.loadRefreshRepositoriesButton = new System.Windows.Forms.Button();
            this.repositoriesBox = new System.Windows.Forms.ListBox();
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.verifyUrlButton = new System.Windows.Forms.Button();
            this.categoriesLabel = new System.Windows.Forms.Label();
            this.tddFileProcessingProgressBar = new System.Windows.Forms.ProgressBar();
            this.loadIntoRepositoryButton = new System.Windows.Forms.Button();
            this.migrationProgressBar = new System.Windows.Forms.ProgressBar();
            this.reverseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pickFileButton
            // 
            this.pickFileButton.BackColor = System.Drawing.Color.Gainsboro;
            this.pickFileButton.Enabled = false;
            this.pickFileButton.FlatAppearance.BorderSize = 0;
            this.pickFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pickFileButton.Location = new System.Drawing.Point(396, 69);
            this.pickFileButton.Name = "pickFileButton";
            this.pickFileButton.Size = new System.Drawing.Size(130, 28);
            this.pickFileButton.TabIndex = 4;
            this.pickFileButton.Text = "...";
            this.pickFileButton.UseVisualStyleBackColor = false;
            this.pickFileButton.Click += new System.EventHandler(this.PickFileButton_Click);
            // 
            // TDDPathTextBox
            // 
            this.TDDPathTextBox.Enabled = false;
            this.TDDPathTextBox.Location = new System.Drawing.Point(122, 69);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(45, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = ".tdd File ";
            // 
            // createRepositoryButton
            // 
            this.createRepositoryButton.BackColor = System.Drawing.Color.Gainsboro;
            this.createRepositoryButton.Enabled = false;
            this.createRepositoryButton.FlatAppearance.BorderSize = 0;
            this.createRepositoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createRepositoryButton.Location = new System.Drawing.Point(972, 60);
            this.createRepositoryButton.Name = "createRepositoryButton";
            this.createRepositoryButton.Size = new System.Drawing.Size(130, 43);
            this.createRepositoryButton.TabIndex = 5;
            this.createRepositoryButton.Text = "New repository";
            this.createRepositoryButton.UseVisualStyleBackColor = false;
            this.createRepositoryButton.Click += new System.EventHandler(this.CreateRepositoryButton_Click);
            // 
            // clearRepositoryButton
            // 
            this.clearRepositoryButton.BackColor = System.Drawing.Color.Gainsboro;
            this.clearRepositoryButton.Enabled = false;
            this.clearRepositoryButton.FlatAppearance.BorderSize = 0;
            this.clearRepositoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearRepositoryButton.Location = new System.Drawing.Point(972, 110);
            this.clearRepositoryButton.Name = "clearRepositoryButton";
            this.clearRepositoryButton.Size = new System.Drawing.Size(130, 43);
            this.clearRepositoryButton.TabIndex = 7;
            this.clearRepositoryButton.Text = "Clear repository";
            this.clearRepositoryButton.UseVisualStyleBackColor = false;
            this.clearRepositoryButton.Click += new System.EventHandler(this.ClearRepositoryButton_Click);
            // 
            // categoriesListBox
            // 
            this.categoriesListBox.CheckOnClick = true;
            this.categoriesListBox.ColumnWidth = 111;
            this.categoriesListBox.Enabled = false;
            this.categoriesListBox.FormattingEnabled = true;
            this.categoriesListBox.Location = new System.Drawing.Point(590, 294);
            this.categoriesListBox.Name = "categoriesListBox";
            this.categoriesListBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.categoriesListBox.Size = new System.Drawing.Size(356, 276);
            this.categoriesListBox.Sorted = true;
            this.categoriesListBox.TabIndex = 15;
            this.categoriesListBox.ThreeDCheckBoxes = true;
            this.categoriesListBox.SelectedIndexChanged += new System.EventHandler(this.CategoriesListBox_SelectedIndexChanged);
            this.categoriesListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.CategoriesListBox_Format);
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.BackColor = System.Drawing.Color.Gainsboro;
            this.deselectAllButton.Enabled = false;
            this.deselectAllButton.FlatAppearance.BorderSize = 0;
            this.deselectAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deselectAllButton.Location = new System.Drawing.Point(972, 333);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(130, 33);
            this.deselectAllButton.TabIndex = 11;
            this.deselectAllButton.Text = "Deselect All";
            this.deselectAllButton.UseVisualStyleBackColor = false;
            this.deselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.BackColor = System.Drawing.Color.Gainsboro;
            this.selectAllButton.Enabled = false;
            this.selectAllButton.FlatAppearance.BorderSize = 0;
            this.selectAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectAllButton.Location = new System.Drawing.Point(972, 294);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(130, 33);
            this.selectAllButton.TabIndex = 10;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = false;
            this.selectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // apiURL
            // 
            this.apiURL.AutoSize = true;
            this.apiURL.BackColor = System.Drawing.Color.Transparent;
            this.apiURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apiURL.Location = new System.Drawing.Point(44, 35);
            this.apiURL.Name = "apiURL";
            this.apiURL.Size = new System.Drawing.Size(64, 18);
            this.apiURL.TabIndex = 13;
            this.apiURL.Text = "API URL";
            // 
            // apiUrlTextBox
            // 
            this.apiUrlTextBox.BackColor = System.Drawing.Color.White;
            this.apiUrlTextBox.Location = new System.Drawing.Point(122, 34);
            this.apiUrlTextBox.Name = "apiUrlTextBox";
            this.apiUrlTextBox.Size = new System.Drawing.Size(248, 22);
            this.apiUrlTextBox.TabIndex = 1;
            this.apiUrlTextBox.Text = "http://localhost:81/testdataservice";
            // 
            // deleteRepositoryButton
            // 
            this.deleteRepositoryButton.BackColor = System.Drawing.Color.Gainsboro;
            this.deleteRepositoryButton.Enabled = false;
            this.deleteRepositoryButton.FlatAppearance.BorderSize = 0;
            this.deleteRepositoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteRepositoryButton.Location = new System.Drawing.Point(972, 159);
            this.deleteRepositoryButton.Name = "deleteRepositoryButton";
            this.deleteRepositoryButton.Size = new System.Drawing.Size(130, 43);
            this.deleteRepositoryButton.TabIndex = 8;
            this.deleteRepositoryButton.Text = "Delete repository";
            this.deleteRepositoryButton.UseVisualStyleBackColor = false;
            this.deleteRepositoryButton.Click += new System.EventHandler(this.DeleteRepositoryButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(586, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 18);
            this.label3.TabIndex = 19;
            this.label3.Text = "Repositories";
            // 
            // loadRefreshRepositoriesButton
            // 
            this.loadRefreshRepositoriesButton.BackColor = System.Drawing.Color.Gainsboro;
            this.loadRefreshRepositoriesButton.Enabled = false;
            this.loadRefreshRepositoriesButton.FlatAppearance.BorderSize = 0;
            this.loadRefreshRepositoriesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadRefreshRepositoriesButton.Location = new System.Drawing.Point(972, 208);
            this.loadRefreshRepositoriesButton.Name = "loadRefreshRepositoriesButton";
            this.loadRefreshRepositoriesButton.Size = new System.Drawing.Size(130, 32);
            this.loadRefreshRepositoriesButton.TabIndex = 9;
            this.loadRefreshRepositoriesButton.Text = "Refresh";
            this.loadRefreshRepositoriesButton.UseVisualStyleBackColor = false;
            this.loadRefreshRepositoriesButton.Click += new System.EventHandler(this.LoadRefreshRepositoriesButton_Click);
            // 
            // repositoriesBox
            // 
            this.repositoriesBox.DisplayMember = "Name";
            this.repositoriesBox.Enabled = false;
            this.repositoriesBox.FormattingEnabled = true;
            this.repositoriesBox.ItemHeight = 16;
            this.repositoriesBox.Location = new System.Drawing.Point(590, 60);
            this.repositoriesBox.Name = "repositoriesBox";
            this.repositoriesBox.Size = new System.Drawing.Size(356, 180);
            this.repositoriesBox.Sorted = true;
            this.repositoriesBox.TabIndex = 16;
            this.repositoriesBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.RepositoriesBox_Format);
            this.repositoriesBox.SelectedValueChanged += new System.EventHandler(this.RepositoriesBox_SelectedValueChanged);
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.logTextBox.Location = new System.Drawing.Point(47, 110);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(479, 535);
            this.logTextBox.TabIndex = 14;
            this.logTextBox.Text = "";
            this.logTextBox.TextChanged += new System.EventHandler(this.LogTextBox_TextChanged);
            // 
            // verifyUrlButton
            // 
            this.verifyUrlButton.BackColor = System.Drawing.Color.Gainsboro;
            this.verifyUrlButton.FlatAppearance.BorderSize = 0;
            this.verifyUrlButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.verifyUrlButton.Location = new System.Drawing.Point(396, 31);
            this.verifyUrlButton.Name = "verifyUrlButton";
            this.verifyUrlButton.Size = new System.Drawing.Size(130, 28);
            this.verifyUrlButton.TabIndex = 2;
            this.verifyUrlButton.Text = "Verify URL";
            this.verifyUrlButton.UseVisualStyleBackColor = false;
            this.verifyUrlButton.Click += new System.EventHandler(this.VerifyUrlButton_Click);
            // 
            // categoriesLabel
            // 
            this.categoriesLabel.AutoSize = true;
            this.categoriesLabel.BackColor = System.Drawing.Color.Transparent;
            this.categoriesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.categoriesLabel.Location = new System.Drawing.Point(586, 271);
            this.categoriesLabel.Name = "categoriesLabel";
            this.categoriesLabel.Size = new System.Drawing.Size(80, 18);
            this.categoriesLabel.TabIndex = 26;
            this.categoriesLabel.Text = "Categories";
            // 
            // tddFileProcessingProgressBar
            // 
            this.tddFileProcessingProgressBar.Location = new System.Drawing.Point(693, 274);
            this.tddFileProcessingProgressBar.MarqueeAnimationSpeed = 25;
            this.tddFileProcessingProgressBar.Name = "tddFileProcessingProgressBar";
            this.tddFileProcessingProgressBar.Size = new System.Drawing.Size(253, 17);
            this.tddFileProcessingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.tddFileProcessingProgressBar.TabIndex = 27;
            this.tddFileProcessingProgressBar.Visible = false;
            // 
            // loadIntoRepositoryButton
            // 
            this.loadIntoRepositoryButton.BackColor = System.Drawing.Color.Gainsboro;
            this.loadIntoRepositoryButton.Enabled = false;
            this.loadIntoRepositoryButton.FlatAppearance.BorderSize = 0;
            this.loadIntoRepositoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadIntoRepositoryButton.Location = new System.Drawing.Point(590, 586);
            this.loadIntoRepositoryButton.Name = "loadIntoRepositoryButton";
            this.loadIntoRepositoryButton.Size = new System.Drawing.Size(508, 59);
            this.loadIntoRepositoryButton.TabIndex = 12;
            this.loadIntoRepositoryButton.Text = "Load categories into repository";
            this.loadIntoRepositoryButton.UseVisualStyleBackColor = false;
            this.loadIntoRepositoryButton.Click += new System.EventHandler(this.LoadIntoRepositoryButton_Click);
            // 
            // migrationProgressBar
            // 
            this.migrationProgressBar.Location = new System.Drawing.Point(590, 564);
            this.migrationProgressBar.MarqueeAnimationSpeed = 25;
            this.migrationProgressBar.Name = "migrationProgressBar";
            this.migrationProgressBar.Size = new System.Drawing.Size(356, 16);
            this.migrationProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.migrationProgressBar.TabIndex = 28;
            this.migrationProgressBar.Visible = false;
            // 
            // reverseButton
            // 
            this.reverseButton.BackColor = System.Drawing.Color.Gainsboro;
            this.reverseButton.Enabled = false;
            this.reverseButton.FlatAppearance.BorderSize = 0;
            this.reverseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reverseButton.Location = new System.Drawing.Point(972, 381);
            this.reverseButton.Name = "reverseButton";
            this.reverseButton.Size = new System.Drawing.Size(130, 33);
            this.reverseButton.TabIndex = 29;
            this.reverseButton.Text = "Reverse";
            this.reverseButton.UseVisualStyleBackColor = false;
            this.reverseButton.Click += new System.EventHandler(this.ReverseButton_Click);
            // 
            // TdsMigrator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuBar;
            this.BackgroundImage = global::MigratorUI.Properties.Resources.fonmdappd;
            this.ClientSize = new System.Drawing.Size(1141, 683);
            this.Controls.Add(this.reverseButton);
            this.Controls.Add(this.migrationProgressBar);
            this.Controls.Add(this.tddFileProcessingProgressBar);
            this.Controls.Add(this.categoriesLabel);
            this.Controls.Add(this.verifyUrlButton);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.repositoriesBox);
            this.Controls.Add(this.loadRefreshRepositoriesButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.deleteRepositoryButton);
            this.Controls.Add(this.apiUrlTextBox);
            this.Controls.Add(this.apiURL);
            this.Controls.Add(this.selectAllButton);
            this.Controls.Add(this.deselectAllButton);
            this.Controls.Add(this.categoriesListBox);
            this.Controls.Add(this.clearRepositoryButton);
            this.Controls.Add(this.createRepositoryButton);
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
        private System.Windows.Forms.Button createRepositoryButton;
        private System.Windows.Forms.Button clearRepositoryButton;
        private System.Windows.Forms.CheckedListBox categoriesListBox;
        private System.Windows.Forms.Button deselectAllButton;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Label apiURL;
        private System.Windows.Forms.TextBox apiUrlTextBox;
        private System.Windows.Forms.Button deleteRepositoryButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button loadRefreshRepositoriesButton;
        private System.Windows.Forms.Button verifyUrlButton;
        private System.Windows.Forms.Label categoriesLabel;
        private System.Windows.Forms.ProgressBar tddFileProcessingProgressBar;
        private System.Windows.Forms.Button loadIntoRepositoryButton;
        private System.Windows.Forms.ProgressBar migrationProgressBar;
        public System.Windows.Forms.ListBox repositoriesBox;
        public System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.Button reverseButton;
    }
}


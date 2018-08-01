﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TDMtoTDSMigrator;

namespace MigratorUI
{
    public partial class TdsMigrator : Form
    {
        public TdsMigrator()
        {
            InitializeComponent();
        }


        private List<TableObject> _dataList; // List of TableObjects each corresponding to one row of data 
        private string _xmlPath; 
        private Boolean _migrationInWork=false;

        //Initialization 
        private void TDSMigrator_Load(object sender, EventArgs e)
        {
            logTextBox.AppendText("Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
            verifyUrlButton.Select();
        }


        //Migration and tdd processing methods
        private async Task<HttpResponseMessage> LaunchMigration(List<string> authorizedTypes, Boolean applyFilter, string xmlPath, List<TableObject> objectList, string selectedRepository, string apiUrl)

        {



            if (applyFilter)

            {

                HttpResponseMessage message = await TdsLoader.MigrateXmlDataIntoTdsWithFilter(xmlPath, objectList, selectedRepository, authorizedTypes, apiUrl);
                return message;
            }

            else

            {

                HttpResponseMessage message = await TdsLoader.MigrateXmlDataIntoTdsWithoutFilter(xmlPath, objectList, selectedRepository, apiUrl);
                return message;
            }

           

        }
        private void LoadCategoriesIntoListBox()
        {

            XmlNode metaInfoType = XmlParser.GetMetaInfoTypes(_xmlPath);
            for (int i = 0; i < metaInfoType.ChildNodes.Count; i++)
            {
                categoriesListBox.Items.Add(metaInfoType.ChildNodes[i].Attributes?[1].Value ?? throw new InvalidOperationException(), true);
            }
        }


        //Verification methods
        private string ValidateUrl(string url)
        { 
            if (url.Length > 0 && url[url.Length - 1] != '/')
            {
                url = url + "/";
            }
            return url;
        }
        private void CheckForAssociations()
        {

            XmlNode metaInfoAssoc = XmlParser.GetMetaInfoAssociations(_xmlPath);
            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(_xmlPath);
            TableObject obj = new TableObject();
            StringBuilder s = new StringBuilder();

            if (metaInfoAssoc.HasChildNodes)
            {
                
                s.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (XmlNode node in metaInfoAssoc.ChildNodes)
                {
                    
                    s.Append(node.Attributes?[1].Value + " and " + obj.FindCategoryName(node.Attributes?[2].Value,metaInfoTypes) + "\n");
                }
                System.Windows.Forms.MessageBox.Show(s.ToString(),
                                    "Associations not supported",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CheckForEmptyCategories()
        {

            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(_xmlPath);

            List<string> emptyType = new List<string>();
            var items = categoriesListBox.Items;
            for (int i = 0; i < metaInfoTypes.ChildNodes.Count; i++)
            {
                emptyType.Add(metaInfoTypes.ChildNodes[i].Attributes?[1].Value);
            }

            for (int i = 0; i < _dataList.Count; i++)
            {
                for (int j = 0; j < emptyType.Count; j++)
                {
                    if (_dataList[i].GetCategoryName() == emptyType[j])
                    {
                        emptyType.RemoveAt(j);
                    }
                }
            }

            foreach (string s in emptyType)
            {
                for (int i = 0; i < categoriesListBox.Items.Count; i++)
                {
                    if (categoriesListBox.Items[i].ToString() == s)
                    {
                        categoriesListBox.Items[i] = categoriesListBox.Items[i].ToString() + " (Empty)";
                    }
                }
            }
        }


        //UI element attributes and logText methods 
        private void TddFileProcessingInWork(Boolean processingInWork)
        {

            GenerateButton.Enabled = !processingInWork;
            categoriesListBox.Enabled = !processingInWork;
            selectAllButton.Enabled = !processingInWork;
            deselectAllButton.Enabled = !processingInWork;
            verifyUrlButton.Enabled = !processingInWork;
            pickFileButton.Enabled = !processingInWork;
            tddFileProcessingProgressBar.Visible = processingInWork;
        }
        private void MigrationInWork(Boolean migrationInWork)
        {
            verifyUrlButton.Enabled = !migrationInWork;
            pickFileButton.Enabled = !migrationInWork;
            migrationProgressBar.Visible = migrationInWork;
            deleteRepositoryButton.Enabled = !migrationInWork;
            clearRepositoryButton.Enabled = !migrationInWork;
            GenerateButton.Enabled = !migrationInWork;
            loadRefreshRepositories.Enabled = !migrationInWork;
        }
        private void ApiConnectionOk(Boolean apiConnectionOk, object sender, EventArgs e)
        {
            Boolean tddFilePicked = TDDPathTextBox.Text != "";

            createRepositoryButton.Enabled = apiConnectionOk;
            deleteRepositoryButton.Enabled = apiConnectionOk;
            clearRepositoryButton.Enabled = apiConnectionOk;
            GenerateButton.Enabled = apiConnectionOk & tddFilePicked & !_migrationInWork;
            loadRefreshRepositories.Enabled = apiConnectionOk;
            repositoriesBox.Enabled = apiConnectionOk;
            repositoryDescriptionTextbox.Enabled = apiConnectionOk;
            repositoryNameTextBox.Enabled = apiConnectionOk;
            categoriesListBox.Enabled = apiConnectionOk & tddFilePicked & !_migrationInWork;
            selectAllButton.Enabled = apiConnectionOk & tddFilePicked & !_migrationInWork;
            deselectAllButton.Enabled = apiConnectionOk & tddFilePicked & !_migrationInWork;
            apiUrlTextBox.Enabled = !apiConnectionOk & !_migrationInWork;
            pickFileButton.Enabled = apiConnectionOk & !_migrationInWork;

            if (apiConnectionOk)
            {
                pickFileButton.Text = "Browse...";
                loadRefreshRepositories_Click(sender, e);
                pickFileButton.Select();
            }
            else
            {
                pickFileButton.Text = "...";
            }

            


        }
        private int EstimatedWaitTime()
        {            
            FileInfo tdd = new FileInfo(TDDPathTextBox.Text);
            int scaleLength = 611797; // length of the tdd file that is the scale for processing time estimation (~45 seconds for this file)
            return (int)((float)tdd.Length / (float)scaleLength * 45);
        }
        private void RefreshRepositoriesList()
        {
            repositoriesBox.Items.Clear();
            string reposJson = HttpRequest.GetRepositories(ValidateUrl(apiUrlTextBox.Text));
            string[] repoList = JsonConverter.ParseJsonIntoRepositoryList(reposJson);

            for (int i = 0; i < repoList.Length; i++)
            {
                repositoriesBox.Items.Add(repoList[i]);
            }
            if (repoList.Length > 0)
            {
                repositoriesBox.SelectedItem = repositoriesBox.Items[0];
            }
        }


        //logText message generation methoids
        private string CreatedRepositoryMessage(string name, string description)
        {
            StringBuilder s = new StringBuilder();
            s.Append("Repository Created : " + name);
            if (description != "")
            {
                s.Append(" , Description : " + description);
            }

            s.Append("\n");
            return  s.ToString();
        }
        private string DeletedRepositoryMessage(string name)
        {
            return "Repository Deleted : " + name+"\n";
        }
        private string ClearedRepositoryMessage(string name)
        {
            return "Repository Cleared : " + name+"\n";
        }
        private string MigrationFinishedMessage(int numberOfCategories, string repositoryName)
        {
            return "Successfully migrated "+numberOfCategories+" out of "+categoriesListBox.Items.Count+" available categories into the repository : \""+repositoryName+"\".\n";
        }


        //Event methods
        private async void loadIntoRepositoryButton_Click(object sender, EventArgs e)
{   
            Boolean applyFilter = false;
            Boolean atLeastOneCategorySelected = false;
            XmlNode metaInfoTypes = XmlParser.GetMetaInfoTypes(_xmlPath);
            List<string> filteredCategories = new List<string>();

            for (int i = 0; i < categoriesListBox.Items.Count; i++)
            {
                if (categoriesListBox.GetItemChecked(i))
                {
                     atLeastOneCategorySelected = true;
                     filteredCategories.Add(categoriesListBox.Items[i].ToString());
                }
                else
                {
                    applyFilter = true;
                }
            }
            if (repositoriesBox.SelectedItem == null)
            {
                logTextBox.AppendText("Please pick a repository, or create one\n");

            }
            else if (!atLeastOneCategorySelected)
                {
                logTextBox.AppendText("Please pick a category\n");
            }
            else
            {
                logTextBox.AppendText("Migrating " + filteredCategories.Count + " categories into \"" + repositoriesBox.SelectedItem + "\". Please wait...\n");

                MigrationInWork(true);
                await Task.Delay(10);

                await LaunchMigration(filteredCategories, applyFilter, _xmlPath, _dataList, repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));

                logTextBox.AppendText(MigrationFinishedMessage(filteredCategories.Count, repositoriesBox.SelectedItem.ToString()));
                logTextBox.Refresh();

                MigrationInWork(false);
            }

        }
        private async void TDDPathTextBox_TextChanged(object sender, EventArgs e)
        {
            int estimatedWait = EstimatedWaitTime();
            logTextBox.Select();

            logTextBox.AppendText("The .tdd file is being processed.\nPlease wait...\n");
            if (estimatedWait > 5)
            {
                logTextBox.AppendText("Estimated waiting time : " + estimatedWait + " seconds \n");
            }
            
            logTextBox.Refresh();
            TddFileProcessingInWork(true);

            await Task.Delay(10);          

            this._xmlPath = TdsLoader.DecompressTddFileIntoXml(new FileInfo(TDDPathTextBox.Text));

            LoadCategoriesIntoListBox();
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => {
                r.Result = this._dataList = XmlParser.CreateDataList(_xmlPath);
            };
            worker.RunWorkerCompleted += (s, r) => {
                this._dataList =(List<TableObject>)r.Result;

                logTextBox.AppendText("\nThe .tdd file was successfully processed. \n" + categoriesListBox.Items.Count + " categories were found. \n\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n\n");
                logTextBox.Refresh();

                CheckForEmptyCategories();    
                TddFileProcessingInWork(false);
                tddFileProcessingProgressBar.Visible = false;
                CheckForAssociations();
            };
            worker.RunWorkerAsync();


        }
        private void pickFileButton_Click(object sender, EventArgs e)
        {

            openFileDialog.ShowDialog();

        }
        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            categoriesListBox.Items.Clear();
            TDDPathTextBox.Text = openFileDialog.FileName;
            
        }
        private void createRepositoryButton_Click(object sender, EventArgs e)
        {
            if (repositoryNameTextBox.Text != "")
            {
                RefreshRepositoriesList();

                if (!repositoriesBox.Items.Contains(repositoryNameTextBox.Text))
                {  
                HttpRequest.CreateRepository(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text, ValidateUrl(apiUrlTextBox.Text));
                logTextBox.AppendText(CreatedRepositoryMessage(repositoryNameTextBox.Text, repositoryDescriptionTextbox.Text));
                repositoriesBox.Items.Add(repositoryNameTextBox.Text);
                repositoriesBox.SelectedItem = repositoryNameTextBox.Text;
                }
                else
                {
                    logTextBox.AppendText("Repository \"" + repositoryNameTextBox.Text + "\" already exists \n");
                }
            }
            else
            {
                logTextBox.AppendText("Please enter a repository name\n");
            }

            
        }
        private void clearRepositoryButton_Click(object sender, EventArgs e)
        {
            if (repositoriesBox.SelectedItem != null)
            {
                var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                     "Clear " + repositoriesBox.SelectedItem.ToString() + " repository",
                                     MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.OK)
                {
                    HttpRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(ClearedRepositoryMessage(repositoriesBox.SelectedItem.ToString()));
                }
                
            }
            else
            {
                logTextBox.AppendText("Please select a repository to clear \n");

            }
        }
        private void deleteRepositoryButton_Click(object sender, EventArgs e) 
        {
            
            if (repositoriesBox.SelectedItem != null)
            {
                var confirmResult = MessageBox.Show("All the data contained in this repository will be erased",
                                     "Clear " + repositoriesBox.SelectedItem.ToString() + " repository",
                                     MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.OK)
                {

                    HttpRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    HttpRequest.DeleteRepository(repositoriesBox.SelectedItem.ToString(), ValidateUrl(apiUrlTextBox.Text));
                    logTextBox.AppendText(DeletedRepositoryMessage(repositoriesBox.SelectedItem.ToString()));
                    repositoriesBox.Items.Remove(repositoriesBox.SelectedItem);

                    if (repositoriesBox.Items.Count > 0)
                    {
                        repositoriesBox.SelectedItem = repositoriesBox.Items[0];
                    }
                }

                
            }
            else
            {
                logTextBox.AppendText("Please select a repository to delete \n");
            }




        }
        private void selectAllButton_Click(object sender, EventArgs e)
        {
            for(int  i = 0; i<categoriesListBox.Items.Count; i++)
            {
                categoriesListBox.SetItemChecked(i, true);
            }
            
        }
        private void deselectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < categoriesListBox.Items.Count; i++)
            {
                categoriesListBox.SetItemChecked(i, false);
            }
        }
        private void loadRefreshRepositories_Click(object sender, EventArgs e)
        {
            RefreshRepositoriesList();
        }
        private void verifyUrlButton_Click_1(object sender, EventArgs e)
        {
            if(verifyUrlButton.Text == "Change URL")
            {
                verifyUrlButton.Text = "Verify URL";
                ApiConnectionOk(false,sender,e);

            }
            else
            {

                if (HttpRequest.SetAndVerifyConnection(ValidateUrl(apiUrlTextBox.Text)))
                {
                    apiUrlTextBox.BackColor = Color.Lime;
                    ApiConnectionOk(true, sender, e);
                    verifyUrlButton.Text = "Change URL";
                    logTextBox.AppendText("Valid URL. \n");
                    if (TDDPathTextBox.Text == "")
                    {
                        logTextBox.AppendText("Please pick a.tdd file in your filesystem.\n");
                    }
                    logTextBox.Refresh();
                    


                }
                else
                {
                    apiUrlTextBox.BackColor = Color.PaleVioletRed;
                    ApiConnectionOk(false, sender, e);
                    logTextBox.AppendText("Not a valid URL.\n");
                    logTextBox.Refresh();
                }
            }
           


        }
        private void logTextBox_TextChanged(object sender, EventArgs e)
        {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }
    }


}


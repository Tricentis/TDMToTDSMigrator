using System;
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
    public partial class TDSMigrator : Form
    {
        public TDSMigrator()
        {
            InitializeComponent();
        }


        private List<TableObject> objectList; // List of TableObjects each corresponding to one row of data 
        private string xmlPath; // path of the decompressed xml file
        private Boolean migrationInWork=false; 

        //Initialization 
        private void TDSMigrator_Load(object sender, EventArgs e)
        {
            logTextBox.AppendText("Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL and click on \"Verify Url\". \nExample : http://localhost:80/testdataservice \n");
            verifyUrlButton.Select();
            //verifyUrlButton.BackColor = Color.Aquamarine;
        }


        //Migration and tdd processing methods
        private async Task<HttpResponseMessage> Migrate(List<string> authorizedTypes, Boolean applyFilter, string xmlPath, List<TableObject> objectList, string selectedRepository, string apiUrl)
        {//Loads the selected categories into the selected repository in TDS

            HttpResponseMessage message = null;
            if (applyFilter)
            {
                message = await TDSLoader.LoadIntoTDSWithFilter(xmlPath, objectList, selectedRepository, authorizedTypes, apiUrl);
            }
            else
            {
                message = await TDSLoader.LoadIntoTDS(xmlPath, objectList, selectedRepository, apiUrl);
            }
            return message;
        }
        private void LoadCategories(XmlNode metaInfoType)
        {//Loads categories into categories box
            var items = categoriesListBox.Items;
            for (int i = 0; i < metaInfoType.ChildNodes.Count; i++)
            {
                items.Add(metaInfoType.ChildNodes[i].Attributes[1].Value, true);
            }
        }


        //Verification methods
        private string ValidateURI(string uri)
        {//checks for the last slash ("/") in the uri entered by the user. if not present, adds it.
            if (uri.Length > 0 && uri[uri.Length - 1] != '/')
            {
                uri = uri + "/";
            }
            return uri;
        }
        private void CheckForAssociations()
        {//checks if there were any associations in the TDM. If yes, warns the user that they will no longer be supported
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            XmlNode metaInfoAssoc = XMLParser.GetParentNodeOfData(doc).ChildNodes[3];
            XmlNode metaInfoTypes = XMLParser.GetMetaInfoTypes(XMLParser.GetParentNodeOfData(doc));
            TableObject obj = new TableObject();
            StringBuilder s = new StringBuilder();

            if (metaInfoAssoc.HasChildNodes)
            {
                
                s.Append("Please note that the following associations will no longer be supported by Tricentis TDS :\n\n");
                foreach (XmlNode node in metaInfoAssoc.ChildNodes)
                {
                    
                    s.Append(node.Attributes[1].Value + " and " + obj.FindTypeName(node.Attributes[2].Value,metaInfoTypes) + "\n");
                }
                System.Windows.Forms.MessageBox.Show(s.ToString(),
                                    "Associations not supported",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CheckForEmptyCategories(XmlNode metaInfoTypes)
        {
            //Checks for categories that have no object associated with them in the objectList
            //Adds the info "Empty" next to the category name in the categories box
            List<string> emptyType = new List<string>();
            var items = categoriesListBox.Items;
            for (int i = 0; i < metaInfoTypes.ChildNodes.Count; i++)
            {
                emptyType.Add(metaInfoTypes.ChildNodes[i].Attributes[1].Value);
            }

            for (int i = 0; i < objectList.Count; i++)
            {
                for (int j = 0; j < emptyType.Count; j++)
                {
                    if (objectList[i].GetTypeName() == emptyType[j])
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
        private void TDDFileProcessingInWork(Boolean processing)
        {//sets the enabled state of ui elements depending on whether a .tdd file is being processed

            GenerateButton.Enabled = !processing;
            categoriesListBox.Enabled = !processing;
            selectAllButton.Enabled = !processing;
            deselectAllButton.Enabled = !processing;
            verifyUrlButton.Enabled = !processing;
            pickFileButton.Enabled = !processing;
        }
        private void MigrationInWork(Boolean inWork)
        {// sets the enabled state of ui elements depending on whether files are currently being transferred to the api
            if (inWork)
            {
                progressBar2.Visible = true;
            }
            else
            {
                progressBar2.Visible = false;
            }
            deleteRepositoryButton.Enabled = !inWork;
            clearRepositoryButton.Enabled = !inWork;
            GenerateButton.Enabled = !inWork;
            loadRefreshRepositories.Enabled = !inWork;
        }
        private void ApiConnectionOk(Boolean apiConnectionOK, object sender, EventArgs e)
        {//sets the enabled state of ui elements depending on the success of api url verification
            Boolean tddFilePicked = TDDPathTextBox.Text != "";

            createRepositoryButton.Enabled = apiConnectionOK;
            deleteRepositoryButton.Enabled = apiConnectionOK;
            clearRepositoryButton.Enabled = apiConnectionOK;
            GenerateButton.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            loadRefreshRepositories.Enabled = apiConnectionOK;
            repositoriesBox.Enabled = apiConnectionOK;
            repositoryDescriptionTextbox.Enabled = apiConnectionOK;
            repositoryName.Enabled = apiConnectionOK;
            categoriesListBox.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            selectAllButton.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            deselectAllButton.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            apiUrlTextBox.Enabled = !apiConnectionOK & !migrationInWork;
            pickFileButton.Enabled = apiConnectionOK & !migrationInWork;

            if (apiConnectionOK)
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
            // returns the waiting time for processing the tdd after the file was picked
            FileInfo tdd = new FileInfo(TDDPathTextBox.Text);

            int scaleLength = 611797; // length of the tdd file that is the scale for processing time estimation (~35 seconds)
            return (int)((float)tdd.Length / (float)scaleLength * 35);
        }


        //logText message generation methoids
        private string CreatedRepositoryMessage(string name)
        {
            return "Repository Created : " + name+"\n";
        }
        private string CreatedRepositoryMessage(string name, string description)
        {
            return "Repository Created : " + name +" , Description : "+description+"\n";
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


        //Event on click and text changed methods
        private async void loadIntoRepositoryButton_Click(object sender, EventArgs e)
{   
            Boolean applyFilter = false;
            Boolean atLeastOneItemChecked = false;
            XmlNode metaInfoTypes = XMLParser.GetMetaInfoTypes(xmlPath);

            List<string> authorizedTypes = new List<string>();

            for (int i = 0; i < categoriesListBox.Items.Count; i++)
            {
                if (categoriesListBox.GetItemChecked(i))
                {
                     atLeastOneItemChecked = true;
                     authorizedTypes.Add(categoriesListBox.Items[i].ToString());
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
            else if (!atLeastOneItemChecked)
                {
                logTextBox.AppendText("Please pick a category\n");
            }
            else
            {
                logTextBox.AppendText("Migrating " + authorizedTypes.Count + " categories into \"" + repositoriesBox.SelectedItem.ToString() + "\". Please wait...\n");

                MigrationInWork(true);
                await Task.Delay(10);
                await Migrate(authorizedTypes, applyFilter, xmlPath, objectList, repositoriesBox.SelectedItem.ToString(), ValidateURI(apiUrlTextBox.Text));

                logTextBox.AppendText(MigrationFinishedMessage(authorizedTypes.Count, repositoriesBox.SelectedItem.ToString()));
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
            TDDFileProcessingInWork(true);

            await Task.Delay(10);

            //decompress tdd file into xml
            XmlDocument doc = new XmlDocument();
            this.xmlPath = TDSLoader.Decompress(new FileInfo(TDDPathTextBox.Text));
            doc.Load(xmlPath);



            //Loads categories into categories box
            XmlNode metaInfoTypes = XMLParser.GetMetaInfoTypes(XMLParser.GetParentNodeOfData(doc));
            LoadCategories(metaInfoTypes);
            progressBar.Visible = true;

            //Asynchronously parses XML file and retrieves the TableObject list
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => {
                //Some work...
                r.Result = this.objectList = TDSLoader.TransformXMLIntoObjectList(xmlPath);
            };
            worker.RunWorkerCompleted += (s, r) => {
                this.objectList =(List<TableObject>)r.Result;

                logTextBox.AppendText("\nThe .tdd file was successfully processed. \n" + metaInfoTypes.ChildNodes.Count + " categories were found. \n\nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n\n");
                logTextBox.Refresh();

                CheckForEmptyCategories(metaInfoTypes);    
                TDDFileProcessingInWork(false);
                progressBar.Visible = false;
                CheckForAssociations();
            };
            worker.RunWorkerAsync();
            
            
            //Timer, bugs if something is written in the logbox while it is running
            /*if (estimatedWait > 5)
            {
                int i = -1;
                logTextBox.AppendText("Seconds elapsed :   ");
                while (worker.IsBusy)
                {
                    i++;
                    logTextBox.Text = logTextBox.Text.Remove(logTextBox.Text.Length - i.ToString().Length, i.ToString().Length);
                    logTextBox.AppendText(i.ToString());
                    logTextBox.Refresh();
                    await Task.Delay(1200);

                }
            }*/
            
        

        }
        private void pickFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = false;
            openFileDialog.DefaultExt = "tdd";
            openFileDialog.ShowDialog();

        }
        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            categoriesListBox.Items.Clear();
            TDDPathTextBox.Text = openFileDialog.FileName;
            

        }
        private void createRepositoryButton_Click(object sender, EventArgs e)
        {
            if (repositoryName.Text != "")
            {//repository name is not empty
                if (!repositoriesBox.Items.Contains(repositoryName.Text))
                {//repository name is not already taken
                    if (repositoryDescriptionTextbox.Text != "")
                    {
                        HTTPRequest.CreateRepository(repositoryName.Text, repositoryDescriptionTextbox.Text, ValidateURI(apiUrlTextBox.Text));
                        logTextBox.AppendText(CreatedRepositoryMessage(repositoryName.Text, repositoryDescriptionTextbox.Text));
                    }
                    else
                    {
                        HTTPRequest.CreateRepository(repositoryName.Text, ValidateURI(apiUrlTextBox.Text));
                        logTextBox.AppendText(CreatedRepositoryMessage(repositoryName.Text));
                    }
                    repositoriesBox.Items.Add(repositoryName.Text);
                    repositoriesBox.SelectedItem = repositoryName.Text;

                    
                }
                else
                {
                    logTextBox.AppendText("Repository \"" + repositoryName.Text + "\" already exists \n");
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
                    HTTPRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateURI(apiUrlTextBox.Text));
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

                    HTTPRequest.ClearRepository(repositoriesBox.SelectedItem.ToString(), ValidateURI(apiUrlTextBox.Text));
                    HTTPRequest.DeleteRepository(repositoriesBox.SelectedItem.ToString(), ValidateURI(apiUrlTextBox.Text));
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
            repositoriesBox.Items.Clear();
            string reposJson = HTTPRequest.GetRepositories(ValidateURI(apiUrlTextBox.Text));
            string[] repoList = JSONConverter.ParseJsonIntoRepositoryList(reposJson);

            for (int i = 0; i < repoList.Length; i++)
            {
                repositoriesBox.Items.Add(repoList[i]);
            }
            if (repoList.Length > 0)
            {
                repositoriesBox.SelectedItem = repositoriesBox.Items[0];
            }
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
                if (HTTPRequest.VerifyApiURI(ValidateURI(apiUrlTextBox.Text)))
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


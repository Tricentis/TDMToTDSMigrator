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

namespace WindowsFormsApp2
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
            logTextBox.AppendText("Welcome to Tricentis TDM to TDS Migrator.\nPlease enter a valid API URL (ex: http://localhost:80/testdataservice) \n");
        }


        //Migration and tdd processing methods
        public async Task<HttpResponseMessage> Migrate(List<string> authorizedTypes, Boolean applyFilter, string xmlPath, List<TableObject> objectList, string selectedRepository, string apiUrl)
        {
            HttpResponseMessage message = null;
            if (applyFilter)
            {
                message = await TDMtoTDSMigrator.TDSLoader.LoadIntoTDSWithFilter(xmlPath, objectList, selectedRepository, authorizedTypes, apiUrl);
            }
            else
            {
                message = await TDMtoTDSMigrator.TDSLoader.LoadIntoTDS(xmlPath, objectList, selectedRepository, apiUrl);
            }
            return message;
        }
        private void LoadCategories(XmlNode metaInfoType)
        {
            var items = checkedListBox1.Items;
            for (int i = 0; i < metaInfoType.ChildNodes.Count; i++)
            {
                items.Add(metaInfoType.ChildNodes[i].Attributes[1].Value, true);
            }
        }


        //Verification methods
        public string ValidateURI(string uri)
        {//checks for the last slash in the uri entered by the user. if not present, adds it.
            if (uri.Length > 0 && uri[uri.Length - 1] != '/')
            {
                uri = uri + "/";
            }
            return uri;
        }
        public void CheckForAssociations()
        {
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
                    s.Append(node.Attributes[1].Value + " and " + obj.GetType(node.Attributes[3].Value, metaInfoTypes) + "\n");
                }
                System.Windows.Forms.MessageBox.Show(s.ToString(),
                                    "Associations not supported",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }
        public void CheckForEmptyCategories(XmlNode metaInfoTypes)
        {
            //Checks for types with no instances and adds a warning next to the category name
            List<string> emptyType = new List<string>();
            var items = checkedListBox1.Items;
            for (int i = 0; i < metaInfoTypes.ChildNodes.Count; i++)
            {
                emptyType.Add(metaInfoTypes.ChildNodes[i].Attributes[1].Value);
            }

            for (int i = 0; i < objectList.Count; i++)
            {
                for (int j = 0; j < emptyType.Count; j++)
                {
                    if (objectList[i].GetType(objectList[i].GetTypeId(), metaInfoTypes) == emptyType[j])
                    {
                        emptyType.RemoveAt(j);
                    }
                }
            }

            foreach (string s in emptyType)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.Items[i].ToString() == s)
                    {
                        checkedListBox1.Items[i] = checkedListBox1.Items[i].ToString() + " (Empty)";
                    }
                }
            }
        }


        //UI element attributes and logText methods 
        private void TDDFileProcessingInWork(Boolean processing)
        {
            GenerateButton.Enabled = !processing;
            checkedListBox1.Enabled = !processing;
            selectAllButton.Enabled = !processing;
            deselectAllButton.Enabled = !processing;
            verifyUrlButton.Enabled = !processing;
            pickFileButton.Enabled = !processing;
        }
        private void MigrationInWork(Boolean inWork)
        {
            if (inWork)
            {
                logTextBox.Height = 470;
                progressBar.Visible = true;
            }
            else
            {
                progressBar.Visible = false;
                logTextBox.Height = 533;
            }
            //createRepositoryButton.Enabled = !inWork;
            deleteRepositoryButton.Enabled = !inWork;
            clearRepositoryButton.Enabled = !inWork;
            GenerateButton.Enabled = !inWork;
            loadRefreshRepositories.Enabled = !inWork;
            //repositoriesBox.Enabled = !inWork;
            //repositoryDescriptionTextbox.Enabled = !inWork;
            //repositoryName.Enabled = !inWork;
            //checkedListBox1.Enabled = !inWork;
            //selectAllButton.Enabled = !inWork;
            //deselectAllButton.Enabled = !inWork;
            //apiUrlTextBox.Enabled = !inWork;
            //PickFileButton.Enabled = !inWork;




        }
        private void ApiConnectionOk(Boolean apiConnectionOK, object sender, EventArgs e)
        {
            Boolean tddFilePicked = TDDPathTextBox.Text != "";

            createRepositoryButton.Enabled = apiConnectionOK;
            deleteRepositoryButton.Enabled = apiConnectionOK;
            clearRepositoryButton.Enabled = apiConnectionOK;
            GenerateButton.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            loadRefreshRepositories.Enabled = apiConnectionOK;
            repositoriesBox.Enabled = apiConnectionOK;
            repositoryDescriptionTextbox.Enabled = apiConnectionOK;
            repositoryName.Enabled = apiConnectionOK;
            checkedListBox1.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            selectAllButton.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            deselectAllButton.Enabled = apiConnectionOK & tddFilePicked & !migrationInWork;
            apiUrlTextBox.Enabled = !apiConnectionOK & !migrationInWork;
            pickFileButton.Enabled = apiConnectionOK & !migrationInWork;

            if (apiConnectionOK)
            {
                pickFileButton.Text = "Browse...";
                loadRefreshRepositories_Click(sender, e);
            }
            else
            {
                pickFileButton.Text = "...";
            }




        }
        public int EstimatedWaitTime()
        {
            FileInfo tdd = new FileInfo(TDDPathTextBox.Text);
            int scaleLength = 611797; // length of the tdd file that is the scale for processing time estimation (~17 minutes)
            return (int)((float)tdd.Length / (float)scaleLength * 25);


        }


        //logText message generation methoids
        public string CreatedRepositoryMessage(string name)
        {
            return "Repository Created : " + name+"\n";
        }
        public string CreatedRepositoryMessage(string name, string description)
        {
            return "Repository Created : " + name +" , Description : "+description+"\n";
        }
        public string DeletedRepositoryMessage(string name)
        {
            return "Repository Deleted : " + name+"\n";
        }
        public string ClearedRepositoryMessage(string name)
        {
            return "Repository Cleared : " + name+"\n";
        }
        public string MigrationFinishedMessage(int numberOfCategories, string repositoryName)
        {
            return "Successfully migrated "+numberOfCategories+" categories into the repository : \""+repositoryName+"\".\n";
        }


        //Event on click methods
        private async void loadIntoRepositoryButton_Click(object sender, EventArgs e)
{   
            Boolean applyFilter = false;
            Boolean atLeastOneItemChecked = false;
            XmlNode metaInfoTypes = XMLParser.GetMetaInfoTypes(xmlPath);

            List<string> authorizedTypes = new List<string>();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                     atLeastOneItemChecked = true;
                     authorizedTypes.Add(checkedListBox1.Items[i].ToString());
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

            logTextBox.AppendText("The .tdd file is being processed.\nPlease wait...\n");
            if (estimatedWait > 5)
            {
                logTextBox.AppendText("Estimated waiting time : " + estimatedWait + " seconds \n");
            }
            
            logTextBox.Refresh();
            TDDFileProcessingInWork(true);

            await Task.Delay(10);

            XmlDocument doc = new XmlDocument();
            this.xmlPath = TDMtoTDSMigrator.TDSLoader.Decompress(new FileInfo(TDDPathTextBox.Text));
            doc.Load(xmlPath);
            XmlNode metaInfoTypes = XMLParser.GetMetaInfoTypes(XMLParser.GetParentNodeOfData(doc));
            LoadCategories(metaInfoTypes);

            logTextBox.Height = 470;
            progressBar.Visible = true;


            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, r) => {
                //Some work...
                r.Result = this.objectList = TDSLoader.TransformXMLIntoObjectList(xmlPath);
            };
            worker.RunWorkerCompleted += (s, r) => {
                this.objectList =(List<TableObject>)r.Result;
                logTextBox.AppendText("\nThe .tdd file was successfully processed. \n" + metaInfoTypes.ChildNodes.Count + " categories were found. \nPlease filter out the categories you need, pick a target repository, then click \"Load categories into repository\" to launch the transfer.\n");
                logTextBox.Refresh();
                CheckForEmptyCategories(metaInfoTypes);    
                TDDFileProcessingInWork(false);
                progressBar.Visible = false;
                logTextBox.Height = 533;
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
            checkedListBox1.Items.Clear();

            
            TDDPathTextBox.Text = openFileDialog.FileName;
            

        }
        private void createRepositoryButton_Click(object sender, EventArgs e)
        {
            if (repositoryName.Text != "")
            {
                if (!repositoriesBox.Items.Contains(repositoryName.Text))
                {


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
            for(int  i = 0; i<checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            
        }
        private void deselectAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
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


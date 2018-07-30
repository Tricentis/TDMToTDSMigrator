using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace TDMtoTDSMigrator
{
    public class XmlParser
    {
        /*
            Decompressed XML File attributes :
                
                -RepositoryDump (parent node of contained data)

                    -MetaInfoTypes (ex: Person, City)
                    -MetaInfoAttributes (ex: Name, Adress ; Country, Population) --> each attribute is linked to a metaInfoType
                    -MetaStringAttributes (ex: surrogate=1 , attribute=1 , value = John) --> sets the value of each attribute of an object (each object has a unique surrogate)
                    -Associations (not supported by TDS, if there are any the user is warned that they will no longer be available in TDS)
               
             
        */
        
        public static List<TableObject> CreateDataList(XmlNode stringAttributes, XmlNode metaInfoTypes, XmlNode metaInfoAttributes,List<string[]> typeIDs)
        {
            
            List<TableObject> dataList = new List<TableObject>();
            string currentsurrogate = stringAttributes.ChildNodes[0].Attributes?[0].Value;

            TableObject currentDataObject = new TableObject();

            //Reads the attributes of each data object
            for (int i = 0; i < stringAttributes.ChildNodes.Count; i++)
            {
                //Checks if the loop has switched to a different data object (surrogate=surrogate+1).                
                if (currentsurrogate != stringAttributes.ChildNodes[i].Attributes?[0].Value)
                {
                    //Finds and sets the category and attributes names of the object
                    for (int j = 0; j < typeIDs.Count; j++)
                    {
                        if (currentDataObject.GetAttributes()[0][0] != typeIDs[j][0]) continue;
                        currentDataObject.SetTypeId(typeIDs[j][2]);
                        break;
                    }

                    currentDataObject.SetCategoryName(currentDataObject.GetTypeId(), metaInfoTypes);
                    currentDataObject.SetAttributeNames(metaInfoAttributes);

                    dataList.Add(new TableObject(currentDataObject));
                    currentDataObject = new TableObject();
                }

                currentDataObject.AddAttribute(stringAttributes.ChildNodes[i].Attributes?[1].Value, stringAttributes.ChildNodes[i].Attributes?[2].Value);
                currentsurrogate = stringAttributes.ChildNodes[i].Attributes?[0].Value;
            }


            //Store the last object into the list
            //Find and set the TypeID of the last object
            foreach (var typeId in typeIDs)
            {
                if (currentDataObject.GetAttributes()[0][0] == typeId[0])
                {
                    currentDataObject.SetTypeId(typeId[2]);
                    break;
                }
            }
            currentDataObject.SetCategoryName(currentDataObject.GetTypeId(), metaInfoTypes);
            currentDataObject.SetAttributeNames(metaInfoAttributes);
            dataList.Add(new TableObject(currentDataObject));



            return dataList;
        }       
        public static List<TableObject> CreateDataList(XmlDocument doc)
        {

            XmlNode metaInfoTypes = GetMetaInfoTypes(doc);
            XmlNode metaInfoAttributes = GetMetaInfoAttributes(doc);
            XmlNode stringAttributes = GetStringAttributes(doc);

            List<string[]> typeIDs = GetTypeIDs(metaInfoAttributes);

            List<TableObject> dataList = CreateDataList(stringAttributes, metaInfoTypes, metaInfoAttributes, typeIDs);

            return dataList;
        }
        public static List<TableObject> CreateDataList(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return CreateDataList(doc);
        }



        public static XmlNode GetRepositoryDump(XmlDocument doc)
        {
            XmlNode repositoryDump = doc.FirstChild;
            repositoryDump = repositoryDump.NextSibling; 
            return repositoryDump;
        }

        public static XmlNode GetMetaInfoTypes(XmlDocument doc)
        {
            XmlNode repositoryDump = GetRepositoryDump(doc);
            XmlNode metaInfoType = null;
            for (int i = 0; i< repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoType")
                {
                    metaInfoType = repositoryDump.ChildNodes[i];
                    break;
                }               
            }
            return metaInfoType;
        }
        public static XmlNode GetMetaInfoTypes(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetMetaInfoTypes(doc);
        }

        public static XmlNode GetMetaInfoAttributes(XmlDocument doc)
        {
            XmlNode repositoryDump = GetRepositoryDump(doc);
            XmlNode metaInfoAttributes = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoAttribute")
                {
                    metaInfoAttributes = repositoryDump.ChildNodes[i];
                    break;
                }
            }
            return metaInfoAttributes;
        }
        public static XmlNode GetMetaInfoAttributes(string xmlPath)
        {
            XmlDocument doc =new XmlDocument();
            doc.Load(xmlPath);
            return GetMetaInfoAttributes(doc);
        }

        public static XmlNode GetStringAttributes(XmlDocument doc)
        {
            XmlNode repositoryDump = GetRepositoryDump(doc);
            XmlNode stringAttributes = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {               
                if (repositoryDump.ChildNodes[i].Name == "StringAttribute")
                {
                    stringAttributes = repositoryDump.ChildNodes[i];
                    break;
                }               
            }
            return stringAttributes;
        }
        public static XmlNode GetStringAttributes(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetStringAttributes(doc);
        }

        public static XmlNode GetMetaInfoAssociations(XmlDocument doc)
        {
            XmlNode repositoryDump = GetRepositoryDump(doc);
            XmlNode metaInfoAssoc = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoAssoc")
                {
                    metaInfoAssoc = repositoryDump.ChildNodes[i];
                    break;
                }
            }
            return metaInfoAssoc;
        }
        public static XmlNode GetMetaInfoAssociations(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetMetaInfoAssociations(doc);
        }

        
        public static List<string[]> GetTypeIDs(XmlNode metaInfoAttributes)
        {
            List<string[]> typeIds = new List<string[]>();
            for (int i = 0; i < metaInfoAttributes.ChildNodes.Count; i++)
            {
                typeIds.Add(new string[] { metaInfoAttributes.ChildNodes[i].Attributes?[0].Value, metaInfoAttributes.ChildNodes[i].Attributes?[1].Value, metaInfoAttributes.ChildNodes[i].Attributes?[2].Value });
            }
            return typeIds;
        }
        
    }
}

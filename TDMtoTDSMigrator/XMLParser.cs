using System;
using System.Collections.Generic;
using System.Xml;

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
            //Create an empty list of data rows
            List<TableObject> objects = new List<TableObject>();

            string surrogate = stringAttributes.ChildNodes[0].Attributes?[0].Value;
            string currentsurrogate = stringAttributes.ChildNodes[0].Attributes?[0].Value;

            TableObject obj = new TableObject();

            //Reads the attributes and 
            for (int i = 0; i < stringAttributes.ChildNodes.Count; i++)
            {
                //Checks if the loop has switched to a different row (surrogate=surrogate+1).
                //If yes, the current object is stored in the list and a new object is created
                if (currentsurrogate != stringAttributes.ChildNodes[i].Attributes?[0].Value)
                {
                    //Find and set the TypeID and type of the object
                    for (int j = 0; j < typeIDs.Count; j++)
                    {
                        if (obj.GetAttributes()[0][0] != typeIDs[j][0]) continue;
                        obj.SetTypeId(typeIDs[j][2]);
                        break;
                    }


                    obj.SetCategoryName(obj.GetTypeId(), metaInfoTypes);
                    obj.SetAttributeNames(metaInfoAttributes);

                    objects.Add(new TableObject(obj));
                    obj = new TableObject();
                }

                obj.AddAttribute(stringAttributes.ChildNodes[i].Attributes?[1].Value, stringAttributes.ChildNodes[i].Attributes?[2].Value);
                currentsurrogate = stringAttributes.ChildNodes[i].Attributes?[0].Value;
            }


            //Store the last object into the list
            //Find and set the TypeID of the last object
            foreach (var typeId in typeIDs)
            {
                if (obj.GetAttributes()[0][0] == typeId[0])
                {
                    obj.SetTypeId(typeId[2]);
                    break;
                }
            }
            obj.SetCategoryName(obj.GetTypeId(), metaInfoTypes);
            obj.SetAttributeNames(metaInfoAttributes);
            objects.Add(new TableObject(obj));



            return objects;
        }

        
        public static List<TableObject> ConvertXmlIntoDataList(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return ConvertXmlIntoDataList(doc);
        }
        public static List<TableObject> ConvertXmlIntoDataList(XmlDocument doc)
        {
            XmlNode repositoryDump = GetRepositoryDump(doc);

            XmlNode metaInfoTypes = GetMetaInfoTypes(repositoryDump);
            XmlNode metaInfoAttributes = GetMetaInfoAttributes(repositoryDump);
            XmlNode stringAttributes = GetStringAttributes(repositoryDump);

            List<string[]> typeIDs = GetTypeIDs(metaInfoAttributes);

            List<TableObject> objectList = CreateDataList(stringAttributes, metaInfoTypes, metaInfoAttributes, typeIDs);

            return objectList;
        }


        public static XmlNode GetRepositoryDump(XmlDocument doc)
        {
            XmlNode root = doc.FirstChild;
            root = root.NextSibling; 
            return root;
        }
        public static XmlNode GetMetaInfoTypes(XmlNode repositoryDump)
        {
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
            XmlNode repositoryDump = GetRepositoryDump(doc);
            XmlNode metaInfoType = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoType")
                {
                    metaInfoType = repositoryDump.ChildNodes[i];
                    break;
                }
            }
            return metaInfoType;
        }
        public static XmlNode GetMetaInfoAttributes(XmlNode repositoryDump)
        {
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
        public static XmlNode GetStringAttributes(XmlNode repositoryDump)
        {
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
        public static List<string[]> GetTypes(XmlNode metaInfoTypes)
        {
            List<string[]> types = new List<string[]>();
            for (int i = 0; i < metaInfoTypes.ChildNodes.Count; i++)
            {
                types.Add(new string[] { metaInfoTypes.ChildNodes[i].Attributes?[0].Value, metaInfoTypes.ChildNodes[i].Attributes?[1].Value });
            }
            return types;
        }
        public static List<string[]> GetTypes(XmlDocument doc)
        {
            XmlNode repositoryDump = GetRepositoryDump(doc);
            XmlNode metaInfoTypes = GetMetaInfoTypes(repositoryDump);
            List<string[]> types = GetTypes(metaInfoTypes);
            return types;
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

using System;
using System.Collections.Generic;
using System.Xml;

namespace TDMtoTDSMigrator
{
    public class XMLParser
    {
        /*
            Decompressed XML File attributes :
                
                -RepositoryDump (parent node of contained data)

                    -MetaInfoTypes (ex: Person, City)
                    -MetaInfoAttributes (ex: Name, Adress ; Country, Population) --> each attribute is linked to a metaInfoType
                    -MetaStringAttributes (ex: surrogate=1 , attribute=1 , value = John) --> sets the value of each attribute of an object (each object has a unique surrogate)
                    -Associations (not supported by TDS, if there are any the user is warned that they will no longer be available in TDS)
             
             
        */
        
        public static List<TableObject> CreateObjectList(XmlNode stringAttributes, XmlNode metaInfoTypes, XmlNode metaInfoAttributes,List<string[]> TypeIDs)
        {
            XmlNodeList StringAttributes = stringAttributes.ChildNodes;
            //Create an empty list of rows (objects)
            List<TableObject> Objects = new List<TableObject>();

            string surrogate = StringAttributes[0].Attributes[0].Value;
            string currentsurrogate = StringAttributes[0].Attributes[0].Value;

            TableObject obj = new TableObject();

            //Reads the attributes and arranges them into rows (TableObjects)
            for (int i = 0; i < StringAttributes.Count; i++)
            {
                //Checks if the loop has switched to a different row (surrogate+1).
                //If yes, the current object is stored in the list and a new object is created
                if (currentsurrogate != StringAttributes[i].Attributes[0].Value)
                {
                    //Find and set the TypeID and type of the object
                    for (int j = 0; j < TypeIDs.Count; j++)
                    {
                        if (obj.GetAttributes()[0][0] == TypeIDs[j][0])
                        {
                            obj.SetTypeId(TypeIDs[j][2]);

                            break;
                        }
                    }


                    obj.SetTypeName(obj.GetTypeId(), metaInfoTypes);
                    obj.SetAttributeNames(metaInfoAttributes);

                    Objects.Add(new TableObject(obj));
                    obj = new TableObject();
                }
                obj.AddAttribute(StringAttributes[i].Attributes[1].Value, StringAttributes[i].Attributes[2].Value);
                currentsurrogate = StringAttributes[i].Attributes[0].Value;
            }


            //Store the last object into the list
            //Find and set the TypeID of the last object
            for (int j = 0; j < TypeIDs.Count; j++)
            {
                if (obj.GetAttributes()[0][0] == TypeIDs[j][0])
                {
                    obj.SetTypeId(TypeIDs[j][2]);
                    break;
                }
            }
            obj.SetTypeName(obj.GetTypeId(), metaInfoTypes);
            obj.SetAttributeNames(metaInfoAttributes);
            Objects.Add(new TableObject(obj));



            return Objects;
        }

        
        public static List<TableObject> ParseXmlIntoObjectList(string fileUrl)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileUrl);
            return ParseXmlIntoObjectList(doc);
        }
        public static List<TableObject> ParseXmlIntoObjectList(XmlDocument doc)
        {
            XmlNode repositoryDump = GetParentNodeOfData(doc);

            XmlNode metaInfoTypes = GetMetaInfoTypes(repositoryDump);
            XmlNode metaInfoAttributes = GetMetaInfoAttributes(repositoryDump);
            XmlNode stringAttributes = GetStringAttributes(repositoryDump);

            List<string[]> Types = GetTypes(metaInfoTypes);
            List<string[]> TypeIDs = GetTypeIDs(metaInfoAttributes);

            List<TableObject> objectList = CreateObjectList(stringAttributes, metaInfoTypes, metaInfoAttributes, TypeIDs);

            return objectList;
        }


        public static XmlNode GetParentNodeOfData(XmlDocument doc)
        {
            XmlNode root = doc.FirstChild;
            root = root.NextSibling; //moves to the "repositoryDump' node which contains the data
            return root;
        }
        public static XmlNode GetMetaInfoTypes(XmlNode repositoryDump)
        {
            XmlNode MetaInfoType = null;
            for (int i = 0; i< repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoType")
                {
                    MetaInfoType = repositoryDump.ChildNodes[i];
                    break;
                }               
            }
            return MetaInfoType;
        }
        public static XmlNode GetMetaInfoTypes(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            XmlNode repositoryDump = GetParentNodeOfData(doc);
            XmlNode MetaInfoType = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoType")
                {
                    MetaInfoType = repositoryDump.ChildNodes[i];
                    break;
                }
            }
            return MetaInfoType;
        }
        public static string GetAttributeName(string Id, XmlNode metaInfoAttributes) 
        {
            for(int i = 0; i < metaInfoAttributes.ChildNodes.Count; i++)
            {
                if (metaInfoAttributes.ChildNodes[i].Attributes[0].Value == Id)
                {
                    return metaInfoAttributes.ChildNodes[i].Attributes[1].Value;
                }
            }
            
            return "notFound";
        }
        public static XmlNode GetMetaInfoAttributes(XmlNode repositoryDump)
        {
            XmlNode MetaInfoAttribute = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {
                if (repositoryDump.ChildNodes[i].Name == "MetaInfoAttribute")
                {
                    MetaInfoAttribute = repositoryDump.ChildNodes[i];
                    break;
                }
            }
            return MetaInfoAttribute;
        }
        public static XmlNode GetStringAttributes(XmlNode repositoryDump)
        {
            XmlNode StringAttribute = null;
            for (int i = 0; i < repositoryDump.ChildNodes.Count; i++)
            {               
                if (repositoryDump.ChildNodes[i].Name == "StringAttribute")
                {
                    StringAttribute = repositoryDump.ChildNodes[i];
                    break;
                }               
            }
            return StringAttribute;
        }
        public static List<string[]> GetTypes(XmlNode MetaInfoTypes)
        {
            List<string[]> Types = new List<string[]>();
            for (int i = 0; i < MetaInfoTypes.ChildNodes.Count; i++)
            {
                Types.Add(new string[] { MetaInfoTypes.ChildNodes[i].Attributes[0].Value, MetaInfoTypes.ChildNodes[i].Attributes[1].Value });
            }
            return Types;
        }
        public static List<string[]> GetTypes(XmlDocument doc)
        {
            XmlNode repositoryDump = GetParentNodeOfData(doc);
            XmlNode MetaInfoTypes = GetMetaInfoTypes(repositoryDump);
            List<string[]> Types = GetTypes(MetaInfoTypes);
            return Types;
        }
        public static List<string[]> GetTypes(string fileUrl)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileUrl);
            return GetTypes(doc);
        }
        public static List<string[]> GetTypeIDs(XmlNode MetaInfoAttributes)
        {
            List<string[]> TypeIds = new List<string[]>();
            for (int i = 0; i < MetaInfoAttributes.ChildNodes.Count; i++)
            {
                TypeIds.Add(new string[] { MetaInfoAttributes.ChildNodes[i].Attributes[0].Value, MetaInfoAttributes.ChildNodes[i].Attributes[1].Value, MetaInfoAttributes.ChildNodes[i].Attributes[2].Value });
            }
            return TypeIds;
        }
        
    }
}

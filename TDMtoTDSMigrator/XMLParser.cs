using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

using TestDataContract.TestData;

namespace TDMtoTDSMigrator {
    public class XmlParser {
        // Decompressed XML File attributes :
        //     
        //     -RepositoryDump (parent node)
        //
        //     -MetaInfoTypes (ex: Person, City) corresponds to categories in TDS
        //     -MetaInfoAttributes (ex: Name, Adress ; Country, Population) --> each attribute is linked to a unique metaInfoType
        //     -MetaStringAttributes (ex: surrogate=1 , attribute=1 , value = John) --> sets the value of each attribute of an object (each object has a unique surrogate)
        //     -Associations (not supported by TDS, if there are any the user is warned that they will no longer be available in TDS)

        public static string DecompressTddFileIntoXml(FileInfo fi) {
            using (FileStream inFile = fi.OpenRead()) {
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

                string pathOfOutput = origName + ".xml";
                using (FileStream outFile = File.Create(pathOfOutput)) {
                    using (GZipStream decompress = new GZipStream(inFile, CompressionMode.Decompress)) {
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0) {
                            outFile.Write(buffer, 0, numRead);
                        }
                    }
                }
                return pathOfOutput;
            }
        }

        public static Dictionary<string, List<TestDataObject>> CreateDataList(XmlNode stringAttributes, XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            Dictionary<string, RawDataObject> rawDataObjects = new Dictionary<string, RawDataObject>();
            foreach (XmlNode stringAttribute in stringAttributes.ChildNodes) {
                string id = ObjectId(stringAttribute);
                try
                {
                    rawDataObjects.Add(id, new RawDataObject());
                    rawDataObjects[id].AddAttribute(AttributeId(stringAttribute), AttributeValue(stringAttribute));
                }
                catch (ArgumentException)
                { 
                    rawDataObjects[id].AddAttribute(AttributeId(stringAttribute), AttributeValue(stringAttribute));
                }
            }
            return ArrangeData(rawDataObjects, metaInfoTypes, metaInfoAttributes);
        }

        public static Dictionary<string, List<TestDataObject>> CreateDataList(XmlDocument doc) {
            return CreateDataList(GetStringAttributes(doc), GetMetaInfoTypes(doc), GetMetaInfoAttributes(doc));
        }

        public static Dictionary<string, List<TestDataObject>> CreateDataList(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return CreateDataList(doc);
        }

        private static Dictionary<string, List<TestDataObject>> ArrangeData(Dictionary<string,RawDataObject> rawDataObjects, XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            Dictionary<string, List<TestDataObject>> sortedData = new Dictionary<string, List<TestDataObject>>();
            foreach (RawDataObject obj in rawDataObjects.Values) {
                obj.SetAllAttributes(metaInfoTypes, metaInfoAttributes);
                try
                {
                    sortedData.Add(obj.categoryName, new List<TestDataObject>());
                    sortedData[obj.categoryName].Add(obj.ConvertIntoTestDataObject());
                }
                catch (ArgumentException)
                {
                    sortedData[obj.categoryName].Add(obj.ConvertIntoTestDataObject());
                }
            }
            return sortedData;
        }

        public static string ObjectId(XmlNode stringAttribute) {
            return stringAttribute.Attributes?[0].Value;
        }

        public static string AttributeId(XmlNode stringAttribute) {
            return stringAttribute.Attributes?[1].Value;
        }

        public static string AttributeValue(XmlNode stringAttribute) {
            return stringAttribute.Attributes?[2].Value;
        }

        public static string Category(XmlNode metaInfoType) {
            return metaInfoType.Attributes?[1].Value;
        }

        public static XmlNode GetRepositoryDump(XmlDocument doc) {
            foreach (XmlNode node in doc.ChildNodes) {
                if (node.Name == "RepositoryDump") {
                    return node;
                }
            }
            return null;
        }

        public static XmlNode GetMetaInfoTypes(XmlDocument doc) {
            foreach (XmlNode node in GetRepositoryDump(doc).ChildNodes) {
                if (node.Name == "MetaInfoType") {
                    return node;
                }
            }
            return null;
        }

        public static XmlNode GetMetaInfoTypes(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetMetaInfoTypes(doc);
        }

        public static XmlNode GetMetaInfoAttributes(XmlDocument doc) {
            foreach (XmlNode node in GetRepositoryDump(doc).ChildNodes) {
                if (node.Name == "MetaInfoAttribute") {
                    return node;
                }
            }
            return null;
        }

        public static XmlNode GetStringAttributes(XmlDocument doc) {
            foreach (XmlNode node in GetRepositoryDump(doc).ChildNodes) {
                if (node.Name == "StringAttribute") {
                    return node;
                }
            }
            return null;
        }

        public static XmlNode GetMetaInfoAssociations(XmlDocument doc) {
            foreach (XmlNode node in GetRepositoryDump(doc).ChildNodes) {
                if (node.Name == "MetaInfoAssoc") {
                    return node;
                }
            }
            return null;
        }

        public static XmlNode GetMetaInfoAssociations(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetMetaInfoAssociations(doc);
        }

        public static List<string[]> GetCategoriesInfos(XmlNode metaInfoAttributes) {
            List<string[]> categoriesInfo = new List<string[]>();
            foreach (XmlNode metaInfoAttribute in metaInfoAttributes.ChildNodes) {
                categoriesInfo.Add(new[] { metaInfoAttribute.Attributes?[0].Value, metaInfoAttribute.Attributes?[1].Value, metaInfoAttribute.Attributes?[2].Value });
            }
            return categoriesInfo;
        }
    }
}
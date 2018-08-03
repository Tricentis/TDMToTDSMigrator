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
            List<TestDataObject> dataList = new List<TestDataObject>();
            RawDataObject currentObject = new RawDataObject();

            foreach (XmlNode stringAttribute in stringAttributes.ChildNodes) {
                currentObject.AddAttribute(AttributeId(stringAttribute), AttributeValue(stringAttribute));

                if (stringAttribute.NextSibling == null || ObjectId(stringAttribute.NextSibling) != ObjectId(stringAttribute)) {
                    currentObject.SetAllAttributes(stringAttributes, metaInfoTypes, metaInfoAttributes);
                    dataList.Add(currentObject.ConvertIntoTestDataObject());
                    currentObject = new RawDataObject();
                }
            }
            return SortDataList(dataList, metaInfoTypes);
        }

        public static Dictionary<string, List<TestDataObject>> CreateDataList(XmlDocument doc) {
            return CreateDataList(GetStringAttributes(doc), GetMetaInfoTypes(doc), GetMetaInfoAttributes(doc));
        }

        public static Dictionary<string, List<TestDataObject>> CreateDataList(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return CreateDataList(doc);
        }

        private static Dictionary<string, List<TestDataObject>> SortDataList(List<TestDataObject> dataList, XmlNode metaInfoTypes) {
            Dictionary<string, List<TestDataObject>> sortedTestData = new Dictionary<string, List<TestDataObject>>();
            foreach (XmlNode metaInfoType in metaInfoTypes) {
                sortedTestData.Add(Category(metaInfoType), new List<TestDataObject>());
            }
            foreach (TestDataObject obj in dataList) {
                sortedTestData[obj.Category].Add(obj);
            }
            return sortedTestData;
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
            foreach (XmlNode node in metaInfoAttributes.ChildNodes) {
                categoriesInfo.Add(new[] { node.Attributes?[0].Value, node.Attributes?[1].Value, node.Attributes?[2].Value });
            }
            return categoriesInfo;
        }
    }
}
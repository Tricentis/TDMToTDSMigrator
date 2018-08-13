using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

using Newtonsoft.Json.Linq;

using TestDataContract.TestData;

namespace TDMtoTDSMigrator {
    public class TdmDataDocument {
        public XmlNode RepositoryDump;

        public Dictionary<string, MetaInfoType> MetaInfoTypes;

        public Dictionary<string, MetaInfoAttribute> MetaInfoAttributes;

        public List<MetaInfoAssociation> MetaInfoAssociations;

        public Dictionary<string, List<StringAttribute>> StringAttributes;

        public Dictionary<string, TestDataCategory> TestData;

        public TdmDataDocument(string tddPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(DecompressTddFileIntoXml(tddPath));
            RepositoryDump = GetRepositoryDump(doc);
            LoadMetaInfoAssociations();
            LoadMetaInfoTypes();
            LoadMetaInfoAttributes();
            LoadStringAttributes();
        }

        public void CreateDataDictionary() {
            TestData = new Dictionary<string, TestDataCategory>();
            foreach (MetaInfoType metaInfoType in MetaInfoTypes.Values) {
                TestData.Add(metaInfoType.CategoryName, new TestDataCategory { Name = metaInfoType.CategoryName, Elements = new List<TestDataObject>(), ElementCount = 0 });
            }
            foreach (string objectId in StringAttributes.Keys) {
                JObject data = new JObject();
                foreach (StringAttribute stringAttribute in StringAttributes[objectId]) {
                    data.Add(stringAttribute.AttributeName, stringAttribute.AttributeValue);
                }
                TestDataObject obj = new TestDataObject { Category = StringAttributes[objectId][0].CategoryName, Data = JObject.Parse(data.ToString()), Consumed = false };
                TestData[obj.Category].Elements.Add(obj);
                TestData[obj.Category].ElementCount++;
            }
        }

        public static string DecompressTddFileIntoXml(string tddPath) {
            FileInfo fi = new FileInfo(tddPath);
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

        public string FindAttributeName(string attributeId) {
            foreach (MetaInfoAttribute metaInfoAttribute in MetaInfoAttributes.Values) {
                if (metaInfoAttribute.AttributeId == attributeId) {
                    return metaInfoAttribute.AttributeName;
                }
            }
            return "Attribute not found";
        }

        public string FindCategoryName(string categoryId) {
            return MetaInfoTypes[categoryId].CategoryName;
        }

        public string FindCategoryId(string attributeId) {
            return MetaInfoAttributes[attributeId].CorrespondingCategoryId;
        }

        private void LoadMetaInfoTypes() {
            MetaInfoTypes = new Dictionary<string, MetaInfoType>();
            foreach (XmlNode node in GetMetaInfoTypes()) {
                MetaInfoType metaInfoType = new MetaInfoType(node);
                MetaInfoTypes.Add(metaInfoType.CategoryId, metaInfoType);
            }
        }

        private void LoadMetaInfoAttributes() {
            MetaInfoAttributes = new Dictionary<string, MetaInfoAttribute>();
            foreach (XmlNode node in GetMetaInfoAttributes()) {
                MetaInfoAttribute metaInfoAttribute = new MetaInfoAttribute(node);
                MetaInfoAttributes.Add(metaInfoAttribute.AttributeId, metaInfoAttribute);
            }
        }

        private void LoadMetaInfoAssociations() {
            MetaInfoAssociations = new List<MetaInfoAssociation>();
            foreach (XmlNode node in GetMetaInfoAssociations()) {
                MetaInfoAssociations.Add(new MetaInfoAssociation(node));
            }
        }

        private void LoadStringAttributes() {
            StringAttributes = new Dictionary<string, List<StringAttribute>>();
            foreach (XmlNode node in GetStringAttributes()) {
                StringAttribute attribute = new StringAttribute(this, node);
                try {
                    StringAttributes[attribute.ObjectId].Add(attribute);
                } catch (Exception) {
                    StringAttributes.Add(attribute.ObjectId, new List<StringAttribute>());
                    StringAttributes[attribute.ObjectId].Add(attribute);
                }
            }
        }

        private XmlNode GetRepositoryDump(XmlDocument doc) {
            foreach (XmlNode node in doc.ChildNodes) {
                if (node.Name == "RepositoryDump") {
                    return node;
                }
            }
            return null;
        }

        private XmlNode GetMetaInfoTypes() {
            foreach (XmlNode node in RepositoryDump.ChildNodes) {
                if (node.Name == "MetaInfoType") {
                    return node;
                }
            }
            return null;
        }

        private XmlNode GetMetaInfoAttributes() {
            foreach (XmlNode node in RepositoryDump.ChildNodes) {
                if (node.Name == "MetaInfoAttribute") {
                    return node;
                }
            }
            return null;
        }

        private XmlNode GetStringAttributes() {
            foreach (XmlNode node in RepositoryDump.ChildNodes) {
                if (node.Name == "StringAttribute") {
                    return node;
                }
            }
            return null;
        }

        private XmlNode GetMetaInfoAssociations() {
            foreach (XmlNode node in RepositoryDump.ChildNodes) {
                if (node.Name == "MetaInfoAssoc") {
                    return node;
                }
            }
            return null;
        }

        public int CountNumberOfObjects() {
            return StringAttributes.Keys.Count;
        }

        public int CountNumberOfCategories() {
            return MetaInfoTypes.Keys.Count;
        }
    }
}
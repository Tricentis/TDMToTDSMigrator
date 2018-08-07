using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
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

        public TdmDataDocument(string tddPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(DecompressTddFileIntoXml(tddPath));
            RepositoryDump = GetRepositoryDump(doc);
            LoadMetaInfoAssociations();
            LoadMetaInfoTypes();
            LoadMetaInfoAttributes();
            LoadStringAttributes();
        }

        public Dictionary<string, List<TestDataObject>> CreateDataList()
        {
            Dictionary<string, List<TestDataObject>> testDataObjects = new Dictionary<string, List<TestDataObject>>();
            foreach (MetaInfoType metaInfoType in MetaInfoTypes.Values) {
                testDataObjects.Add(metaInfoType.CategoryName, new List<TestDataObject>());
            }
            foreach (string objectId in StringAttributes.Keys) {
                JObject data = new JObject();
                foreach (StringAttribute stringAttribute in StringAttributes[objectId])
                {
                    data.Add(stringAttribute.AttributeName, stringAttribute.AttributeValue);
                }
                TestDataObject obj = new TestDataObject() {
                        Category = StringAttributes[objectId][0].CategoryName,
                        Data = JObject.Parse(data.ToString()),
                        Consumed = false 
                };
                testDataObjects[obj.Category].Add(obj);
            }
            return testDataObjects;
        }

        public static string DecompressTddFileIntoXml(string tddPath)
        {
            FileInfo fi = new FileInfo(tddPath);
            using (FileStream inFile = fi.OpenRead())
            {
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

                string pathOfOutput = origName + ".xml";
                using (FileStream outFile = File.Create(pathOfOutput))
                {
                    using (GZipStream decompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFile.Write(buffer, 0, numRead);
                        }
                    }
                }
                return pathOfOutput;
            }
        }

        public string FindAttributeName(string attributeId)
        {
            foreach (MetaInfoAttribute metaInfoAttribute in MetaInfoAttributes.Values)
            {
                if (metaInfoAttribute.AttributeId == attributeId)
                {
                    return metaInfoAttribute.AttributeName;
                }
            }
            return "Attribute not found";
        }

        public string FindCategoryName(string categoryId)
        {
             return MetaInfoTypes[categoryId].CategoryName;
        }

        public string FindCategoryId(string attributeId)
        {
             return MetaInfoAttributes[attributeId].CorrespondingCategoryId;
        }

        private void LoadMetaInfoTypes() {
            MetaInfoTypes = new Dictionary<string, MetaInfoType>();
            foreach (XmlNode node in GetMetaInfoTypes()) {
                MetaInfoTypes.Add(node.Attributes?[0].Value ?? throw new InvalidOperationException(),new MetaInfoType(node));
            }
        }

        private void LoadMetaInfoAttributes() {
            MetaInfoAttributes = new Dictionary<string, MetaInfoAttribute>();
            foreach (XmlNode node in GetMetaInfoAttributes()) {
                MetaInfoAttributes.Add(node.Attributes?[0].Value ?? throw new InvalidOperationException(), new MetaInfoAttribute(node));
            }
        }

        private void LoadMetaInfoAssociations() {
            MetaInfoAssociations =new List<MetaInfoAssociation>();
            foreach (XmlNode node in GetMetaInfoAssociations()) {
                MetaInfoAssociations.Add(new MetaInfoAssociation(node));
            }
        }

        private void LoadStringAttributes() {
            StringAttributes = new Dictionary<string, List<StringAttribute>>();
            foreach (XmlNode node in GetStringAttributes()) {
                try {
                    StringAttributes[node.Attributes?[0].Value ?? throw new InvalidOperationException()].Add(new StringAttribute(this,node));
                } catch (Exception) {
                    StringAttributes.Add(node.Attributes?[0].Value ?? throw new InvalidOperationException(), new List<StringAttribute>());
                    StringAttributes[node.Attributes?[0].Value].Add(new StringAttribute(this, node));
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
    }

    public class StringAttribute {

        private readonly TdmDataDocument tdmDataSheet;
        public string ObjectId;
        public string AttributeId;

        public string AttributeName {
            get {
                if (AttributeId!=null) {
                    return tdmDataSheet.FindAttributeName(AttributeId);
                }
                return "Not Found";
            }
        }

        public string CategoryName {
            get {
                if (AttributeId != null) {
                    return tdmDataSheet.FindCategoryName(tdmDataSheet.FindCategoryId(AttributeId));
                }
                return "Not Found";
            }
        }

        public string AttributeValue;

        public StringAttribute(TdmDataDocument tdmDataSheet, XmlNode stringAttribute) {
            this.tdmDataSheet = tdmDataSheet;
            ObjectId = stringAttribute.Attributes?[0].Value;
            AttributeId = stringAttribute.Attributes?[1].Value;
            AttributeValue = stringAttribute.Attributes?[2].Value;
        }
    }

    public class MetaInfoAttribute {

        public string AttributeId;
        public string AttributeName;
        public string CorrespondingCategoryId;

        public MetaInfoAttribute(XmlNode metaInfoAttribute) {
            AttributeId = metaInfoAttribute.Attributes?[0].Value;
            AttributeName = metaInfoAttribute.Attributes?[1].Value;
            CorrespondingCategoryId = metaInfoAttribute.Attributes?[2].Value;
        }
    }

    public class MetaInfoType {

        public string CategoryId;
        public string CategoryName;

        public MetaInfoType(XmlNode metaInfoType) {
            CategoryId = metaInfoType.Attributes?[0].Value;
            CategoryName = metaInfoType.Attributes?[1].Value;
        }
    }

    public class MetaInfoAssociation {

        public string AssociationId;
        public string CategoryName;
        public string CategoryId;
        public string PartnerId;
        public string PartnerName;

        public MetaInfoAssociation(XmlNode metaInfoAssociation) {
            AssociationId = metaInfoAssociation.Attributes?[0].Value;
            CategoryName = metaInfoAssociation.Attributes?[1].Value;
            CategoryId = metaInfoAssociation.Attributes?[2].Value;
            PartnerId = metaInfoAssociation.Attributes?[3].Value;
            PartnerName = metaInfoAssociation.Attributes?[4].Value;
        }
    }
}
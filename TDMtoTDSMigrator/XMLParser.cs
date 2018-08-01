using System.Collections.Generic;
using System.Xml;

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

        public static List<DataRow> CreateDataList(XmlNode stringAttributes, XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            List<DataRow> dataList = new List<DataRow>();

            string formerRowId = GetCurrentRowId(stringAttributes.ChildNodes[0]);
            DataRow currentDataRow = new DataRow();

            foreach (XmlNode stringAttribute in stringAttributes.ChildNodes) {
                if (formerRowId != GetCurrentRowId(stringAttribute)) {
                    dataList.Add(new DataRow(SetDataAttributes(currentDataRow, stringAttributes, metaInfoTypes, metaInfoAttributes)));
                    currentDataRow = new DataRow();
                }

                currentDataRow.AddAttribute(stringAttribute.Attributes?[1].Value, stringAttribute.Attributes?[2].Value);
                formerRowId = GetCurrentRowId(stringAttribute);
            }

            dataList.Add(new DataRow(SetDataAttributes(currentDataRow, stringAttributes, metaInfoTypes, metaInfoAttributes)));
            return dataList;
        }

        public static List<DataRow> CreateDataList(XmlDocument doc) {
            return CreateDataList(GetStringAttributes(doc), GetMetaInfoTypes(doc), GetMetaInfoAttributes(doc));
        }

        public static List<DataRow> CreateDataList(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return CreateDataList(doc);
        }

        public static DataRow SetDataAttributes(DataRow row, XmlNode stringAttributes, XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            List<string[]> categoryInfos = GetCategoryInfos(metaInfoAttributes);
            foreach (string[] categoryInfo in categoryInfos) {
                if (row.GetAttributes()[0][0] != categoryInfo[0]) {
                    continue;
                }
                row.SetTypeId(categoryInfo[2]);
                break;
            }
            row.SetCategoryName(metaInfoTypes);
            row.SetAttributeNames(metaInfoAttributes);
            return row;
        }

        public static string GetCurrentRowId(XmlNode stringAttribute) {
            return stringAttribute.Attributes?[0].Value;
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

        public static XmlNode GetMetaInfoAttributes(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetMetaInfoAttributes(doc);
        }

        public static XmlNode GetStringAttributes(XmlDocument doc) {
            foreach (XmlNode node in GetRepositoryDump(doc).ChildNodes) {
                if (node.Name == "StringAttribute") {
                    return node;
                }
            }
            return null;
        }

        public static XmlNode GetStringAttributes(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetStringAttributes(doc);
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

        public static List<string[]> GetCategoryInfos(XmlNode metaInfoAttributes) {
            List<string[]> typeIds = new List<string[]>();
            foreach (XmlNode node in metaInfoAttributes.ChildNodes) {
                typeIds.Add(new[] { node.Attributes?[0].Value, node.Attributes?[1].Value, node.Attributes?[2].Value });
            }
            return typeIds;
        }
    }
}
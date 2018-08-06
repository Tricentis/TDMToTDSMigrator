using System.Collections.Generic;
using System.Xml;

namespace TDMtoTDSMigrator {
    public class TdmDataDocument {
        #region Fields

        public List<MetaInfoAssociation> MetaInfoAssociations;

        public List<MetaInfoAttribute> MetaInfoAttributes;

        public List<MetaInfoType> MetaInfoTypes;

        public XmlNode RepositoryDump;

        public List<StringAttribute> StringAttributes;

        #endregion

        #region Constructors and Destructors

        public TdmDataDocument(string xmlPath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            RepositoryDump = GetRepositoryDump(doc);
            LoadMetaInfoAssociations();
            LoadMetaInfoTypes();
            LoadMetaInfoAttributes();
            LoadStringAttributes();
        }

        #endregion

        #region Methods

        private XmlNode GetMetaInfoAssociations() {
            foreach (XmlNode node in RepositoryDump.ChildNodes) {
                if (node.Name == "MetaInfoAssoc") {
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

        private XmlNode GetMetaInfoTypes() {
            foreach (XmlNode node in RepositoryDump.ChildNodes) {
                if (node.Name == "MetaInfoType") {
                    return node;
                }
            }
            return null;
        }

        private XmlNode GetRepositoryDump(XmlDocument doc) {
            foreach (XmlNode node in doc.ChildNodes) {
                if (node.Name == "RepositoryDump") {
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

        private void LoadMetaInfoAssociations() {
            MetaInfoAssociations = new List<MetaInfoAssociation>();
            foreach (XmlNode node in GetMetaInfoAssociations()) {
                MetaInfoAssociations.Add(new MetaInfoAssociation(node));
            }
        }

        private void LoadMetaInfoAttributes() {
            MetaInfoAttributes = new List<MetaInfoAttribute>();
            foreach (XmlNode node in GetMetaInfoAttributes()) {
                MetaInfoAttributes.Add(new MetaInfoAttribute(node));
            }
        }

        private void LoadMetaInfoTypes() {
            MetaInfoTypes = new List<MetaInfoType>();
            foreach (XmlNode node in GetMetaInfoTypes()) {
                MetaInfoTypes.Add(new MetaInfoType(node));
            }
        }

        private void LoadStringAttributes() {
            StringAttributes = new List<StringAttribute>();
            foreach (XmlNode node in GetStringAttributes()) {
                StringAttributes.Add(new StringAttribute(node));
            }
        }

        #endregion
    }

    public class StringAttribute {
        #region Fields

        public string AttributeId;

        public string AttributeValue;

        public string ObjectId;

        #endregion

        #region Constructors and Destructors

        public StringAttribute(XmlNode stringAttribute) {
            ObjectId = stringAttribute.Attributes?[0].Value;
            AttributeId = stringAttribute.Attributes?[1].Value;
            AttributeValue = stringAttribute.Attributes?[2].Value;
        }

        #endregion
    }

    public class MetaInfoAttribute {
        #region Fields

        public string AssociatedCategoryId;

        public string AttributeId;

        public string AttributeName;

        #endregion

        #region Constructors and Destructors

        public MetaInfoAttribute(XmlNode metaInfoAttribute) {
            AttributeId = metaInfoAttribute.Attributes?[0].Value;
            AttributeName = metaInfoAttribute.Attributes?[1].Value;
            AssociatedCategoryId = metaInfoAttribute.Attributes?[2].Value;
        }

        #endregion
    }

    public class MetaInfoType {
        #region Fields

        public string CategoryId;

        public string CategoryName;

        #endregion

        #region Constructors and Destructors

        public MetaInfoType(XmlNode metaInfoType) {
            CategoryId = metaInfoType.Attributes?[0].Value;
            CategoryName = metaInfoType.Attributes?[1].Value;
        }

        #endregion
    }

    public class MetaInfoAssociation {
        #region Fields

        public string AssociatedCategoryId;

        public string AssociationId;

        public string CategoryId;

        public string CategoryName;

        public string PartnerName;

        #endregion

        #region Constructors and Destructors

        public MetaInfoAssociation(XmlNode metaInfoType) {
            AssociationId = metaInfoType.Attributes?[0].Value;
            CategoryName = metaInfoType.Attributes?[1].Value;
            CategoryId = metaInfoType.Attributes?[2].Value;
            AssociatedCategoryId = metaInfoType.Attributes?[3].Value;
            PartnerName = metaInfoType.Attributes?[4].Value;
        }

        #endregion
    }
}
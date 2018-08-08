using System.Xml;

namespace TDMtoTDSMigrator {
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
}
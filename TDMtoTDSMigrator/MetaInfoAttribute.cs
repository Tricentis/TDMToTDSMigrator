using System.Xml;

namespace TDMtoTDSMigrator {
    public class MetaInfoAttribute {
        public string AttributeId;

        public string AttributeName;

        public string CorrespondingCategoryId;

        public MetaInfoAttribute(XmlNode metaInfoAttribute) {
            AttributeId = metaInfoAttribute.Attributes?[0].Value; //ex:"3"
            AttributeName = metaInfoAttribute.Attributes?[1].Value; //ex:"Adress"
            CorrespondingCategoryId = metaInfoAttribute.Attributes?[2].Value; //ex:"4"
        }
    }
}